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
    public class LaughingBuddhaFetcher : OptionalSpinDataFetcher_1
    {
        public LaughingBuddhaFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,string strBrandGameId, string strClientVersion, double realBet,int lineCount,int betLevel,double stake,int coin,IList<int> freeOptions):
            base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin,freeOptions)
        {
            _freeSpinOptions = freeOptions;
        }
        protected override async Task<string> sendPickRequest(HttpClient httpClient,string gameid,string gssid)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            KoiGateRequest request = new KoiGateRequest();
            dynamic PayLoadGame = new JObject();

            PayLoadGame["action"]           = "game";
            PayLoadGame["brandgameid"]      = _brandGameId;
            PayLoadGame["kn"]               = _gameSymbol;
            PayLoadGame["sessionid"]        = _sessionId;
            PayLoadGame["gameid"]           = gameid;
            _freeSpinOption = _freeSpinOptions[_random.Next(0, _freeSpinOptions.Count)];

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
            PayLoadPortMessage["pickIdIndex"]   = _freeSpinOption;
            PayLoadPortMessage["gameMode"]      = "pick";
            PayLoadPortMessage["pssid"]         = "";
            PayLoadPortMessage["gssid"]         = gssid;

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

        protected override async Task<List<SpinData>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList     = new List<SpinData>();
            string strResponse      = "";
            int spinType            = 0;
            string gameid           = null;
            string gssid            = null;
            double prevOdd          = 0;
            int selectedSymbolId    = -1;

            Habanero1GameState nextGameState = Habanero1GameState.NormalSpin;

            try
            {
                do
                {
                    if (nextGameState == Habanero1GameState.PickSymbol)
                        strResponse = await sendPickRequest(httpClient, gameid, gssid);
                    else
                        strResponse = await sendSpinRequest(httpClient, gameid, gssid);
                    
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    gameid          = Convert.ToString(response["game"]["gameid"]);
                    
                    if(!object.ReferenceEquals(response["portmessage"]["gssid"],null))
                        gssid = Convert.ToString(response["portmessage"]["gssid"]);
                    
                    nextGameState   = convertStringToGameState((string)response["portmessage"]["nextgamestate"]);
                    SpinData spinData = new SpinData();
                    if (nextGameState == Habanero1GameState.PickSymbol)
                        spinType = 100;
                    else if (nextGameState == Habanero1GameState.NormalSpin)
                        spinType = 0;

                    string strSpinData = JsonConvert.SerializeObject(response["portmessage"]);
                    strResponseHistory.Add(strSpinData);
                    if(nextGameState == Habanero1GameState.PickSymbol)
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = spinType;
                        spinResponse.SpinOdd    = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        strResponseHistory.Clear();
                        prevOdd = spinResponse.SpinOdd;
                    }
                    else if (nextGameState == Habanero1GameState.FreeSpin)
                    {
                        //그냥스핀을 돌리면된다
                        if (!object.ReferenceEquals(response["portmessage"]["pickId"], null))
                            selectedSymbolId = (int)response["portmessage"]["pickId"];
                    }
                    else
                    {
                        //Habanero1GameState.NormalSpin
                        bool isFreeLastGame = !object.ReferenceEquals(response["portmessage"]["numfreegames"], null); 
                        gameid  = null;
                        gssid   = null;
                        SpinData spinResponse = new SpinData();

                        if(isFreeLastGame)
                            spinResponse.SpinType   = 200 + selectedSymbolId;
                        else
                            spinResponse.SpinType = spinType;

                        spinResponse.SpinOdd    = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet - prevOdd;
                        spinResponse.RealOdd    = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet - prevOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);

                        responseList[0].SpinOdd = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet;
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
