using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using CQ9DemoBot.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ9DemoBot.CQ9Fetchers
{
    public class OptionGameFetcher : SpinDataFetcher
    {
        private readonly ILoggingAdapter _logger    = Logging.GetLogger(Context);
        protected int       _freeSpinOptionCount    = 5;
        protected int       _freeSpinOptionIndex    = -1;
        protected double    _freeAccmWin            = 0.0f;
        protected long      startTotalWin           = 0;
        protected IList<int> _freeOpts = new List<int>();

        public OptionGameFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _freeOpts   = config.GetIntList("freeOpts");
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new OptionGameFetcher(proxyIndex, config));
        }
        protected override async Task sendSpinRequest()
        {
            Spin10MemberRequest requestParam = new Spin10MemberRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = _playline;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.PlayDenom      = _playdenom;
            requestParam.MiniBet        = _playmini;
            requestParam.ReelPay        = 0;
            requestParam.ReelSelected   = new int[] { 0, 0, 0, 0, 0 };
            requestParam.ActionMode     = 0;

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }
        protected override async Task receiveFreeSpinStartResponse(JObject response)
        {
            try
            {
                if (!_nowFreeSpin)
                {
                    _logger.Warning("Free spin start response when not triggered free spin");
                    _self.Tell(new RestartMessage());
                    return;
                }
                response = removeCheckCommonParams(response);
                response["ScatterPayFromBaseGame"] = 0;
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                await sendFreeSpinRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive freespin start response ex: {0}", ex);
                _self.Tell(new RestartMessage());
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
                response["AccumlateWinAmt"] = Convert.ToInt64(response.Property("AccumlateWinAmt").Value) - Convert.ToInt64(response.Property("ScatterPayFromBaseGame").Value);
                response["ScatterPayFromBaseGame"] = 0;

                _freeSpinStack.Add(JsonConvert.SerializeObject(response));

                int awaredSpinTimes = Convert.ToInt32(response.Property("AwardSpinTimes").Value);
                int currentSpinTimes = Convert.ToInt32(response.Property("CurrentSpinTimes").Value);
                if (currentSpinTimes < awaredSpinTimes)
                    await sendFreeSpinRequest();
                else
                    await sendFreeSpinResultRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive freespin response ex: {0}", ex);
                _self.Tell(new RestartMessage());
            }
        }
        protected override async Task receiveFreeSpinOptionResponse(JObject response)
        {
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            int index = Pcg.Default.Next(0, _freeOpts.Count);
            _freeSpinOptionIndex = _freeOpts[index];
            await sendFreeSpinOptSelectRequest(4, _freeSpinOptionIndex);
        }
        protected override async Task receiveFreeSpinOptSelectResponse(JObject response)
        {
            response["ScatterPayFromBaseGame"] = 0;
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            await sendFreeSpinOptResultRequest();
        }
        protected override async Task receiveFreeSpinOptResultResponse(JObject response)
        {
            response["TotalWinAmt"]             = 0;
            response["ScatterPayFromBaseGame"]  = 0;
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            int nextModule = Convert.ToInt32(response.Property("NextModule").Value);
            if ((NextModule)nextModule == NextModule.FreeStart)
                await sendFreeSpinStartRequest();
            else if ((NextModule)nextModule == NextModule.Normal)
                await sendSpinCheckRequest();
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

                long totalWin       = Convert.ToInt64(response.Property("TotalWinAmt").Value);
                int nextModule      = Convert.ToInt32(response.Property("NextModule").Value);

                if(_freeAccmWin == 0)   //프리스핀 라운드1인경우
                {
                    
                    var startResponse   = (JObject)JsonConvert.DeserializeObject(_freeSpinStack[0]);

                    long startWin = 0;
                    if(!object.ReferenceEquals(startResponse["TotalWin"],null))
                        startWin = Convert.ToInt64(startResponse.Property("TotalWin").Value);

                    response["ScatterPayFromBaseGame"]  = 0;
                    response["TotalWinAmt"]             = Convert.ToInt64(response["TotalWinAmt"]) - startWin;
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));

                    if ((NextModule)nextModule == NextModule.FreeOption)
                    {
                        _freeSpinOptionIndex    = -1;
                        _freeAccmWin            = totalWin;
                        _freeSpinStack.Clear();
                        await sendFreeSpinOptionReqeust();
                    }
                    else
                    {
                        List<string> freeStartList = new List<string>();
                        List<string> freeList       = new List<string>();
                        freeStartList.Add(_freeSpinStack[0]);
                        freeStartList.Add(_freeSpinStack[1]);

                        for(int i = 2; i < _freeSpinStack.Count; i++)
                        {
                            freeList.Add(_freeSpinStack[i]);
                        }

                        OptionGameResponse freeStartResponse = new OptionGameResponse();
                        freeStartResponse.SpinType   = 100;
                        freeStartResponse.TotalWin   = totalWin;
                        freeStartResponse.RealWin    = startWin;
                        freeStartResponse.Response   = string.Join("\n", freeStartList);
                        
                        if(totalWin / (_playbet * _playmini) >= _minOdd && (double)totalWin / (_playbet * _playmini) <= _maxOdd)
                            SpinDataQueue.Instance.insertSpinDataToQueue(freeStartResponse);

                        OptionGameResponse freeBodyResponse = new OptionGameResponse();
                        freeBodyResponse.SpinType   = 200 + _freeSpinOptionIndex;
                        freeBodyResponse.TotalWin   = totalWin - startWin;
                        freeBodyResponse.RealWin    = totalWin - startWin;
                        freeBodyResponse.Response   = string.Join("\n", freeList);

                        if (totalWin / (_playbet * _playmini) >= _minOdd && (double)totalWin / (_playbet * _playmini) <= _maxOdd)
                            SpinDataQueue.Instance.insertSpinDataToQueue(freeBodyResponse);
                    
                        _freeSpinOptionIndex    = -1;
                        _freeAccmWin            = 0;
                        _freeSpinStack.Clear();
                        await sendSpinCheckRequest();
                    }
                }
                else
                {
                    if((NextModule)nextModule == NextModule.FreeOption)
                    {
                        _freeSpinOptionIndex    = -1;
                        _freeAccmWin            = totalWin;
                        await sendFreeSpinOptionReqeust();
                    }
                    else
                    {
                        _freeSpinOptionIndex    = -1;
                        _freeAccmWin            = 0;
                        await sendSpinCheckRequest();
                    }
                    _freeSpinStack.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive free sum response ex: {0}", ex);
                _self.Tell("restart");
            }
        }
    }
}
