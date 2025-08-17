using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;

namespace HabaneroDemoBot
{
    public class SpinDataFetcher : ISpinDataFetcher
    {
        protected string        _brandGameId    = null;
        protected string        _gameSymbol     = null;
        protected int           _lineCount;
        protected int           _betLevel;
        protected double        _realBet;
        protected double        _stake;
        protected int           _coin;
        protected bool          _mustStop       = false;
        protected string        _proxyInfo      = null;
        protected string        _proxyUserID    = null;
        protected string        _proxyPassword  = null;
        protected string        _clientVersion  = null;
        protected string        _dtld           = null;

        
        protected string        _sessionId      = null;
        protected string        _sstoken        = null;


        public SpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,string strBrandGameId, string strClientVersion, double realBet,int lineCount,int betLevel,double stake,int coin)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _brandGameId    = strBrandGameId;
            _clientVersion  = strClientVersion;
            _realBet        = realBet;
            _lineCount      = lineCount;
            _stake          = stake;
            _coin           = coin;
            _betLevel       = betLevel;

        }
        public override async Task startFetch(string strGameSymbol, string strGameURL, double defaultC, int lineCount)
        {
            try
            {
#if Proxy
                var proxy = new WebProxy
                {
                    Address                 = new Uri(string.Format("http://{0}", _proxyInfo)),
                    BypassProxyOnLocal      = false,
                    UseDefaultCredentials   = false,
                    Credentials             = new NetworkCredential(
                        userName    : _proxyUserID,
                        password    : _proxyPassword)
                };
#endif

                var httpClientHandler   = new HttpClientHandler();

#if Proxy
                httpClientHandler.Proxy = proxy;
#endif
                do
                {
                    _gameSymbol = strGameSymbol;
                    HttpClient httpClient       = new HttpClient(httpClientHandler);
                    HttpResponseMessage message = await httpClient.GetAsync(strGameURL + "&brandgameid=" + _brandGameId);
                    message.EnsureSuccessStatusCode();

                    string strContent = await message.Content.ReadAsStringAsync();
                    string strPattern = "window._settings=JSON.parse(\"";
                    //int startIndex    = strContent.IndexOf(strPattern) + strPattern.Length;
                    //int endIndex      = strContent.IndexOf("\")", startIndex);
                    //string strJson    = strContent.Substring(startIndex, endIndex - startIndex);

                    strPattern  = "DtLoaded\\\":";
                    int startIndex = strContent.IndexOf(strPattern) + strPattern.Length;
                    int endIndex = strContent.IndexOf(",", startIndex);
                    string strDtLoaded = strContent.Substring(startIndex, endIndex - startIndex);
                    _dtld = strDtLoaded;

                    bool initResult = await doInit(httpClient, _clientVersion, _brandGameId, strGameSymbol, strDtLoaded);
                    if (!initResult)
                    {
                        Console.WriteLine("restart session from init{0}", _proxyInfo);
                        await Task.Delay(1000);
                        continue;
                    }


                    DoSpinsResults result = await doSpins(httpClient);
                    if (result == DoSpinsResults.NEEDRESTARTSESSION)
                    {
                        Console.WriteLine("restart session");
                        await Task.Delay(1000);
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
            }
        }

        protected virtual async Task<bool> doInit(HttpClient httpClient,string clientVersion,string brandgameid,string kn,string dtld)
        {
            try
            {
                string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", clientVersion);

                HabaneroRequest request = new HabaneroRequest();
                dynamic PayLoadGame = new JObject();
                PayLoadGame["action"]       = "init";
                PayLoadGame["brandgameid"]  = brandgameid;
                PayLoadGame["kn"]           = kn;
                PayLoadGame["sessionid"]    = null;

                dynamic PayLoadHeader       = new JObject();
                dynamic PayLoadHeaderPlayer = new JObject();
                PayLoadHeaderPlayer["brandid"]  = "a9d542ac-c9bb-e311-93f6-80c16e0883f4";
                PayLoadHeaderPlayer["dtld"]     = dtld;
                PayLoadHeaderPlayer["funmode"]  = true;
                PayLoadHeaderPlayer["lc"]       = "en";
                PayLoadHeaderPlayer["mobile"]   = false;
                PayLoadHeaderPlayer["sfb"]      = null;
                PayLoadHeaderPlayer["sstoken"]  = "";
                
                PayLoadHeader["player"]     = PayLoadHeaderPlayer;
                PayLoadHeader["version"]    = clientVersion;

                request.game    = PayLoadGame;
                request.grid    = Guid.NewGuid().ToString();
                request.header  = PayLoadHeader;

                string strRequest = JsonConvert.SerializeObject(request);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(strURL, new StringContent(strRequest, Encoding.UTF8, "text/plain"));
                string strResponse = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
                if(object.ReferenceEquals(response["game"], null) || object.ReferenceEquals(response["game"]["sessionid"], null))
                    return false;
                _sessionId = Convert.ToString(response["game"]["sessionid"]);

                if (object.ReferenceEquals(response["header"], null) || object.ReferenceEquals(response["header"]["player"], null) || object.ReferenceEquals(response["header"]["player"]["ssotoken"], null))
                    return false;
                _sstoken = Convert.ToString(response["header"]["player"]["ssotoken"]);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected virtual async Task<DoSpinsResults> doSpins(HttpClient httpClient)
        {
            int count = 0;
            do
            {
                List<SpinData> responses = await doSpin(httpClient);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                count++;

                //await Task.Delay(300);
                if (count >= 1000)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }

        protected virtual async Task<string> sendSpinRequest(HttpClient httpClient,string gameid,string gssid)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            HabaneroRequest request = new HabaneroRequest();
            dynamic PayLoadGame = new JObject();

            PayLoadGame["action"]       = "spin";
            PayLoadGame["brandgameid"]  = _brandGameId;
            PayLoadGame["kn"]           = _gameSymbol;
            PayLoadGame["sessionid"]    = _sessionId;
            if(!string.IsNullOrEmpty(gameid))
                PayLoadGame["gameid"] = gameid;
            PayLoadGame["play"]         = new JObject();
            PayLoadGame["play"]["betstate"] = new JObject();

            JObject betItem = new JObject();
            betItem["betlevel"]         = _betLevel;
            betItem["coins"]            = _coin;
            betItem["numpaylines"]      = _lineCount;
            betItem["stake"]            = _stake;
            JArray betArray = new JArray();
            betArray.Add(betItem);

            PayLoadGame["play"]["betstate"]["bet"] =  betArray;


            dynamic PayLoadHeader       = new JObject();
            
            dynamic PayLoadHeaderPlayer     = new JObject();
            PayLoadHeaderPlayer["brandid"]  = "a9d542ac-c9bb-e311-93f6-80c16e0883f4";
            PayLoadHeaderPlayer["funmode"]  = true;
            PayLoadHeaderPlayer["lc"]       = "en";
            PayLoadHeaderPlayer["mobile"]   = false;
            PayLoadHeaderPlayer["ssotoken"] = _sstoken;
                
            PayLoadHeader["player"]     = PayLoadHeaderPlayer;
            PayLoadHeader["version"]    = _clientVersion;

            request.game    = PayLoadGame;
            request.grid    = Guid.NewGuid().ToString();
            request.header  = PayLoadHeader;

            string strRequest = JsonConvert.SerializeObject(request);
            HttpResponseMessage responseMessage = await httpClient.PostAsync(strURL, new StringContent(strRequest, Encoding.UTF8, "text/plain"));
            responseMessage.EnsureSuccessStatusCode();
            string strContent = await responseMessage.Content.ReadAsStringAsync();
            return strContent;
        }

        protected virtual async Task<List<SpinData>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList = new List<SpinData>();
            string strResponse  = "";
            int spinType        = 0;
            string gameid       = null;
            string gssid        = null;
            HabaneroGameMode nextActionMode = HabaneroGameMode.Main;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient,gameid,gssid);
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);

                    if (!object.ReferenceEquals(response["game"]["gameid"], null))
                        gameid = Convert.ToString(response["game"]["gameid"]);

                    nextActionMode = (HabaneroGameMode)Convert.ToInt32(response["game"]["play"]["videoslotstate"]["gamemodeid"]);
                    SpinData spinData = new SpinData();

                    if ((string)response["game"]["play"]["videoslotstate"]["gamemodename"] == "freegame")
                        spinType = 1;

                    string strSpinData = JsonConvert.SerializeObject(response["game"]["play"]);
                    strResponseHistory.Add(strSpinData);
                    if (Convert.ToInt32(response["game"]["play"]["videoslotstate"]["gamemodeid"]) == 1)
                    {
                        gameid  = null;
                        gssid   = null;
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = spinType;
                        spinResponse.SpinOdd    = Convert.ToDouble(response["game"]["play"]["totalpayout"]) / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        return responseList;
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
            }
            return null;
        }
    }

    public class HabaneroRequest
    {
        public dynamic  game    { get; set; }
        public string   grid    { get; set; }
        public dynamic  header  { get; set; }
    }

    public enum DoSpinsResults
    {
        USERSTOPPED         = 0,
        NEEDRESTARTSESSION  = 1,
    }

    /// <summary>
    /// 다음 액션정보
    /// </summary>
    public enum HabaneroGameMode
    {
        Main        = 1,
        Pick        = 3,
        FreeGame    = 4,
        Bonus1      = 18,//옵션선택1
        Bonus2      = 19,
        Bonus3      = 20,
        Bonus4      = 21,
        Bonus5      = 22,//옵션선택5
        Respin      = 23,
    }
    /// <summary>
    /// 현재끝난액션정보
    /// </summary>
    public enum HabaneroLastSlotEventType
    {
        NormalSpin      = 1,
        BonusSpin       = 2,
        TriggerBonus    = 3,
        SwitchToNormal  = 5,
        PickGameDone    = 6,
    }
}
