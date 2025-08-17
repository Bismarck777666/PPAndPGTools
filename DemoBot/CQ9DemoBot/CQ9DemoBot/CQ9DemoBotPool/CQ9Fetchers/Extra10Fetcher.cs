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
    public class Extra10Fetcher : SpinDataFetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public Extra10Fetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new Extra10Fetcher(proxyIndex, config));
        }
        protected override async Task sendSpinRequest()
        {
            Spin10MemberRequest requestParam = new Spin10MemberRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = _playline;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = _isExtraBet;
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

        protected override async Task receiveSpinResponse(JObject response)
        {
            response = removeNormalSpinCommonParams(response);
            bool isTriggerFG    = Convert.ToBoolean(response.Property("IsTriggerFG").Value);
            int nextModule      = Convert.ToInt32(response.Property("NextModule").Value);
            
            if (!isTriggerFG)
            {
                long totalWin = Convert.ToInt64(response.Property("TotalWin").Value);
                _cnt++;
                ExtraGameResponse spinResponse = new ExtraGameResponse();
                spinResponse.SpinType   = 0;
                spinResponse.Extra      = _isExtraBet;
                spinResponse.TotalWin   = totalWin;
                spinResponse.Response   = JsonConvert.SerializeObject(response);

                if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                await sendSpinCheckRequest();
            }
            else
            {
                if (nextModule == (int)NextModule.FreeStart)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinStartRequest();
                }
                else if (nextModule == (int)NextModule.FreeOption)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinOptionReqeust();
                }
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
                ExtraGameResponse spinResponse = new ExtraGameResponse();
                spinResponse.SpinType   = 1;
                spinResponse.Extra      = _isExtraBet;
                spinResponse.TotalWin   = totalWin;
                spinResponse.Response   = string.Join("\n", _freeSpinStack);
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
