using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Newtonsoft.Json.Linq;
using AmaticDemoBot.Database;
using AmaticDemoBot.CQ9Fetchers;
using System.Net.WebSockets;
using System.Threading;
using System.IO;
using AmaticDemoBot.BitReader;
using PCGSharp;

namespace AmaticDemoBot
{
    public class AnteFetcher : SpinDataFetcher
    {
        protected int _anteType = -1;
        protected int _betStep = 0;
        public AnteFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _anteType   = config.GetInt("anteType");
            _betStep    = config.GetInt("betStep");
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new AnteFetcher(proxyIndex, config));
        }

        protected override async Task sendSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u251",
                _playLine[_lineType].ToString(),
                _betStep.ToString(),
                "0",
                _anteType.ToString()
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendPurRequest(int pur)
        {
            List<string> paramList = new List<string>()
            {
                "A/u2566",
                _playLine[_lineType].ToString(),
                _betStep.ToString(),
                "0",
                pur.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendFreeSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u256",
                _playLine[_lineType].ToString(),
                _betStep.ToString(),
                "0"
            };

            string requestMsg = string.Join(",", paramList);

            await sendMessage(requestMsg);
        }

        protected override async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            _cnt++;
            if (packet.messagetype != (long)MessageType.FreeStart)
            {
                await base.receiveSpinResponse(packet, message);
            }
            else if (packet.messagetype == (long)MessageType.FreeStart)
            {
                _nowFreeSpin = true;
                _freeSpinStack = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                //if(_anteType != -1)
                //{
                //    if (packet.unknowparam3[0] == 5 && packet.unknowparam3[1] == 5)
                //    {
                //        _self.Tell(new RestartMessage());
                //        return;
                //    }
                //    else if (packet.unknowparam3[0] == 7 && packet.unknowparam3[1] == 7)
                //    {
                //        _self.Tell(new RestartMessage());
                //        return;
                //    }
                //    else if (packet.unknowparam3[0] == 10 && packet.unknowparam3[1] == 10)
                //    {
                //        _self.Tell(new RestartMessage());
                //        return;
                //    }
                //}

                await sendFreeSpinRequest();
            }
        }

    }
}