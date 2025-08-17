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
    public class ColossalGemsFetcher : SpinDataFetcher
    {
        public ColossalGemsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strBrandGameId, string strClientVersion, double realBet, int lineCount, int betLevel, double stake, int coin)
            : base(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, strClientVersion, realBet, lineCount, betLevel, stake, coin)
        {
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
                    strResponse = await sendSpinRequest(httpClient,gameid,gssid);
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(strResponse);
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
