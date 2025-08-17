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
    public class WheelMoneyFetcher : BaseOption8Fetcher
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public WheelMoneyFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }
        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new WheelMoneyFetcher(proxyIndex, config));
        }
        protected override async Task receiveFreeSpinOptSelectResponse(JObject response)
        {
            _freeSpinStack.Add(JsonConvert.SerializeObject(response));
            bool gameComplete = Convert.ToBoolean(response.Property("GameComplete").Value);
            if (gameComplete)
                await sendFreeSpinOptResultRequest();
            else
                await sendFreeSpinOptSelectRequest(4, 0);
        }
    }
}
