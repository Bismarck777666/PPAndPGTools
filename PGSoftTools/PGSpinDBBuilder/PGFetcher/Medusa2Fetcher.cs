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
    class Medusa2Fetcher : NewPGSpinDataFetcher
    {
        public Medusa2Fetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
        {

        }

        protected async Task<string> sendSelectPotRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin/SelectPot?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk",  "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
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
            string strResponse  = "";
            int spinType        = 0;
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

                    if (nextState == 3)
                    {
                        strResponse = await sendSelectPotRequest(httpClient);
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

        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient)
        {
            int count = 0;
            List<SpinResponse> responseList = new List<SpinResponse>();
            do
            {
                List<SpinResponse> responses = await doSpin(httpClient);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                string[] strResponses = responses[0].Response.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                JToken   responseData = JToken.Parse(strResponses[0]);
                if ((int) responseData["ms"] == 1 && responseList.Count > 0)
                {
                    GroupSpinResponse groupResponse = new GroupSpinResponse();
                    groupResponse.TotalWin          = 0.0;
                    groupResponse.ResponseList      = new List<SpinResponse>();
                    List<string> responseStrings = new List<string>();
                    for (int i = 0; i < responseList.Count; i++)
                    {
                        groupResponse.TotalWin += responseList[i].TotalWin;
                        responseStrings.Add(responseList[i].Response);
                        groupResponse.ResponseList.Add(responseList[i]);
                    }
                    groupResponse.Response = string.Join("####", responseStrings.ToArray());
                    SpinDataQueue.Instance.addSpinDataToQueue(groupResponse);
                    responseList.Clear();
                }

                responseList.Add(responses[0]);
                count++;
                await Task.Delay(500);
                if (count >= 1000)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }

    }
}
