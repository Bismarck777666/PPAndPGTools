using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using CQ9DemoBot.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ9DemoBot.CQ9Fetchers
{
    public class Temble9TicketFetcher : SpinDataFetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public Temble9TicketFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new Temble9TicketFetcher(proxyIndex, config));
        }
        protected override async Task sendSpinRequest()
        {
            Spin10TicketRequest requestParam = new Spin10TicketRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = 1;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.PlayDenom      = _playdenom;
            requestParam.MiniBet        = _playmini;
            requestParam.ReelPay        = 0;
            requestParam.ReelSelected   = new int[] { };
            requestParam.ActionMode     = 0;
            requestParam.Ticket         = "";

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }

        protected override async Task receiveSpinResponse(JObject response)
        {
            response = removeNormalSpinCommonParams(response);
            int nextModule      = Convert.ToInt32(response.Property("NextModule").Value);
            if (nextModule == (int)NextModule.Normal)
            {
                bool isRespin = Convert.ToBoolean(response.Property("IsRespin").Value);
                long totalWin = Convert.ToInt64(response.Property("TotalWin").Value);
                if(isRespin)
                {
                    _tembleSpinStack = new List<string>();
                    _tembleSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendTembleRequest();
                    return;
                }
                _cnt++;
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType   = 0;
                spinResponse.TotalWin   = totalWin;
                spinResponse.Response   = JsonConvert.SerializeObject(response);
                if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                await sendSpinCheckRequest();
            }
            else
            {
                if(nextModule == (int)NextModule.FreeStart)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinStartRequest();
                }
            }
        }

        protected override async Task sendTembleRequest()
        {
            Temble9TicketRequest requestParam = new Temble9TicketRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalTembleRequest;
            requestParam.PlayLine       = 1;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.MiniBet        = _playmini;
            requestParam.ReelSelected   = new int[] { };
            requestParam.ActionMode     = 0;
            requestParam.Ticket         = "";

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }

        protected override async Task receiveTembleResponse(JObject response)
        {
            response = removeNormalSpinCommonParams(response);
            int nextModule      = Convert.ToInt32(response.Property("NextModule").Value);
            if (nextModule == (int)NextModule.Normal)
            {
                long totalWin = Convert.ToInt64(response.Property("TotalWin").Value);
                
                if(_freeSpinStack == null || _freeSpinStack.Count == 0)
                {
                    _tembleSpinStack.Add(JsonConvert.SerializeObject(response));
                    if (totalWin > 0)
                    {
                        await sendTembleRequest();
                        return;
                    }
                    foreach (string itemStr in _tembleSpinStack)
                    {
                        dynamic item = JsonConvert.DeserializeObject(itemStr);
                        long win    = Convert.ToInt64(item["TotalWin"]);
                        long specialAward = Convert.ToInt64(item["SpecialAward"]);
                        if (win == 0)
                            win += specialAward;
                        totalWin += win;
                    }
                    _cnt++;
                    SpinResponse spinResponse = new SpinResponse();
                    spinResponse.SpinType = 0;
                    spinResponse.TotalWin = totalWin;
                    spinResponse.Response = string.Join("\n", _tembleSpinStack);
                    if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                        SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                }
                else
                {
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    if (totalWin > 0)
                    {
                        await sendTembleRequest();
                        return;
                    }
                    
                    foreach (string itemStr in _freeSpinStack)
                    {
                        dynamic item = JsonConvert.DeserializeObject(itemStr);
                        long win            = Convert.ToInt64(item["TotalWin"]);
                        long specialAward   = Convert.ToInt64(item["SpecialAward"]);
                        
                        if (win == 0)
                            win += specialAward;
                        totalWin += win;
                    }
                    _cnt++;
                    SpinResponse spinResponse = new SpinResponse();
                    spinResponse.SpinType = 1;
                    spinResponse.TotalWin = totalWin;
                    spinResponse.Response = string.Join("\n", _freeSpinStack);
                    if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                        SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                }
                await sendSpinCheckRequest();
            }
            else
            {
                if(nextModule == (int)NextModule.FreeStart)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    if(_tembleSpinStack != null)
                    {
                        foreach(string item in _tembleSpinStack)
                        {
                            _freeSpinStack.Add(item);
                        }
                    }
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinStartRequest();
                }
            }
        }

        protected override async Task receiveFreeSpinResponse(JObject response)
        {
            try
            {
                if (!_nowFreeSpin)
                {
                    _logger.Warning("Free spin response when not triggered free spin");
                    _self.Tell(new RestartMessage());
                    return;
                }
                response = removeFreeSpinCommonParams(response);
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));

                int awaredSpinTimes     = Convert.ToInt32(response.Property("AwardSpinTimes").Value);
                int currentSpinTimes    = Convert.ToInt32(response.Property("CurrentSpinTimes").Value);
                long totalwin           = Convert.ToInt64(response.Property("TotalWin").Value);
                if (currentSpinTimes == awaredSpinTimes && totalwin == 0)
                    await sendFreeSpinResultRequest();
                else
                    await sendFreeSpinRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive freespin response ex: {0}", ex);
                _self.Tell(new RestartMessage());
            }
        }

        protected override async Task receiveFreeSpinResultResponse(JObject response)
        {
            try
            {
                if (!_nowFreeSpin)
                {
                    _logger.Warning("Free spin sum response when not triggered free spin");
                    _self.Tell(new RestartMessage());
                    return;
                }
                response = removeCheckCommonParams(response);
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));

                long totalWin = Convert.ToInt64(response.Property("TotalWinAmt").Value);
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.TotalWin = totalWin;
                spinResponse.Response = string.Join("\n", _freeSpinStack);
                if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                        SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                await sendSpinCheckRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive free sum response ex: {0}", ex);
                _self.Tell(new RestartMessage());
            }
        }
    }
}
