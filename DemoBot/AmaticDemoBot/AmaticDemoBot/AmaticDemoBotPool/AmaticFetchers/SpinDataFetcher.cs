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
    public class SpinDataFetcher : ReceiveActor
    {
        protected int                       _proxyIndex             = -1;
        protected IList<string>             _httpProxyList          = new List<string>();
        protected IList<string>             _socks5ProxyList        = new List<string>();
        protected string                    _proxyUserID            = null;
        protected string                    _proxyPassword          = null;
        protected string                    _gameName               = "";
        protected string                    _gameID                 = "";
        protected string                    _gameConfig             = "";
        protected string                    _initString             = "";
        protected int                       _onlyFree               = 0;

        protected IList<int>                _playLine               = new List<int>();
        protected int                       _lineType               = -1;
        protected int                       _playMini               = 0;
        protected int                       _realBet                = 0;
        protected int                       _reelCnt                = 5;
        protected int                       _freeReelCnt            = 5;
        protected double                    _minOdd                 = 0.0;
        protected double                    _maxOdd                 = 3000.0;
        protected string                    _gameUrl                = null;
        protected string                    _wssUrl                 = null;

        protected ClientWebSocket           _ws1                    = null;
        protected bool                      _SocketConnected        = false;
        protected DateTime                  _LastMessageTime        = new DateTime(1970, 1, 1);
        protected readonly ILoggingAdapter  _logger                 = Logging.GetLogger(Context);

        protected ICancelable               _heartbeatCancelable    = null;
        protected IActorRef                 _self                   = null;
        protected bool                      _nowFreeSpin            = false;
        protected List<string>              _freeSpinStack          = null;
        protected List<string>              _tembleSpinStack        = null;
        protected int                       _cnt                    = 0;    //한세션동안 넣는 스핀번호(최대 99,999,999이상)
        protected AmaDecrypt                _amaConverter           = null; 

        public SpinDataFetcher(int proxyIndex, Config config)
        {
            _proxyIndex         = proxyIndex;
            _httpProxyList      = config.GetStringList("httpProxyList");
            _socks5ProxyList    = config.GetStringList("socks5ProxyList");
            _proxyUserID        = config.GetString("proxyUserID");
            _proxyPassword      = config.GetString("proxyPassword");
            _gameName           = config.GetString("gameName");
            _gameID             = config.GetString("gameID");
            _gameConfig         = config.GetString("gameConfig");

            _playLine           = config.GetIntList("playline");
            _lineType           = config.GetInt("linetype");
            _playMini           = config.GetInt("playmini");
            _realBet            = config.GetInt("realBet");
            _reelCnt            = config.GetInt("reelCnt");
            _freeReelCnt        = config.GetInt("freeReelCnt");
            _initString         = config.GetString("initString");
            _gameUrl            = config.GetString("gameURL");
            _wssUrl             = config.GetString("WssURL");
            _minOdd             = config.GetDouble("minOdd");
            _maxOdd             = config.GetDouble("maxOdd");
            _onlyFree           = config.GetInt("onlyFree");

            ReceiveAsync<string>                (processCommand);
            ReceiveAsync<RestartMessage>        (procRestartMessage);
        }

        public static Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new SpinDataFetcher(proxyIndex, config));
        }
        
        protected override void PreStart()
        {
        }
        
        protected override void PostStop()
        {
            if (_heartbeatCancelable != null)
                _heartbeatCancelable.Cancel();

            base.PostStop();
        }
        
        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            _self.Tell(new RestartMessage());
        }
        
        private async Task processCommand(string command)
        {
            switch (command)
            {
                case "start":
                    await startFetch();
                    break;
                case "terminate":
                    _logger.Info(string.Format("Close Session"));
                    _self.Tell(PoisonPill.Instance);
                    break;
                case "sendheartbeat":
                    await sendHeartBeat();
                    break;
            }
        }
        
        protected async Task procRestartMessage(RestartMessage message)
        {
            try
            {
                if (_ws1 != null && _ws1.State == WebSocketState.Open)
                    await _ws1.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                _ws1 = null;

                Context.System.Scheduler.ScheduleTellOnce(500, Context.Parent, new FetcherStopMessage(_self.Path.Name, true), Self);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex.ToString());
            }
        }
        
        private async Task startFetch()
        {
            _self       = Self;
            try
            {
                var httpClientHandler = new HttpClientHandler();
#if PROXY
                var proxy = new WebProxy
                {
                    Address                 = new Uri(string.Format("http://{0}", _httpProxyList[_proxyIndex])),
                    BypassProxyOnLocal      = false,
                    UseDefaultCredentials   = false,
                    Credentials             = new NetworkCredential(
                    userName: _proxyUserID,
                    password: _proxyPassword)
                };
                httpClientHandler.Proxy = proxy;
#endif
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpClient httpClient = new HttpClient(httpClientHandler);
                string gameURL = string.Format("{0}/{1}.html?hash=freeplay&curr=EUR&lang=en&uselang=en&config=", _gameUrl, _gameID, _gameConfig);

                HttpResponseMessage message = await httpClient.GetAsync(gameURL);
                message.EnsureSuccessStatusCode();

                string strWebSocket = string.Format("{0}", _wssUrl, "");

                using (_ws1 = new ClientWebSocket())
                {
#if PROXY
                    _ws1.Options.Proxy = new WebProxy(_httpProxyList[_proxyIndex])
                    {
                        Credentials = new NetworkCredential(_proxyUserID, _proxyPassword)
                    };
#endif

                    await _ws1.ConnectAsync(new Uri(strWebSocket), CancellationToken.None);

                    _amaConverter = new AmaDecrypt();
                    _SocketConnected = true;
                    await sendInitRequest();

                    while (_ws1.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
                        WebSocketReceiveResult result = null;
                        using (var ms = new MemoryStream())
                        {
                            do
                            {
                                result = await _ws1.ReceiveAsync(buffer, CancellationToken.None);
                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    _SocketConnected = false;
                                    _logger.Info("closed connection : {0}", result.ToString());
                                    _self.Tell(new RestartMessage());
                                    await _ws1.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                }
                                else
                                {
                                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                                }
                            }
                            while (!result.EndOfMessage);

                            ms.Seek(0, SeekOrigin.Begin);

                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                string strResMessage = reader.ReadToEnd();
                                long sessionClose = await parseMessage(strResMessage);

                                if(sessionClose != 0)
                                {
                                    _SocketConnected = false;
                                    //await _ws1.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                                    await _ws1.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                                    // _logger.Info("closed connection : {0}", result.ToString());
                                    _self.Tell(new RestartMessage());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(string.Format("Exception has been occured in SpinDataFetcher ex:{0}",ex));
                _self.Tell(new RestartMessage());
            }
        }

        #region 소켓이벤트 핸들러
        public async Task<bool> SendString(ClientWebSocket ws, String data, CancellationToken cancellation)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer  = new ArraySegment<Byte>(encoded, 0, encoded.Length);
            try
            {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static async Task<String> ReadString(ClientWebSocket ws)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);

            WebSocketReceiveResult result = null;

            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
        }
        #endregion

        #region 소켓메시지 종류별 파싱
        protected async Task<long> parseMessage(string msgStr)
        {
            _amaConverter.setMessage(msgStr);

            long messageHeader = _amaConverter.Read1BitHexToDec();
            long sessionClose = 0;

            if (messageHeader == 0)
            {
                // 인이트메시지
                await sendSpinRequest();
            }
            else if(messageHeader == 1)
            {
                //스핀관련메시지
                sessionClose = await parseResponse(msgStr);
            }
            else if(messageHeader == 9)
            {

            }
            else
            {
                Console.WriteLine("unknow message header---> {0}", msgStr);
            }

            return sessionClose;
        }
        protected virtual async Task<long> parseResponse(string responseStr)
        {
            try
            {
                AmaPacket packet = new AmaPacket(responseStr, _reelCnt, _freeReelCnt);

                if(packet.messagetype == 5)
                {

                }
                //if(packet.sessionclose != 0)
                //{
                //    _logger.Info("demo session closed : {0} {1}", packet.messagetype, packet.sessionclose);
                //    _self.Tell(new RestartMessage());
                //}

                if(packet.sessionclose == 0)
                    await receiveResponse(packet, responseStr);
                return packet.sessionclose;
            }
            catch (Exception ex)
            {
                _logger.Warning("Exception has been occured in parse response ex: {0} {1}", ex, responseStr);
                return -1;
            }
        }
        protected virtual async Task receiveResponse(AmaPacket packet, string msgStr)
        {
            //_logger.Info(msgStr);
            if (packet.messagetype == (long)MessageType.NormalSpin || packet.messagetype == (long)MessageType.FreeStart || packet.messagetype == (long)MessageType.RespinTrigger || packet.messagetype == (long)MessageType.WheelTrigger)
            {
                await receiveSpinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.FreeSpin || packet.messagetype == (long)MessageType.ExtendFree || packet.messagetype == (long)MessageType.LastFree)
            {
                await receiveFreeSpinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.Respin || packet.messagetype == (long)MessageType.LastRespin)
            {
                await receiveRespinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.Wheel || packet.messagetype == (long)MessageType.LastWheel)
            {
                await receiveWheelResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.FreeRespinStart || packet.messagetype == (long)MessageType.FreeRespin || packet.messagetype == (long)MessageType.FreeRespinEnd)
            {
                await receiveFreeRespinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.DiamondStart || packet.messagetype == (long)MessageType.DiamondSpin || packet.messagetype == (long)MessageType.DiamondEnd)
            {
                await receiveDiamondSpinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.FreeCashStart || packet.messagetype == (long)MessageType.FreeCashSpin || packet.messagetype == (long)MessageType.FreeCashEnd)
            {
                await receiveFreeCashSpinResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.BonusTrigger || packet.messagetype == (long)MessageType.BonusSpin || packet.messagetype == (long)MessageType.BonusEnd)
            {
                await receiveBonusResponse(packet, msgStr);
            }
            else if (packet.messagetype == (long)MessageType.Collect)
            {
                await receiveCollectResponse(packet, msgStr);
            }
            else
            {
                _logger.Info("unknow messagetype : {0}", packet.messagetype);
                _self.Tell(new RestartMessage());
            }
        }
        protected async Task sendMessage(string str)
        {
            if (!_SocketConnected)
                return;
            
            string message = str;
            //Console.WriteLine("Send message -->{0}",message);
            await Task.Delay(500);
            bool flag = await SendString(_ws1,message,CancellationToken.None);
        }

        #endregion

        #region 셋팅소켓메시지
        protected async Task sendHeartBeat()
        {
            if (!_SocketConnected)
                return;

            await sendMessage("A/u250");
            _heartbeatCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(3000, Self, "sendheartbeat", ActorRefs.NoSender);
        }

        protected virtual async Task sendInitRequest()
        {
            if (!_SocketConnected)
                return;

            await sendMessage(_initString);
        }

        #endregion

        #region 소켓메시지관련 가상함수들
        protected virtual async Task sendSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u251",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receiveSpinResponse(AmaPacket packet, string message)
        {
            _cnt++;
            if(packet.messagetype == (long)MessageType.NormalSpin)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 0;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = message.Split('#')[0];

                if (_onlyFree != 1 && (double)packet.win / (_realBet * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else if(packet.messagetype == (long)MessageType.FreeStart)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendFreeSpinRequest();
            }
            else if (packet.messagetype == (long)MessageType.RespinTrigger)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendRespinRequest();
            }
            else if (packet.messagetype == (long)MessageType.WheelTrigger)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
                _freeSpinStack.Add(message.Split('#')[0]);

                await sendWheelRequest();
            }
        }

        protected virtual async Task sendCollectRequest()
        {
            await sendMessage("A/u254");
        }

        protected virtual async Task receiveCollectResponse(AmaPacket packet, string message)
        {
            _nowFreeSpin        = false;
            _freeSpinStack      = new List<string>();
            _tembleSpinStack    = new List<string>();

            int bet = _playLine[_lineType];
            if (bet == 9)
                bet = 1;

            if (_cnt >= 1000 || packet.balance < bet * _playMini)
            {
                _self.Tell(new RestartMessage());
            }
            else
                await sendSpinRequest();
        }

        protected virtual async Task sendFreeSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u256",
                _playLine[_lineType].ToString(),
                "0",
                "0"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receiveFreeSpinResponse(AmaPacket packet, string message)
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

                if ((double)packet.win / _realBet <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win != 0)
                    await sendCollectRequest();
                else
                    await sendSpinRequest();
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }

        protected virtual async Task sendRespinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2531",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receiveRespinResponse(AmaPacket packet, string message)
        {
            if (!_nowFreeSpin)
            {
                _logger.Warning("Respin response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.LastRespin)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);

                if ((double)packet.win / (_realBet * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if(packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else
            {
                await sendRespinRequest();
            }
        }

        protected virtual async Task sendWheelRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2510",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receiveWheelResponse(AmaPacket packet, string message)
        {
            if (!_nowFreeSpin)
            {
                _logger.Warning("Wheel response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.LastWheel)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);

                if ((double)packet.win / (_realBet * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else
            {
                await sendWheelRequest();
            }
        }

        protected virtual async Task sendFreeRespinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2535",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        //프리스핀안에서 리스핀
        protected virtual async Task receiveFreeRespinResponse(AmaPacket packet, string message)
        {
            if (!_nowFreeSpin)
            {
                _logger.Warning(" Respin response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.FreeRespinStart || packet.messagetype == (long)MessageType.FreeRespin)
            {
                await sendFreeRespinRequest();
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }

        protected virtual async Task sendDiamondSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2558",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        //프리스핀안에서 리스핀
        protected virtual async Task receiveDiamondSpinResponse(AmaPacket packet, string message)
        {
            if (packet.messagetype == (long)MessageType.DiamondStart)
            {
                _nowFreeSpin    = true;
                _freeSpinStack  = new List<string>();
            }

            if (!_nowFreeSpin)
            {
                _logger.Warning(" Diamond response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if(packet.messagetype == (long)MessageType.DiamondStart || packet.messagetype == (long)MessageType.DiamondSpin)
            {
                await sendDiamondSpinRequest();
            }
            else
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);

                if ((double)packet.win / (_realBet * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if (packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
        }

        protected virtual async Task sendFreeCashSpinRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2553",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        //프리스핀안에서 리스핀
        protected virtual async Task receiveFreeCashSpinResponse(AmaPacket packet, string message)
        {
            if (!_nowFreeSpin)
            {
                _logger.Warning("Free cash response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if(packet.messagetype == (long)MessageType.FreeCashStart || packet.messagetype == (long)MessageType.FreeCashSpin)
            {
                await sendFreeCashSpinRequest();
            }
            else
            {
                await sendFreeSpinRequest();
            }
        }

        protected virtual async Task sendPurRequest(int pur)
        {
            List<string> paramList = new List<string>()
            {
                "A/u2566",
                _playLine[_lineType].ToString(),
                "0",
                "0",
                pur.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task sendOptionRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2517",
                0.ToString(),
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task sendBonusRequest()
        {
            List<string> paramList = new List<string>()
            {
                "A/u2546",
                _playLine[_lineType].ToString(),
                "0",
                "2"
            };

            string requestMsg = string.Join(",", paramList);
            await sendMessage(requestMsg);
        }

        protected virtual async Task receiveBonusResponse(AmaPacket packet, string message)
        {
            if (packet.messagetype == (long)MessageType.BonusTrigger)
            {
                _nowFreeSpin = true;
                _freeSpinStack = new List<string>();
            }

            if (!_nowFreeSpin)
            {
                _logger.Warning("Bonus response when not triggered free spin");
                _self.Tell(new RestartMessage());
                return;
            }

            _freeSpinStack.Add(message.Split('#')[0]);

            if (packet.messagetype == (long)MessageType.BonusEnd)
            {
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType = 1;
                spinResponse.LineType = _lineType;
                spinResponse.TotalWin = packet.win;
                spinResponse.Response = string.Join("\n", _freeSpinStack);

                if ((double)packet.win / (_realBet * _playMini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);

                if(packet.win == 0)
                    await sendSpinRequest();
                else
                    await sendCollectRequest();
            }
            else
            {
                await sendBonusRequest();
            }
        }

        

        #endregion
    }

    public class RestartMessage
    {

    }

    public class DataReadWebsocket
    {
        public bool State { get; set; }
        public DataReadWebsocket(bool state)
        {
            State = state;
        }
    }
    
    public class DataMessageReceived
    {
        public string MsgData { get; private set; }
        public DataMessageReceived(string msgData)
        {
            this.MsgData = msgData;
        }
    }
}