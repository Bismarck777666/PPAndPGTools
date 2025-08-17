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
    public class FiveMariachisFetcher : SpinDataFetcher
    {
        public FiveMariachisFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
            : base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
        }

        protected virtual async Task<string> sendPickRequest(HttpClient httpClient,string gameid,string gssid)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            HabaneroRequest request = new HabaneroRequest();
            dynamic PayLoadGame = new JObject();

            PayLoadGame["action"]           = "pickgamedonepay";
            PayLoadGame["brandgameid"]      = _brandGameId;
            PayLoadGame["kn"]               = _gameSymbol;
            PayLoadGame["sessionid"]        = _sessionId;
            if(gameid != null)
                PayLoadGame["gameid"]           = gameid;
            
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

        protected override async Task<List<SpinData>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList = new List<SpinData>();
            string strResponse  = "";
            string gameid       = null;
            string gssid        = null;
            int spinType        = 0;
            HabaneroGameMode nextActionMode = HabaneroGameMode.Main;
            try
            {
                do
                {
                    if (nextActionMode == HabaneroGameMode.Pick)
                        strResponse = await sendPickRequest(httpClient, gameid, gssid);
                    else
                        strResponse = await sendSpinRequest(httpClient, gameid, gssid);
                    
                    //strResponse = await sendSpinRequest(httpClient,gameid,gssid);
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
}
