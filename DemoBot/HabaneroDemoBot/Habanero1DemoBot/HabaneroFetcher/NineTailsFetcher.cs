using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HabaneroDemoBot
{
    public class NineTailsFetcher : SpinDataFetcher_1
    {
        protected string _pssid = "";
        public NineTailsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
            : base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
        }

        protected override async Task<string> sendSpinRequest(HttpClient httpClient, string gameid,string gssid)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            KoiGateRequest request = new KoiGateRequest();
            
            dynamic PayLoadGame = new JObject();
            PayLoadGame["action"]       = "game";
            PayLoadGame["brandgameid"]  = _brandGameId;
            PayLoadGame["kn"]           = _gameSymbol;
            PayLoadGame["sessionid"]    = _sessionId;
            if(!string.IsNullOrEmpty(gameid))
                PayLoadGame["gameid"] = gameid;
            
            dynamic PayLoadHeader           = new JObject();
            dynamic PayLoadHeaderPlayer     = new JObject();
            PayLoadHeaderPlayer["brandid"]  = "a9d542ac-c9bb-e311-93f6-80c16e0883f4";
            PayLoadHeaderPlayer["funmode"]  = true;
            PayLoadHeaderPlayer["lc"]       = "en";
            PayLoadHeaderPlayer["mobile"]   = false;
            PayLoadHeaderPlayer["ssotoken"] = _sstoken;
                
            PayLoadHeader["player"]     = PayLoadHeaderPlayer;
            PayLoadHeader["version"]    = _clientVersion;

            JObject PayLoadPortMessage = new JObject();
            if (string.IsNullOrEmpty(gameid))
            {
                PayLoadPortMessage["betLevel"]     = _betLevel;
                PayLoadPortMessage["numLines"]     = _lineCount;
                PayLoadPortMessage["coinValue"]    = _stake;
                PayLoadPortMessage["pssid"]        = _pssid;
                PayLoadPortMessage["gameMode"]     = "base";
            }
            else
            {
                PayLoadPortMessage["gssid"]        = gssid;
                PayLoadPortMessage["pssid"]        = _pssid;
                PayLoadPortMessage["gameMode"]     = "free";
            }

            request.game        = PayLoadGame;
            request.grid        = Guid.NewGuid().ToString();
            request.header      = PayLoadHeader;
            request.portmessage = JsonConvert.SerializeObject(PayLoadPortMessage);

            string strRequest = JsonConvert.SerializeObject(request);
            HttpResponseMessage responseMessage = await httpClient.PostAsync(strURL, new StringContent(strRequest, Encoding.UTF8, "text/plain"));
            responseMessage.EnsureSuccessStatusCode();
            string strContent = await responseMessage.Content.ReadAsStringAsync();
            return strContent;
        }

        protected override async Task<bool> doInit(HttpClient httpClient,string clientVersion,string brandgameid,string kn,string dtld)
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

                if (object.ReferenceEquals(response["portmessage"], null) || object.ReferenceEquals(response["portmessage"]["pssid"], null))
                    return false;
                _pssid = Convert.ToString(response["portmessage"]["pssid"]);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected override async Task<List<SpinData>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList     = new List<SpinData>();
            string strResponse  = "";
            string gameid       = null;
            string gssid        = null;
            bool nowFreeSpin    = false;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient, gameid, gssid);
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    gameid  = Convert.ToString(response["game"]["gameid"]);
                    gssid   = Convert.ToString(response["portmessage"]["gssid"]);
                    if (!object.ReferenceEquals(response["portmessage"]["pssid"], null))
                    {
                        _pssid = Convert.ToString(response["portmessage"]["pssid"]);
                    }

                    SpinData spinData = new SpinData();

                    if (!object.ReferenceEquals(response["portmessage"]["featuretriggered"],null) && (bool)response["portmessage"]["featuretriggered"])
                        nowFreeSpin = true;

                    string strSpinData = JsonConvert.SerializeObject(response["portmessage"]);
                    strResponseHistory.Add(strSpinData);
                    if ((bool)response["portmessage"]["isgamedone"])
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = nowFreeSpin ? 1 : 0;
                        spinResponse.SpinOdd    = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        gameid      = null;
                        gssid       = null;
                        nowFreeSpin = false;
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
}
