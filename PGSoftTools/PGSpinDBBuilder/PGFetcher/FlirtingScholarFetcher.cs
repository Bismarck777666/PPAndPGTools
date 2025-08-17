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
    class FlirtingScholarFetcher : NewPGSpinDataFetcher
    {
        public FlirtingScholarFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
        {

        }

        protected async Task<string> sendFreeSpinOptionSelect(HttpClient httpClient, int ind)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
                new KeyValuePair<string, string>("fp",  ind.ToString()),

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
            string  strResponse = "";
            int     spinType    = 0;
            int     optionIndex = 0;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient);
                    JToken response = JToken.Parse(strResponse);
                    bool isBreak = true;
                    if (!IsNullOrEmpty(response["err"]))
                    {
                        return null;
                    }
                    if (!IsNullOrEmpty(response["dt"]["si"]["fs"]))
                    {
                        if (spinType == 0)
                            spinType = 1;
                    }
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

                    string strSpinData = siObj.ToString(Formatting.None);
                    strResponseHistory.Add(strSpinData);

                    if (nextState == 51)
                    {
                        strResponse = await sendFreeSpinOptionSelect(httpClient, optionIndex);
                        response = JToken.Parse(strResponse);
                        if (!IsNullOrEmpty(response["err"]))
                            return null;

                        siObj = response["dt"]["si"] as JObject;
                        _strLastID = siObj["sid"].ToString();
                        siObj.Remove("bl");
                        siObj.Remove("blab");
                        siObj.Remove("blb");
                        siObj.Remove("sid");
                        siObj.Remove("psid");
                        strSpinData = siObj.ToString(Formatting.None);
                        strResponseHistory.Add(strSpinData);
                        optionIndex++;
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
