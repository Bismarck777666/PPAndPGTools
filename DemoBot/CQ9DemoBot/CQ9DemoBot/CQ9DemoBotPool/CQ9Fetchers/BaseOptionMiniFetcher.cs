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
    public class BaseOptionMiniFetcher : SpinDataFetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public BaseOptionMiniFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new BaseOptionMiniFetcher(proxyIndex, config));
        }
        protected override async Task sendSpinRequest()
        {
            MiniSpinRequest requestParam = new MiniSpinRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = _playline;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.PlayDenom      = _playdenom;
            requestParam.MiniBet        = _playmini;

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }
        protected override async Task receiveSpinResponse(JObject response)
        {
            response = removeNormalSpinCommonParams(response);
            bool isTriggerFG        = Convert.ToBoolean(response.Property("IsTriggerFG").Value);
            int nextModule          = Convert.ToInt32(response.Property("NextModule").Value);
            
            JArray reelPayArray     = (JArray)response["ReelPay"];
            JArray udsOutputWinLine = (JArray)response["udsOutputWinLine"];
            
            for(int i = 0; i < reelPayArray.Count; i++)
            {
                int reelPayVal = Convert.ToInt32(reelPayArray[i]);
                if(reelPayVal != 0 && udsOutputWinLine.Count > 0)
                {
                    await sendSpinCheckRequest();
                    return;
                }
            }

            if (nextModule == (int)NextModule.Normal)
            {
                long totalWin = Convert.ToInt64(response.Property("TotalWin").Value);
                _cnt++;
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 0;
                spinResponse.TotalWin = totalWin;
                spinResponse.Response = JsonConvert.SerializeObject(response);
                if ((double)totalWin / (_playbet * _playmini) >= _minOdd && (double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                await sendSpinCheckRequest();
            }
            else if (nextModule == (int)NextModule.FreeStart)
            {
                _nowFreeSpin = true;
                _freeSpinStack = new List<string>();
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                await sendFreeSpinStartRequest();
            }
            else if (nextModule == (int)NextModule.BaseOption)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                await sendFreeSpinOptionReqeust();
            }
            else if (nextModule == (int)NextModule.FreeOption)
                {
                    _nowFreeSpin = true;
                    _freeSpinStack = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinOptionReqeust();
                }
        }
        protected override async Task receiveFreeSpinOptionResponse(JObject response)
        {
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            await sendFreeSpinOptSelectRequest(4, 0);
        }
        protected override async Task receiveFreeSpinOptSelectResponse(JObject response)
        {
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            await sendFreeSpinOptResultRequest();
        }
        protected override async Task receiveFreeSpinOptResultResponse(JObject response)
        {
            long totalWin = Convert.ToInt64(response.Property("TotalWinAmt").Value);
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            int nextModule = Convert.ToInt32(response.Property("NextModule").Value);
            if ((NextModule)nextModule == NextModule.FreeStart)
                await sendFreeSpinStartRequest();
            else if ((NextModule)nextModule == NextModule.Normal)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.TotalWin = totalWin;
                spinResponse.Response = string.Join("\n", _freeSpinStack);
                if ((double)totalWin / (_playbet * _playmini) >= _minOdd && (double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                await sendSpinCheckRequest();
            }
        }
    }
}
