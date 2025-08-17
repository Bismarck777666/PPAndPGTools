using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PlaysonDemobot
{
    public class PlaysonSpinDataFetcher : ISpinDataFetcher
    {
        protected string    _gameSymbol;
        protected int       _defaultBet;
        protected int       _lineCount;
        protected int       _realBet;
        protected string    _enginVersion;
        protected bool      _mustStop       = false;
        protected string    _proxyInfo      = null;
        protected string    _proxyUserID    = null;
        protected string    _proxyPassword  = null;
        protected bool      _isBuy          = false;

        protected string    _session        = "";
        protected long      _userBalance    = 0;
        protected string    _prnd           = "";

        public PlaysonSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,bool isBuy)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _isBuy          = isBuy;
        }

        public override async Task startFetch(string strGameSymbol, string strGameURL, double defaultC, double realBet,int lineCount, string strEnginVersion)
        {
            try
            {
                _gameSymbol     = strGameSymbol;
                _defaultBet     = (int)defaultC;
                _realBet        = (int)realBet;
                _lineCount      = lineCount;
                _enginVersion   = strEnginVersion;
                var httpClientHandler = new HttpClientHandler();

#if PROXY
                var proxy = new WebProxy
                {
                    Address                 = new Uri(string.Format("http://{0}", _proxyInfo)),
                    BypassProxyOnLocal      = false,
                    UseDefaultCredentials   = false,
                    Credentials             = new NetworkCredential(
                    userName: _proxyUserID,
                    password: _proxyPassword)
                };

                httpClientHandler.Proxy = proxy;
#endif

                do
                {
                    HttpClient httpClient       = new HttpClient(httpClientHandler);
                    httpClient.DefaultRequestHeaders.Add("Origin", "https://xplatform-demo-dgm.ps-gamespace.com");
                    httpClient.DefaultRequestHeaders.Add("Referer", "https://xplatform-demo-dgm.ps-gamespace.com/");

                    HttpResponseMessage message = await httpClient.GetAsync(strGameURL + strGameSymbol);
                    message.EnsureSuccessStatusCode();

                    string strContent = await message.Content.ReadAsStringAsync();
                    string strPattern = "window.playsonConfig = {";
                    int startIndex  = strContent.IndexOf(strPattern) + strPattern.Length - 1;
                    int endIndex    = strContent.IndexOf(";\r\n</script>", startIndex);
                    string strJson  = strContent.Substring(startIndex, endIndex - startIndex);

                    dynamic gameConfig = JsonConvert.DeserializeObject<dynamic>(strJson);
                    string server_url       = (string)gameConfig["partner"]["server_url"];
                    string build_date       = (string)gameConfig["date"];
                    string build_version    = (string)gameConfig["version"];
                    string project_id       = (string)gameConfig["partner"]["project_id"];

                    string serverEndPoint = string.Format("{0}/connect?gid={1}&r={2}&wlCode=demomode&projectId={3}", server_url, _gameSymbol, DateTimeOffset.Now.ToUnixTimeMilliseconds(), project_id);

                    await doConnect(httpClient, serverEndPoint,build_date,build_version);
                    if (string.IsNullOrEmpty(_session))
                        break;

                    serverEndPoint = string.Format("{0}/reconnect?gid={1}&r={2}&wlCode=demomode&projectId={3}", server_url, _gameSymbol, DateTimeOffset.Now.ToUnixTimeMilliseconds(), project_id);
                    await doReconnect(httpClient, serverEndPoint);

                    DoSpinsResults result = await doSpins(httpClient, serverEndPoint);
                    if (result == DoSpinsResults.NEEDRESTARTSESSION)
                    {
                        Console.WriteLine("restart session");
                        _prnd = "";
                        continue;
                    }
                    else
                    {
                        break;
                    }

                } while (true);
                Console.WriteLine("close session");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _session = string.Empty;
            }
        }

        protected async Task doConnect(HttpClient httpClient, string serverUrl,string buildDate,string buildVersion)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("show_rtp",       "true");
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "connect");
                root.SetAttribute("playerguid",     "TEST1100000");
                root.SetAttribute("gameid",         _gameSymbol);
                root.SetAttribute("platform",       "desk");
                root.SetAttribute("engine_type",    "unicore");
                root.SetAttribute("engine_version", _enginVersion);
                root.SetAttribute("wl",             "demomode");
                root.SetAttribute("snd",            "0");
                root.SetAttribute("spn",            "0");

                XmlElement debug = xml.CreateElement("debug");
                debug.SetAttribute("build_date",    buildDate);
                debug.SetAttribute("build_version", buildVersion);

                root.AppendChild(debug);
                xml.AppendChild(root);

                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse  = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(strResponse);

                XmlNode serverNode  = resultDoc.SelectSingleNode("/server");
                string session      = serverNode.Attributes["session"].Value;
                
                XmlNode userNode    = resultDoc.SelectSingleNode("/server/user_new");
                _userBalance        = Convert.ToInt64(userNode.Attributes["cash"].Value);

                _session = session;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _session = string.Empty;
            }
        }

        protected async Task doStart(HttpClient httpClient,string serverUrl)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session",        _session);
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "start");
                root.SetAttribute("show_rtp",       "true");

                XmlElement game = xml.CreateElement("game");
                game.SetAttribute("id",             _gameSymbol);

                root.AppendChild(game);
                xml.AppendChild(root);

                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse  = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(strResponse);

                XmlNode serverNode = null;
                foreach(XmlNode childNode in resultDoc.ChildNodes)
                {
                    if(childNode.Name == "server")
                    {
                        serverNode = childNode;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _session = string.Empty;
            }
        }

        protected async Task doReconnect(HttpClient httpClient,string serverUrl)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session",        _session);
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "reconnect");
                root.SetAttribute("show_rtp",       "true");

                xml.AppendChild(root);

                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse  = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(strResponse);

                XmlNode serverNode = null;
                foreach(XmlNode childNode in resultDoc.ChildNodes)
                {
                    if(childNode.Name == "server")
                    {
                        serverNode = childNode;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _session = string.Empty;
            }
        }

        protected virtual async Task<DoSpinsResults> doSpins(HttpClient httpClient, string serverUrl)
        {
            int count = 0;
            do
            {
                List<SpinData> responses = await doSpin(httpClient, serverUrl);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                count++;

                await Task.Delay(500);
                if (count >= 5000)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }

        protected virtual async Task<List<SpinData>> doSpin(HttpClient httpClient, string serverUrl)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList     = new List<SpinData>();
            string  strResponse     = "";
            string  nextAction      = null;
            int     spinType        = 0;
            double  spinWin         = 0.0;
            bool    nowFreeSpin     = false;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient, serverUrl, nextAction);

                    XmlDocument resultDoc = new XmlDocument();
                    resultDoc.LoadXml(strResponse);

                    XmlNode serverNode  = resultDoc.SelectSingleNode("/server");
                    XmlNode stateNode   = resultDoc.SelectSingleNode("/server/state");
                    string state        = stateNode.Attributes["current_state"].Value;
                    
                    if(resultDoc.SelectSingleNode("/server/user_new") != null)
                    {
                        XmlNode userNode = resultDoc.SelectSingleNode("/server/user_new");
                        _userBalance = Convert.ToInt64(userNode.Attributes["cash"].Value);
                        serverNode.RemoveChild(userNode);
                    }

                    if (state == "fs")
                    {
                        nowFreeSpin = true;
                    }

                    SpinData spinData = new SpinData();
                    strResponseHistory.Add(resultDoc.InnerXml);

                    XmlNode gameNode = resultDoc.SelectSingleNode("/server/game");
                    spinWin += Convert.ToDouble(gameNode.Attributes["cash-win"].Value);

                    if (state == "idle")
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = nowFreeSpin ? 1 : 0;
                        spinResponse.SpinOdd    = spinWin / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        nowFreeSpin = false;
                        return responseList;
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                _session = string.Empty;
            }
            return null;
        }

        protected virtual async Task<string> sendSpinRequest(HttpClient httpClient, string serverUrl, string nextAction, bool isBuy = false)
        {

            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session",        _session);
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "bet");

                XmlElement bet      = xml.CreateElement("bet");
                bet.SetAttribute("cash",            _defaultBet.ToString());

                XmlElement bonusBuy = xml.CreateElement("buy");
                bonusBuy.SetAttribute("freegame", "true");

                XmlElement debug    = xml.CreateElement("debug");
                debug.SetAttribute("user_cash",     (_userBalance - _realBet).ToString());
                debug.SetAttribute("bet_cash",      _realBet.ToString());

                root.AppendChild(bet);
                if(_isBuy)
                    root.AppendChild(bonusBuy);
                root.AppendChild(debug);
                xml.AppendChild(root);

                serverUrl = serverUrl.Replace("/reconnect?",    "/bet?");
                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse  = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                return strResponse;
            }
            catch (Exception ex)
            {
                _session = string.Empty;
                Console.WriteLine(ex);
                return string.Empty;
            }
        }
    }

    public enum DoSpinsResults
    {
        USERSTOPPED         = 0,
        NEEDRESTARTSESSION  = 1,
    }

    public class SpinResponse
    {
        public double   TotalWin    { get; set; }
        public double   RealWin     { get; set; }
        public string   NextAction  { get; set; }
        public int      SpinType    { get; set; }
        public string   Response    { get; set; }
        public int      FreeSpinType { get; set; }

    }
}
