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
    public class TabernaDeLosMuertosUltraFetcher : SpinDataFetcher_1
    {
        protected string _pssid = "";
        public TabernaDeLosMuertosUltraFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
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
                PayLoadPortMessage["betLevel"]      = _betLevel;
                PayLoadPortMessage["numLines"]      = _lineCount;
                PayLoadPortMessage["coinValue"]     = _stake;
                PayLoadPortMessage["pssid"]         = _pssid;
                PayLoadPortMessage["gameMode"]      = "base";
                PayLoadPortMessage["betMode"]       = "ultra";
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
    }
}
