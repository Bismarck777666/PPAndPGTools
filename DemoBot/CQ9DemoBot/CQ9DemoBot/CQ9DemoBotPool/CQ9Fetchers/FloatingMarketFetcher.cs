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
    public class FloatingMarketFetcher : Temble8Fetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public FloatingMarketFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new FloatingMarketFetcher(proxyIndex, config));
        }
        protected override async Task sendSpinRequest()
        {
            Spin8MemberRequest requestParam = new Spin8MemberRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = _playline;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.PlayDenom      = _playdenom;
            requestParam.MiniBet        = _playmini;
            requestParam.ReelPay        = 0;

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
        
        protected override async Task sendTembleRequest()
        {
            FloatingMarketTembleRequest requestParam = new FloatingMarketTembleRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalTembleRequest;
            requestParam.PlayLine       = _playline;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.MiniBet        = _playmini;
            requestParam.PlayDenom      = _playdenom;
            requestParam.ReelSelected   = new int[] { };
            requestParam.ActionMode     = 0;
            requestParam.ReelPay        = 0;

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }
    }
    public class FloatingMarketTembleRequest
    {
        public int Type             { get; set; }
        public int ID               { get; set; }
        public int PlayLine         { get; set; }
        public int PlayBet          { get; set; }
        public int IsExtraBet       { get; set; }
        public int PlayDenom        { get; set; }
        public int MiniBet          { get; set; }
        public int ReelPay          { get; set; }
        public int[] ReelSelected   { get; set; }
        public int ActionMode       { get; set; }
    }
}
