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
    public class Option1Fetcher : SpinDataFetcher
    {
        public Option1Fetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new Option1Fetcher(proxyIndex, config));
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
            if(packet.messagetype == (long)MessageType.FreeOption)
            {
                _cnt++;
                _nowFreeSpin = true;
                _freeSpinStack  = new List<string>();

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
    }
}