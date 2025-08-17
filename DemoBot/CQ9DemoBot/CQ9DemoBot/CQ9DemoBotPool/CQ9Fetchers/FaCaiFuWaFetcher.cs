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
    public class FaCaiFuWaFetcher : Normal8Fetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public FaCaiFuWaFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new FaCaiFuWaFetcher(proxyIndex, config));
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
                int awardRound          = Convert.ToInt32(response.Property("AwardRound").Value);
                int currentRound        = Convert.ToInt32(response.Property("CurrentRound").Value);
                bool isRespin           = Convert.ToBoolean(response.Property("IsRespin").Value);

                if (currentSpinTimes < awaredSpinTimes)
                    await sendFreeSpinRequest();
                else
                    await sendFreeSpinResultRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("Exception is occured in receive freespin response ex: {0}",ex);
                _self.Tell(new RestartMessage());
            }
        }
    }
}
