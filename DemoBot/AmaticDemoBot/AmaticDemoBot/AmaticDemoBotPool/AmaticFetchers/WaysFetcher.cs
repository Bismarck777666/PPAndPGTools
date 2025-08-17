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

namespace AmaticDemoBot
{
    public class FlyingDutchmanFetcher : SpinDataFetcher
    {
        public FlyingDutchmanFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new FlyingDutchmanFetcher(proxyIndex, config));
        }

        protected override async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            _cnt++;
            if(packet.messagetype == (long)MessageType.NormalSpin)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 0;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = message.Split('#')[0];

                if (_onlyFree != 1 && (double)packet.win / (1 * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendFreeSpinRequest();
            }
        }
        
        protected override async Task receiveCollectResponse(AmaPacket packet, string message)
        {
            _freeSpinStack      = new List<string>();
            _tembleSpinStack    = new List<string>();
            if (_cnt >= 1000 || packet.balance < 1 * _playMini)
            {
                _self.Tell(new RestartMessage());
            }
            else
                await sendSpinRequest();
        }

        protected override async Task receiveFreeSpinResponse(AmaPacket packet, string message)
        {
            if (!_nowFreeSpin)
            {
                _logger.Warning("Free spin response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.LastFree)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);
                if ((double)packet.win / (1 * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                await sendCollectRequest();
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }
    }
}