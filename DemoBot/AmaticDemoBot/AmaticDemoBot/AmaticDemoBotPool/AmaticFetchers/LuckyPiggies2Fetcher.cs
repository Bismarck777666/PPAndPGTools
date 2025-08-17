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
    public class LuckyPiggies2Fetcher : AnteFetcher
    {
        protected int _purType = -1;
        public LuckyPiggies2Fetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _purType    = config.GetInt("purType");
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new LuckyPiggies2Fetcher(proxyIndex, config));
        }

        protected override async Task receiveResponse(AmaPacket packet, string msgStr)
        {
            if(packet.messagetype == (int)MessageType.PurBonus)
            {
                await receiveSpinResponse(packet, msgStr);
            }
            else
                await base.receiveResponse(packet, msgStr);
        }

        protected override async Task sendSpinRequest()
        {
            if (_purType == -1)
                await base.sendSpinRequest();
            else
                await sendPurRequest(_purType);
        }

        protected override async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            if (packet.messagetype == (long)MessageType.PurBonus)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendBonusRequest();
            }
            else
                await base.receiveSpinResponse(packet, message);
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
                if(_purType == -1)
                {
                    SpinResponse spinResponse = new SpinResponse();
                    spinResponse.SpinType = 1;
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
                    SpinResponse spinResponse = new SpinResponse();
                    spinResponse.SpinType = 1;
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
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }

        
    }
}