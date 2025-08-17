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
    public class HeySushiFetcher : SpinDataFetcher_1
    {
        public HeySushiFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
            : base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
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

                    SpinData spinData = new SpinData();

                    if ((string)response["portmessage"]["nextgamestate"] == "freegame")
                        nowFreeSpin = true;

                    string strSpinData = JsonConvert.SerializeObject(response["portmessage"]);
                    strResponseHistory.Add(strSpinData);
                    if (Convert.ToBoolean(response["portmessage"]["isgamedone"]))
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType = nowFreeSpin ? 1 : 0;
                        spinResponse.SpinOdd = Convert.ToDouble(response["portmessage"]["totalwincash"]) / _realBet;
                        spinResponse.RealOdd = spinResponse.SpinOdd;
                        spinResponse.Response = string.Join("\n", strResponseHistory);
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
