using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGSpinDBBuilder.PGFetcher
{
    class HoneyTrapOfDiaoChanFetcher : NewPGSpinDataFetcher
    {
        protected int[] _freeSpinCounts = new int[] { 110, 110, 0, 110, 0, 110, 10, 10, 0 };

        public HoneyTrapOfDiaoChanFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
        {

        }
        protected int selectMinFreeSpinType(int freeSpinGroup)
        {
            int minCount = -1;
            int minIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                if (minCount == -1 || minCount > _freeSpinCounts[3 * freeSpinGroup + i])
                {
                    minCount = _freeSpinCounts[3 * freeSpinGroup + i];
                    minIndex = i;
                }
            }
            return minIndex;
        }
        protected async Task<string> sendFreeSpinSelectRequest(HttpClient httpClient, int index)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/selectcharacter?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("s",  (index + 1).ToString()),
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
            });
            HttpResponseMessage message = await httpClient.PostAsync(strURL, postContent);
            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();
            return strContent;
        }
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";
            int spinType = 0;
            double startSpinWin = 0.0;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient);
                    JToken response = JToken.Parse(strResponse);
                    bool isBreak = true;
                    if (!IsNullOrEmpty(response["err"]))
                        return null;

                    int nextState = response["dt"]["si"]["nst"].ToObject<int>();
                    if (nextState != 1)
                        isBreak = false;

                    JObject siObj = response["dt"]["si"] as JObject;
                    _strLastID = siObj["sid"].ToString();
                    siObj.Remove("bl");
                    siObj.Remove("blab");
                    siObj.Remove("blb");
                    siObj.Remove("sid");
                    siObj.Remove("psid");
                    if (spinType >= 200)
                        siObj["aw"] = Math.Round(siObj["aw"].ToObject<double>() - startSpinWin, 2);

                    string strSpinData = siObj.ToString(Formatting.None);
                    strResponseHistory.Add(strSpinData);
                    if (nextState == 30)
                    {
                        int scatterCount  = (siObj["sp"] as JArray).Count;
                        int freeSpinGroup = scatterCount - 3;                        
                        int minId         = selectMinFreeSpinType(freeSpinGroup);
                        startSpinWin      = response["dt"]["si"]["aw"].ToObject<double>();
                        SpinResponse startResponse  = new SpinResponse();
                        startResponse.SpinType      = 100;
                        startResponse.TotalWin      = startSpinWin;
                        startResponse.Response      = string.Join("\n", strResponseHistory);
                        startResponse.FreeSpinType  = freeSpinGroup;
                        responseList.Add(startResponse);

                        strResponseHistory.Clear();
                        strResponse = await sendFreeSpinSelectRequest(httpClient, minId);
                        response    = JToken.Parse(strResponse);
                        if (!IsNullOrEmpty(response["err"]))
                            return null;

                        spinType    = 200 + (minId + 3 * freeSpinGroup);
                        siObj       = response["dt"]["si"] as JObject;
                        _strLastID  = siObj["sid"].ToString();
                        siObj.Remove("bl");
                        siObj.Remove("blab");
                        siObj.Remove("blb");
                        siObj.Remove("sid");
                        siObj.Remove("psid");
                        siObj["aw"] = Math.Round(siObj["aw"].ToObject<double>() - startSpinWin, 2);
                        strSpinData = siObj.ToString(Formatting.None);
                        strResponseHistory.Add(strSpinData);
                        continue;

                    }
                    if (isBreak)
                    {
                        double totalWin = response["dt"]["si"]["aw"].ToObject<double>();
                        SpinResponse spinResponse = new SpinResponse();
                        spinResponse.SpinType = spinType;
                        spinResponse.TotalWin = totalWin;
                        spinResponse.Response = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        if (spinType >= 200)
                            _freeSpinCounts[spinType - 200]++;

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
