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
    public class OldFetcher : SpinDataFetcher
    {
        protected string _hashKey = "";
        public OldFetcher(int proxyIndex, Config config) : base(proxyIndex, config)
        {
            _hashKey = config.GetString("hashKey");
        }

        public static new Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new OldFetcher(proxyIndex, config));
        }

        protected override async Task sendInitRequest()
        {
            _initString = string.Format("{0}{1}", _initString, _hashKey);
            await base.sendInitRequest();
        }

        protected override async Task sendSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u251", _hashKey),
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendCollectRequest()
        {
            await sendMessage(string.Format("{0}{1}", "A/u254", _hashKey));
        }

        protected override async Task sendFreeSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u256" , _hashKey),
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendRespinRequest()
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u2531" , _hashKey),
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendWheelRequest()
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u2510" , _hashKey),
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendPurRequest(int pur)
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u2566" , _hashKey),
                _playLine[_lineType].ToString(),
                "0",
                "0",
                pur.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task sendOptionRequest()
        {
            List<string> paramList = new List<string>()
            {
                string.Format("{0}{1}","A/u2517" , _hashKey),
                0.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected override async Task<long> parseResponse(string responseStr)
        {
            if(string.Equals(responseStr.Substring(0, 3), "104"))
            {
                await receiveCollectResponse(null, responseStr);
                return 0;
            }
            else
            {
                return await base.parseResponse(responseStr);
            }
        }

        protected override async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            message = buildSpinString(packet);

            await base.receiveSpinResponse(packet, message);
        }

        protected override async Task receiveFreeSpinResponse(AmaPacket packet, string message)
        {
            message = buildSpinString(packet);
            
            await base.receiveFreeSpinResponse(packet, message);
        }

        protected override async Task receiveRespinResponse(AmaPacket packet, string message)
        {
            message = buildSpinString(packet);
            await base.receiveRespinResponse(packet, message);
        }

        protected override async Task receiveWheelResponse(AmaPacket packet, string message)
        {
            message = buildSpinString(packet);

            await base.receiveWheelResponse(packet, message);
        }

        protected override async Task receiveCollectResponse(AmaPacket packet, string message)
        {
            _nowFreeSpin = false;
            _freeSpinStack = new List<string>();
            _tembleSpinStack = new List<string>();

            await sendSpinRequest();
        }


        protected virtual string buildSpinString(AmaPacket packet)
        {
            AmaEncrypt encrypt = new AmaEncrypt();
            string newSpinString = string.Empty;

            newSpinString = encrypt.WriteDecHex(newSpinString, packet.messageheader);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.messagetype);
            newSpinString = encrypt.WriteDecHex(newSpinString, packet.sessionclose);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.messageid);

            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.balance);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.win);

            int reelStopCnt = packet.reelstops.Count > 5 ? packet.reelstops.Count : 5;
            if (packet.reelstops.Count >= 5)
            {
                for (int i = 0; i < reelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.reelstops[i]);
            }
            else
            {
                for (int i = 0; i < packet.reelstops.Count; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.reelstops[i]);

                for (int i = packet.reelstops.Count; i < reelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, 0);
            }

            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.betstep);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.betline);

            
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[0]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[1]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[2]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[3]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[4]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.unknowparam3[5]);

            int freeReelStopCnt = packet.freereelstops.Count > 5 ? packet.freereelstops.Count : 5;
            if (packet.freereelstops.Count >= 5)
            {
                for (int i = 0; i < freeReelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freereelstops[i]);
            }
            else
            {
                for (int i = 0; i < packet.freereelstops.Count; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freereelstops[i]);

                for (int i = packet.freereelstops.Count; i < freeReelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, 0);
            }

            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.linewins.Count);
            for (int i = 0; i < packet.linewins.Count; i++)
            {
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.linewins[i]);
            }

            for (int i = 0; i < packet.gamblelogs.Count; i++)
            {
                newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.gamblelogs[i]);
            }

            return newSpinString;
        }
    }
}