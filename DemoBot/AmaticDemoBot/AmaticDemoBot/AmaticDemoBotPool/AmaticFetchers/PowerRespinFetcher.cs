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
    public class PowerRespinFetcher : SpinDataFetcher
    {
        protected int _betStep = 0;
        public PowerRespinFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _betStep    = config.GetInt("betStep");
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new PowerRespinFetcher(proxyIndex, config));
        }

        protected override async Task receiveResponse(AmaPacket packet, string message)
        {
            if (packet.messagetype == (int)MessageType.PowerTrigger)
            {
                _cnt++;
                _nowFreeSpin = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendPowerSpinRequest();
            }
            else if(packet.messagetype == (int)MessageType.PowerRespin || packet.messagetype == (int)MessageType.PowerLast)
            {
                await receivePowerSpinResponse(packet, message);
            }
            else
            {
                await base.receiveResponse(packet, message);
            }
        }

        protected virtual async Task sendPowerSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2538",
                _playLine[_lineType].ToString(),
                _betStep.ToString(),
                "0"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receivePowerSpinResponse(AmaPacket packet, string message)
        {
            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.PowerLast)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 0;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);

                if ((double)packet.win / _realBet <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win != 0)
                    await sendCollectRequest();
                else
                    await sendSpinRequest();
            }
            else
            {
                await sendPowerSpinRequest();
            }
        }
    }
}