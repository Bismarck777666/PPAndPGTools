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
    public class OptionFetcher : SpinDataFetcher
    {
        protected int       _freeSpinOptionIndex    = -1;
        protected IList<int> _freeOpts = new List<int>();
        public OptionFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _freeOpts = config.GetIntList("freeOpts");
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new OptionFetcher(proxyIndex, config));
        }

        protected override async Task receiveResponse(AmaPacket packet, string msgStr)
        {
            if (packet.messagetype == (long)MessageType.FreeOption)
            {
                await receiveSpinResponse(packet, msgStr);
            }
            else
                await base.receiveResponse(packet, msgStr);    
        }

        protected override async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            _cnt++;
            if(packet.messagetype == (long)MessageType.FreeOption)
            {
                _nowFreeSpin = true;
                _freeSpinStack = new List<string>();

                _freeSpinStack.Add(message.Split('#')[0]);
                await sendOptionRequest();
            }
            else if (packet.messagetype == (long)MessageType.FreeStart)
            {
                _freeSpinStack.Add(message.Split('#')[0]);
                await sendFreeSpinRequest();
            }
            else
            {
                await base.receiveSpinResponse(packet, message);
            }
        }
        
        protected override async Task sendOptionRequest()
        {
            int index = Pcg.Default.Next(0, _freeOpts.Count);
            _freeSpinOptionIndex = _freeOpts[index];
            
            List<string> paramList = new List<string>()
            {
                "A/u2517",
                _freeSpinOptionIndex.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
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

            AmaPacket startPacket = new AmaPacket(_freeSpinStack[0], _reelCnt, _freeReelCnt);

            List<string> freeList = new List<string>();
            for (int i = 1; i < _freeSpinStack.Count; i++)
            {
                freeList.Add(_freeSpinStack[i]);
            }

            if (packet.messagetype == (long)MessageType.LastFree)
            {
                OptionGameResponse freeStartResponse = new OptionGameResponse();
                freeStartResponse.SpinType  = 100;
                freeStartResponse.LineType  = _lineType;
                freeStartResponse.TotalWin  = packet.win;
                freeStartResponse.RealWin   = startPacket.win;
                freeStartResponse.Response  = _freeSpinStack[0];
                if ((double)packet.win / _realBet <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(freeStartResponse);

                OptionGameResponse freeBodyResponse = new OptionGameResponse();
                freeBodyResponse.SpinType   = 200 + _freeSpinOptionIndex;
                freeBodyResponse.LineType   = _lineType;
                freeBodyResponse.TotalWin   = packet.win - startPacket.win;
                freeBodyResponse.RealWin    = packet.win - startPacket.win;
                freeBodyResponse.Response   = string.Join("\n", freeList);
                if ((double)packet.win / _realBet <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(freeBodyResponse);

                _nowFreeSpin = false;
                _freeSpinStack.Clear();
                if (packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }
    }
}