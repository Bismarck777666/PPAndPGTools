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
    public class SojuBombFetcher : SpinDataFetcher_1
    {
        public SojuBombFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
            : base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
        }

        protected async Task<string> sendPickRequest(HttpClient httpClient,string gameid,string gssid, int pickStep)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            KoiGateRequest request = new KoiGateRequest();
            dynamic PayLoadGame = new JObject();

            PayLoadGame["action"]           = "game";
            PayLoadGame["brandgameid"]      = _brandGameId;
            PayLoadGame["kn"]               = _gameSymbol;
            PayLoadGame["sessionid"]        = _sessionId;
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

            JObject PayLoadPortMessage = new JObject();
            PayLoadPortMessage["gameMode"]          = "pick";
            PayLoadPortMessage["SojuBomb_PickStep"] = pickStep;
            PayLoadPortMessage["pssid"]             = "";
            PayLoadPortMessage["gssid"]             = gssid;

            request.game    = PayLoadGame;
            request.grid    = Guid.NewGuid().ToString();
            request.header  = PayLoadHeader;
            request.portmessage = JsonConvert.SerializeObject(PayLoadPortMessage);

            string strRequest = JsonConvert.SerializeObject(request);
            HttpResponseMessage responseMessage = await httpClient.PostAsync(strURL, new StringContent(strRequest, Encoding.UTF8, "text/plain"));
            responseMessage.EnsureSuccessStatusCode();
            string strContent = await responseMessage.Content.ReadAsStringAsync();
            return strContent;
        }

        protected override async Task<List<SpinData>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList     = new List<SpinData>();
            string strResponse  = "";
            string gameid       = null;
            string gssid        = null;
            bool nowFreeSpin    = false;

            Habanero1GameState nextGameState    = Habanero1GameState.NormalSpin;
            int bombPickIndex                   = 0;
            try
            {
                do
                {
                    if (nextGameState == Habanero1GameState.Pick)
                        strResponse = await sendPickRequest(httpClient, gameid, gssid, bombPickIndex + 1);
                    else
                        strResponse = await sendSpinRequest(httpClient, gameid, gssid);

                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    gameid  = Convert.ToString(response["game"]["gameid"]);
                    gssid   = Convert.ToString(response["portmessage"]["gssid"]);

                    if(!object.ReferenceEquals(response["portmessage"]["nextgamestate"], null))
                    {
                        nextGameState = convertStringToGameState((string)response["portmessage"]["nextgamestate"]);
                    }
                    else
                    {
                        if (!object.ReferenceEquals(response["portmessage"]["SojuBomb_PickStep"], null))
                            bombPickIndex = Convert.ToInt32(response["portmessage"]["SojuBomb_PickStep"]);
                    }

                    SpinData spinData = new SpinData();

                    if ((string)response["portmessage"]["nextgamestate"] == "freegame")
                        nowFreeSpin = true;

                    string strSpinData = JsonConvert.SerializeObject(response["portmessage"]);
                    strResponseHistory.Add(strSpinData);
                    if (nextGameState == Habanero1GameState.NormalSpin)
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
