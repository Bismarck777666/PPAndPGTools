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
    public class OptionalSpinDataFetcher : SpinDataFetcher
    {
        protected IList<int>    _freeSpinOptions    = null;
        protected int           _freeSpinOption     = 0;
        protected Random        _random             = new Random((int) DateTime.Now.Ticks);
        public OptionalSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,string strBrandGameId, string strClientVersion, double realBet,int lineCount,int betLevel,double stake,int coin,IList<int> freeOptions):
            base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
            _freeSpinOptions = freeOptions;
        }
        protected virtual async Task<string> sendPickRequest(HttpClient httpClient,string gameid,string gssid)
        {
            string strURL = string.Format("{0}{1}","https://gi-test.game-loader.com/pf?client=", _clientVersion);

            HabaneroRequest request = new HabaneroRequest();
            dynamic PayLoadGame = new JObject();

            PayLoadGame["action"]           = "client";
            PayLoadGame["brandgameid"]      = _brandGameId;
            PayLoadGame["kn"]               = _gameSymbol;
            PayLoadGame["sessionid"]        = _sessionId;
            PayLoadGame["gameid"]           = gameid;
            _freeSpinOption = _freeSpinOptions[_random.Next(0, _freeSpinOptions.Count)];
            PayLoadGame["clientpickdata"]   = _freeSpinOption;

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
            List<SpinData> responseList     = new List<SpinData>();
            string strResponse      = "";
            int spinType            = 0;
            string gameid           = null;
            string gssid            = null;
            double prevOdd          = 0;
            //HabaneroGameMode            nextActionMode  = HabaneroGameMode.Main;
            HabaneroLastSlotEventType   lastEventType   = HabaneroLastSlotEventType.NormalSpin;
            try
            {
                do
                {
                    if (lastEventType == HabaneroLastSlotEventType.TriggerBonus)
                        strResponse = await sendPickRequest(httpClient, gameid, gssid);
                    else
                        strResponse = await sendSpinRequest(httpClient, gameid, gssid);
                    
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    gameid          = Convert.ToString(response["game"]["gameid"]);
                    //nextActionMode  = (HabaneroGameMode)Convert.ToInt32(response["game"]["play"]["videoslotstate"]["gamemodeid"]);
                    lastEventType   = (HabaneroLastSlotEventType)Convert.ToInt32(response["game"]["play"]["videoslotstate"]["lastsloteventtypeid"]);

                    SpinData spinData = new SpinData();
                    if (lastEventType == HabaneroLastSlotEventType.TriggerBonus)
                        spinType = 100;
                    else if (lastEventType == HabaneroLastSlotEventType.NormalSpin)
                        spinType = 0;

                    string strSpinData = JsonConvert.SerializeObject(response["game"]["play"]);
                    strResponseHistory.Add(strSpinData);
                    if(lastEventType == HabaneroLastSlotEventType.TriggerBonus)
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = spinType;
                        spinResponse.SpinOdd    = Convert.ToDouble(response["game"]["play"]["totalpayout"]) / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        strResponseHistory.Clear();
                        prevOdd = spinResponse.SpinOdd;
                    }
                    else if (lastEventType == HabaneroLastSlotEventType.BonusSpin || lastEventType == HabaneroLastSlotEventType.PickGameDone)
                    {
                        //그냥스핀을 돌리면된다
                    }
                    else if(lastEventType == HabaneroLastSlotEventType.SwitchToNormal)
                    {
                        gameid  = null;
                        gssid   = null;
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = 200 + _freeSpinOption;
                        spinResponse.SpinOdd    = Convert.ToDouble(response["game"]["play"]["totalpayout"]) / _realBet - prevOdd;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        responseList.First().SpinOdd    = Convert.ToDouble(response["game"]["play"]["totalpayout"]) / _realBet;
                        responseList.First().RealOdd    = prevOdd;
                        return responseList;
                    }
                    else   //NormalSpin : 1
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
