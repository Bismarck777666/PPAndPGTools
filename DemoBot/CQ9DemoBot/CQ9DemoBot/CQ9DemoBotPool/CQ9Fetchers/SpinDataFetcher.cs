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
using CQ9DemoBot.Database;
using CQ9DemoBot.CQ9Fetchers;
using System.Net.WebSockets;
using System.Threading;
using System.IO;

namespace CQ9DemoBot
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
        protected string                    _userID                 = null;
        protected int                       _playbet                = 2;
        protected int                       _playdenom              = 100;
        protected int                       _isExtraBet             = 0;
        protected int                       _playmini               = 30;
        protected int                       _playline               = 1;
        protected double                    _realBet                = 0.0;
        protected double                    _minOdd                = 0.0;
        protected double                    _maxOdd                 = 3000.0;
        protected string                    _gameUrl                = null;
        protected string                    _wssUrl                 = null;

        protected ClientWebSocket           _ws1                    = null;

        protected string                    _sessionId              = null;
        protected AES16                     _Secure                 = null;
        protected AES16_Beta                _SecureReal             = null;
        protected string                    _frame1                 = "~m~";
        protected string                    _frame2                 = "~j~";
        protected bool                      _SocketConnected        = false;
        protected DateTime                  _LastMessageTime        = new DateTime(1970, 1, 1);
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);
        protected List<byte>                _receiveBuffer          = new List<byte>();

        protected ICancelable               _heartbeatCancelable    = null;
        protected IActorRef                 _self                   = null;
        protected bool                      _nowFreeSpin            = false;
        protected List<string>              _freeSpinStack          = null;
        protected List<string>              _tembleSpinStack        = null;
        protected int                       _cnt                    = 0;    //한세션동안 넣는 스핀번호(최대 99,999,999이상)
        protected bool                      _moneylimited           = false;

        public SpinDataFetcher(int proxyIndex, Config config)
        {
            _proxyIndex         = proxyIndex;
            _httpProxyList      = config.GetStringList("httpProxyList");
            _socks5ProxyList    = config.GetStringList("socks5ProxyList");
            _proxyUserID        = config.GetString("proxyUserID");
            _proxyPassword      = config.GetString("proxyPassword");
            _gameName           = config.GetString("gameName");
            _gameID             = config.GetString("gameID");
            _playbet            = config.GetInt("playbet");   
            _playdenom          = config.GetInt("playdenom"); 
            _isExtraBet         = config.GetInt("isExtraBet");
            _playmini           = config.GetInt("playmini");  
            _playline           = config.GetInt("playline");  
            _realBet            = config.GetDouble("realBet");
            _userID             = config.GetString("token");
            _gameUrl            = config.GetString("gameURL");
            _wssUrl             = config.GetString("WssURL");
            _minOdd             = config.GetDouble("minOdd");
            _maxOdd             = config.GetDouble("maxOdd");

            _self               = Self;
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
            _Secure     = new AES16();
            _sessionId  = null;
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
                //string gameURL = string.Format("{0}/{1}/?language=en&token={2}", _gameUrl,_gameID, _userID);
                string gameURL = string.Format("{0}/{1}/?language=en&token={2}", _gameUrl, _gameID, _userID);

                HttpResponseMessage message = await httpClient.GetAsync(gameURL);
                message.EnsureSuccessStatusCode();

                string clientInfoURL        = string.Format("{0}/clientinfo/?token={1}", _gameUrl, _userID);
                message = await httpClient.GetAsync(clientInfoURL);
                message.EnsureSuccessStatusCode();


                string strContent = await message.Content.ReadAsStringAsync();
                ClientInfoItem clientInfo = JsonConvert.DeserializeObject<ClientInfoItem>(strContent);

                Dictionary<string, string> paramsDic = new Dictionary<string, string>();
                paramsDic.Add("ip",         clientInfo.data.ip);
                paramsDic.Add("code",       clientInfo.data.code);
                paramsDic.Add("datatime",   clientInfo.data.datatime);
                paramsDic.Add("gameid",     _gameID.ToString());
                paramsDic.Add("token",      _userID);

                string strWebSocket = string.Format("{0}/?{1}", _wssUrl, combineURLParams(paramsDic));
                //await ConnectWebsocket(strWebSocket);

                using (_ws1 = new ClientWebSocket())
                {
#if PROXY
                    _ws1.Options.Proxy = new WebProxy(_httpProxyList[_proxyIndex])
                    {
                        Credentials = new NetworkCredential(_proxyUserID, _proxyPassword)
                    };
#endif

                    await _ws1.ConnectAsync(new Uri(strWebSocket), CancellationToken.None);
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
                                    _SocketConnected = true;
                                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                                }
                            }
                            while (!result.EndOfMessage);

                            ms.Seek(0, SeekOrigin.Begin);

                            using (var reader = new StreamReader(ms, Encoding.UTF8))
                            {
                                string strResMessage = reader.ReadToEnd();
                                await parseMessage(removeFrame(strResMessage));
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
        public byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

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
        protected async Task parseMessage(string msgStr)
        {
            //Console.WriteLine(msgStr);
            if (msgStr.Substring(0, 3) == _frame2)
            {
                if (msgStr.Contains("\"res\":"))
                {
                    ResponseResPacket resPacket = JsonConvert.DeserializeObject<ResponseResPacket>(msgStr.Substring(3));
                    await onMessageReceived(resPacket);
                    
                }
                else if (msgStr.Contains("\"irs\":"))
                {
                    ResponseIrsPacket irsPacket = JsonConvert.DeserializeObject<ResponseIrsPacket>(msgStr.Substring(3));
                    onMessageReceived(irsPacket);
                }
                else if (msgStr.Contains("\"evt\":"))
                {
                    ResponseEvtPacket evtPacket = JsonConvert.DeserializeObject<ResponseEvtPacket>(msgStr.Substring(3));
                    onMessageReceived(evtPacket);
                }
                else
                {
                    //처음 세션을 받을때(~j~가 없음)
                    if (this._sessionId == null)
                        this._sessionId = msgStr;
                    else
                    {
                        _logger.Warning("Received undefined message type : {0}", msgStr);
                        _self.Tell(new RestartMessage());
                    }
                }
            }
            else
            {
                //환경셋팅요청
                if (!string.IsNullOrEmpty(_sessionId))
                    onMessageReceived(msgStr);
                else
                {
                    this._sessionId = msgStr;
                    RequestReqPacket settingRequestPacket = new RequestReqPacket();
                    settingRequestPacket.req    = 1;
                    settingRequestPacket.vals   = string.Format("[1,\"{0}\",2,\"{1}\",3,\"{2}\"]", _Secure.doEncryptAndAddIV("WEB"),_userID,"en");
                    string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", settingRequestPacket.req, settingRequestPacket.vals));
                    await sendMessage(msg);
                }
            }
        }
        protected async Task onMessageReceived(ResponseResPacket obj)
        {
            if(obj.res == 1)
            {
                await sendInitUIRequest();
            }
            else if(obj.res == 2)
            {
                if(obj.err == 200)
                {
                    string decryptedStr = _Secure.doGetIVAndDecrypt(obj.vals[1], 0);
                    await parseResponse(decryptedStr);
                }
                else
                {
                    _logger.Warning("error occured in response receive ex :{0}", obj.msg);
                }
            }
        }
        protected void onMessageReceived(ResponseEvtPacket obj)
        {
            if(obj.vals[1] < 9)
            {
                _logger.Warning("Balance {0} is Less than 9", obj.vals[1]);
                _moneylimited = true;
                //_self.Tell(new RestartMessage());
                _self.Tell("terminate");
            }
        }
        protected void onMessageReceived(ResponseIrsPacket obj)
        {
            if(obj.err == 0)
            {
            }
            else
            {
                _logger.Warning("Internal response error : {0}", obj.msg);
                _self.Tell(new RestartMessage());
            }
        }
        protected virtual async Task parseResponse(string responseStr)
        {
            var response = (JObject)JsonConvert.DeserializeObject(responseStr);
            try
            {
                int type        = response["Type"].Value<int>();
                int id          = response["ID"].Value<int>();
                int errorcode   = response["ErrorCode"].Value<int>();
                if(type == 1 && id == 111)  //UI셋팅응답
                    await sendInitReelSetRequest();
                else if(type == 1 && id == 112) //Init셋팅응답
                {
                    //Context.System.Scheduler.ScheduleTellOnce(100, _self, new SendSpinMessage(), _self);
                     await sendSpinRequest();
                }
                else if(type == 3)  //베팅응답
                {
                    await receiveResponse(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("Exception has been occured in parse response ex: {0}", ex);
            }
        }
        protected void onMessageReceived(string str)
        {
            _logger.Warning("is not object response : {0}", str);
        }
        protected async Task receiveResponse(JObject response)
        {
            int id = response["ID"].Value<int>();
            int errorcode = response["ErrorCode"].Value<int>();
            if (errorcode != 0)
            {
                _logger.Info("server response error error code : {0}", errorcode);
                _self.Tell(new RestartMessage());
            }
            switch (id)
            {
                case (int)MessageCode.NormalSpinResponse:
                    await receiveSpinResponse(response);
                    break;
                case (int)MessageCode.NormalSpinResultResponse:
                    await receiveSpinCheckResponse(response);
                    break;
                case (int)MessageCode.FreeSpinStartResponse:
                    await receiveFreeSpinStartResponse(response);
                    break;
                case (int)MessageCode.FreeSpinResponse:
                    await receiveFreeSpinResponse(response);
                    break;
                case (int)MessageCode.FreeSpinSumResponse:
                    await receiveFreeSpinResultResponse(response);
                    break;
                case (int)MessageCode.FreeSpinOptionResponse:
                    await receiveFreeSpinOptionResponse(response);
                    break;
                case (int)MessageCode.FreeSpinOptSelectResponse:
                    await receiveFreeSpinOptSelectResponse(response);
                    break;
                case (int)MessageCode.FreeSpinOptResultResponse:
                    await receiveFreeSpinOptResultResponse(response);
                    break;
                case (int)MessageCode.NormalTembleResponse:
                    await receiveTembleResponse(response);
                    break;
                default:
                    break;
            }
        }
        protected async Task sendMessage(string str)
        {
            if (!_SocketConnected)
                return;
            string message = addFrame(str);
            //Console.WriteLine("Send message -->{0}",message);
            bool flag = await SendString(_ws1,message,CancellationToken.None);
        }

#endregion

#region 문자열 프레임 조작
        /// <summary>
        /// ~m~를 자르는 함수
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected string removeFrame(string strMessage)
        {
            strMessage = strMessage.Replace(" ", "");
            List<string> resultArray = new List<string>();
            do
            {
                if (strMessage.Substring(0, _frame1.Length) != _frame1)
                    break;
                strMessage = strMessage.Substring(_frame1.Length);
                string lengthStr    = "";
                int msgLength       = 0;
                for (int i = 0; i < strMessage.Length; i++)
                {
                    int number;
                    bool success = int.TryParse(strMessage[i] + "", out number);
                    if (success)
                        lengthStr += strMessage[i];
                    else
                    {
                        strMessage  = strMessage.Substring(lengthStr.Length + _frame1.Length);
                        msgLength   = Convert.ToInt32(lengthStr);
                        break;
                    }
                }
                int length = strMessage.Length > msgLength ? msgLength: strMessage.Length;
                resultArray.Add(strMessage.Substring(0, length));
                strMessage = strMessage.Substring(length);
            }
            while (strMessage != "");
            return resultArray[0];
        }
        /// <summary>
        /// ~m~  ~j~를 붙이는 함수
        /// </summary>
        /// <param name="strMessage"></param>
        /// <returns></returns>
        protected string addFrame(string strMessage)
        {
            long length = strMessage.Length + 3;
            strMessage = string.Format("{0}{1}{2}{3}{4}",_frame1,length.ToString(),_frame1,_frame2,strMessage);
            return strMessage;
        }
        protected string addBracket(string str)
        {
            str = "{" + str + "}";
            return str;
        }
#endregion

#region 공통파람 삭제
        protected JObject removeNormalSpinCommonParams(JObject obj)
        {
            obj.Property("Type").Remove();
            obj.Property("GamePlaySerialNumber").Remove();
            obj.Property("Version").Remove();
            obj.Property("ErrorCode").Remove();
            obj.Property("EmulatorType").Remove();
            return obj;
        }
        protected JObject removeFreeSpinCommonParams(JObject obj)
        {
            obj.Property("Type").Remove();
            obj.Property("Version").Remove();
            obj.Property("ErrorCode").Remove();
            obj.Property("EmulatorType").Remove();
            return obj;
        }
        protected JObject removeCheckCommonParams(JObject obj)
        {
            obj.Property("Type").Remove();
            obj.Property("Version").Remove();
            obj.Property("ErrorCode").Remove();
            return obj;
        }
#endregion

#region 셋팅소켓메시지
        protected async Task sendHeartBeat()
        {
            if (!_SocketConnected)
                return;
            RequestIrqPacket heartBeat = new RequestIrqPacket();
            heartBeat.irq = 1;
            List<long> vals = new List<long>() { 1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            heartBeat.vals = vals.ToArray();

            string heartBeatMessage = JsonConvert.SerializeObject(heartBeat);
             await sendMessage(heartBeatMessage);
            _heartbeatCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(3000, Self, "sendheartbeat", ActorRefs.NoSender);
        }
        protected async Task sendInitUIRequest()
        {
            RequestInitUI uiParam = new RequestInitUI();
            uiParam.Type      = 1;
            uiParam.ID        = 11;
            uiParam.GameID    = _gameID;

            RequestReqPacket uiRequest = new RequestReqPacket();
            uiRequest.req   = 2;
            uiRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(uiParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", uiRequest.req, uiRequest.vals));
            await sendMessage(msg);
        }
        protected async Task sendInitReelSetRequest()
        {
            RequestInitReelSet reelsetParam = new RequestInitReelSet();
            reelsetParam.Type      = 1;
            reelsetParam.ID        = 12;
            reelsetParam.State     = 0;

            RequestReqPacket reelSetRequest = new RequestReqPacket();
            reelSetRequest.req  = 2;
            reelSetRequest.vals = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(reelsetParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", reelSetRequest.req, reelSetRequest.vals));
            await sendMessage(msg);
        }
#endregion

#region 소켓메시지관련 가상함수들
        /// <summary>
        /// 일반스핀요청(31)
        /// </summary>
        protected virtual async Task sendSpinRequest()
        {
            Spin10MemberRequest requestParam = new Spin10MemberRequest();
            requestParam.Type           = 3;
            requestParam.ID             = (int)MessageCode.NormalSpinRequest;
            requestParam.PlayLine       = 1;
            requestParam.PlayBet        = _playbet;
            requestParam.IsExtraBet     = 0;
            requestParam.PlayDenom      = _playdenom;
            requestParam.MiniBet        = _playmini;
            requestParam.ReelPay        = 0;
            requestParam.ReelSelected   = new int[] { 0, 0, 0, 0, 0 };
            requestParam.ActionMode     = 0;

            RequestReqPacket spinRequest = new RequestReqPacket();
            spinRequest.req     = 2;
            spinRequest.vals    = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(requestParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", spinRequest.req, spinRequest.vals));

            await sendMessage(msg);
        }
        /// <summary>
        /// 일반스핀응답(131)
        /// </summary>
        /// <param name="obj"></param>
        protected virtual async Task receiveSpinResponse(JObject response)
        {
            response = removeNormalSpinCommonParams(response);
            bool isTriggerFG    = Convert.ToBoolean(response.Property("IsTriggerFG").Value);
            int nextModule      = Convert.ToInt32(response.Property("NextModule").Value);
            if (!isTriggerFG)
            {
                long totalWin = Convert.ToInt64(response.Property("TotalWin").Value);
                _cnt++;
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType   = 0;
                spinResponse.TotalWin   = totalWin;
                spinResponse.Response   = JsonConvert.SerializeObject(response);

                if ((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                await sendSpinCheckRequest();
            }
            else
            {
                if(nextModule == (int)NextModule.FreeStart)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinStartRequest();
                }
                else if(nextModule == (int)NextModule.FreeOption)
                {
                    _nowFreeSpin    = true;
                    _freeSpinStack  = new List<string>();
                    _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                    await sendFreeSpinOptionReqeust();
                }
            }
        }
        /// <summary>
        /// 일반스핀이끝난다음 결과확인요청(32)
        /// </summary>
        protected virtual async Task sendSpinCheckRequest()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.NormalSpinResultRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
             await sendMessage(msg);
        }
        /// <summary>
        /// 일반스핀끝난다음 결과응답(132)
        /// </summary>
        /// <param name="obj"></param>
        protected virtual async Task receiveSpinCheckResponse(JObject response)
        {
            _freeSpinStack      = new List<string>();
            _tembleSpinStack    = new List<string>();
            if (_cnt >= 100000 || _moneylimited)
            {
                _self.Tell(new RestartMessage());
            }
            else
                await sendSpinRequest();
        }
        /// <summary>
        /// 프리스핀시작요청(41)
        /// </summary>
        protected async Task sendFreeSpinStartRequest()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.FreeSpinStartRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);
        }
        /// <summary>
        /// 프리스핀시작응답(141)
        /// </summary>
        /// <param name="obj"></param>
        protected virtual async Task receiveFreeSpinStartResponse(JObject response)
        {
            try
            {
                if (!_nowFreeSpin)
                {
                    _logger.Warning("Free spin start response when not triggered free spin");
                    _self.Tell(new RestartMessage());
                    return;
                }
                response = removeCheckCommonParams(response);
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));
                await sendFreeSpinRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive freespin start response ex: {0}",ex);
                _self.Tell(new RestartMessage());
            }
        }
        /// <summary>
        /// 프리스핀요청(42)
        /// </summary>
        protected async Task sendFreeSpinRequest()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.FreeSpinRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);

        }
        /// <summary>
        /// 프리스핀응답(142)
        /// </summary>
        /// <param name="obj"></param>
        protected virtual async Task receiveFreeSpinResponse(JObject response)
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

                if (currentSpinTimes < awaredSpinTimes || currentRound < awardRound)
                    await sendFreeSpinRequest();
                else if(isRespin)
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
        /// <summary>
        /// 프리스핀결과요청(43)
        /// </summary>
        protected async Task  sendFreeSpinResultRequest()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.FreeSpinSumRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);
        }
        /// <summary>
        /// 프리스핀결과응답(143)
        /// </summary>
        /// <param name="obj"></param>
        protected virtual async Task receiveFreeSpinResultResponse(JObject response)
        {
            try
            {
                if (!_nowFreeSpin)
                {
                    _logger.Warning("Free spin sum response when not triggered free spin");
                    _self.Tell(new RestartMessage());
                    return;
                }
                response = removeCheckCommonParams(response);
                _freeSpinStack.Add(JsonConvert.SerializeObject(response));

                long totalWin = Convert.ToInt64(response.Property("TotalWinAmt").Value);
                SpinResponse spinResponse = new SpinResponse();
                spinResponse.SpinType       = 1;
                spinResponse.TotalWin       = totalWin; 
                spinResponse.Response       = string.Join("\n",_freeSpinStack);
                if((double)totalWin / (_playbet * _playmini) <= _maxOdd)
                    SpinDataQueue.Instance.insertSpinDataToQueue(spinResponse);
                
                int nextModule = (int)NextModule.Normal;
                if(!object.ReferenceEquals(response["NextModule"],null))
                    nextModule = Convert.ToInt32(response.Property("NextModule").Value);
                
                if(nextModule == (int)NextModule.FreeStart)
                    await sendFreeSpinStartRequest();
                else
                    await sendSpinCheckRequest();
            }
            catch (Exception ex)
            {
                _logger.Warning("exception is occured in receive free sum response ex: {0}",ex);
                _self.Tell(new RestartMessage());
            }
        }
        /// <summary>
        /// 프리스핀옵션창열림요청(44)
        /// </summary>
        protected async Task sendFreeSpinOptionReqeust()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.FreeSpinOptionRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);
        }
        /// <summary>
        /// 프리스핀옵션창열림응답(144)
        /// </summary>
        /// <param name="response"></param>
        protected virtual async Task receiveFreeSpinOptionResponse(JObject response)
        {

        }
        /// <summary>
        /// 프리스핀옵션선택요청(45)
        /// <param name="state">옵션갯수</param>
        /// <param name="index">선택된옵션인덱스</param>
        /// </summary>
        protected virtual async Task sendFreeSpinOptSelectRequest(int state,int index)
        {
            RequestFreeOptSelect optParam = new RequestFreeOptSelect();
            optParam.Type               = 3;
            optParam.ID                 = (int)MessageCode.FreeSpinOptSelectRequest;
            optParam.PlayerSelectState  = state;
            optParam.PlayerSelectIndex  = index;


            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(optParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);
        }
        /// <summary>
        /// 프리스핀옵션선택응답(145)
        /// </summary>
        /// <param name="response"></param>
        protected virtual async Task receiveFreeSpinOptSelectResponse(JObject response)
        {

        }
        /// <summary>
        /// 프리스핀옵션선택결과요청(46)
        /// </summary>
        protected async Task sendFreeSpinOptResultRequest()
        {
            RequestCheck checkParam = new RequestCheck();
            checkParam.Type = 3;
            checkParam.ID   = (int)MessageCode.FreeSpinOptResultRequest;

            RequestReqPacket checkRequest = new RequestReqPacket();
            checkRequest.req   = 2;
            checkRequest.vals  = string.Format("[1,\"{0}\"]", _Secure.doEncryptAndAddIV(JsonConvert.SerializeObject(checkParam)));
            string msg = addBracket(string.Format("\"req\":{0},\"vals\":{1}", checkRequest.req, checkRequest.vals));
            await sendMessage(msg);
        }
        /// <summary>
        /// 프리스핀옵션선택결과응답(146)
        /// </summary>
        protected virtual async Task receiveFreeSpinOptResultResponse(JObject response)
        {

        }

        /// <summary>
        /// 텀블게임요청보내기(33)
        /// </summary>
        protected virtual async Task sendTembleRequest()
        {
            
        }

        /// <summary>
        /// 텀블게임결과받을때(133)
        /// </summary>
        /// <param name="response"></param>
        protected virtual async Task receiveTembleResponse(JObject response)
        {

        }
        #endregion

    protected string combineURLParams(Dictionary<string, string> dicParams)
        {
            List<string> stringList = new List<string>();
            foreach (KeyValuePair<string, string> pair in dicParams)
            {
                stringList.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", stringList.ToArray());
        }
    }

    public class RestartMessage
    {

    }

    public class Spin10MemberRequest : MiniSpinRequest
    {
        public int      ReelPay         { get; set; }
        public int[]    ReelSelected    { get; set; }
        public int      ActionMode      { get; set; }
    }
    public class MiniSpinRequest
    {
        public int Type             { get; set; }
        public int ID               { get; set; }
        public int PlayLine         { get; set; }
        public int PlayBet          { get; set; }
        public int IsExtraBet       { get; set; }
        public int PlayDenom        { get; set; }
        public int MiniBet          { get; set; }
    }
    public class NormalTembleRequest
    {
        public int Type             { get; set; }
        public int ID               { get; set; }
        public int PlayLine         { get; set; }
        public int PlayBet          { get; set; }
        public int IsExtraBet       { get; set; }
        public int MiniBet          { get; set; }
        public int ReelPay          { get; set; }
        public int[] ReelSelected   { get; set; }
        public int ActionMode       { get; set; }
    }
    public class NormalTemble8Request
    {
        public int Type             { get; set; }
        public int ID               { get; set; }
        public int PlayLine         { get; set; }
        public int PlayDenom        { get; set; }
        public int PlayBet          { get; set; }
        public int IsExtraBet       { get; set; }
        public int MiniBet          { get; set; }
        public int[] ReelSelected   { get; set; }
        public int ActionMode       { get; set; }
    }
    public class Spin9MemberRequest : MiniSpinRequest
    {
        public int      ReelPay         { get; set; }
        public int[]    ReelSelected    { get; set; }
    }
    public class Spin8MemberRequest : MiniSpinRequest
    {
        public int ReelPay { get; set; }
    }
    public class Spin10TicketRequest : Spin8MemberRequest
    {
        public int[]    ReelSelected    { get; set; }
        public int      ActionMode      { get; set; }
        public string   Ticket          { get; set; }
    }
    public class Temble9TicketRequest
    {
        public int      Type            { get; set; }
        public int      ID              { get; set; }
        public int      PlayLine        { get; set; }
        public int      PlayBet         { get; set; }
        public int      IsExtraBet      { get; set; }
        public int      MiniBet         { get; set; }
        public int[]    ReelSelected    { get; set; }
        public int      ActionMode      { get; set; }
        public string   Ticket          { get; set; }
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