using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PGSpinDBBuilder
{
    class CaishenWinsFetcher : PGSpinDataFetcher
    {
        protected int[] _freeSpinCounts = new int[7];
        public CaishenWinsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
                    base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
        {

        }

        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient)
        {
            int count = 0;
            do
            {
                List<SpinResponse> responses = await doSpin(httpClient);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                count++;

                await Task.Delay(250);
                if (count >= 10000)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }

        private int minCountFreeSpinType(int startType)
        {
            int minType = startType;
            for(int i = startType; i < 7; i++)
            {
                if (_freeSpinCounts[i] < _freeSpinCounts[minType])
                    minType = i;
            }
            return minType;
        }
        protected async Task<string> sendGambleRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("ig",  "true"),
                new KeyValuePair<string, string>("gt",  "1"),                
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
            });
            HttpResponseMessage message = await httpClient.PostAsync(strURL, postContent);
            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();
            return strContent;
        }

        protected async Task<string> sendGambleEndRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("ig",  "false"),
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
            });
            HttpResponseMessage message = await httpClient.PostAsync(strURL, postContent);
            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();
            return strContent;
        }

        private string getSimplifiedResponse(JToken response)
        {
            JObject siObj = response["dt"]["si"] as JObject;
            _strLastID = siObj["sid"].ToString();
            siObj.Remove("bl");
            siObj.Remove("blab");
            siObj.Remove("blb");
            siObj.Remove("sid");
            siObj.Remove("psid");

            string strSpinData = siObj.ToString(Formatting.None);
            return strSpinData;
        }
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";
            double startSpinWin = 0.0;
            int spinType = 0;
            try
            {
                bool isFirstFreeSpin = true;
                do
                {
                    strResponse = await sendSpinRequest(httpClient);
                    JToken response = JToken.Parse(strResponse);
                    bool isBreak = true;
                    if (!IsNullOrEmpty(response["err"]))
                        return null;

                    if (!IsNullOrEmpty(response["dt"]["si"]["fs"]))
                    {
                        int remainSpin = response["dt"]["si"]["fs"]["s"].ToObject<int>();
                        if (remainSpin > 0)
                        {
                            if(isFirstFreeSpin)
                            {
                                int currentFreeSpinType     = remainSpin / 2 - 4;
                                int minFreeSpinType         = minCountFreeSpinType(currentFreeSpinType);
                                strResponseHistory.Add(getSimplifiedResponse(response));
                                if (currentFreeSpinType != minFreeSpinType)
                                {
                                    for (int i = 0; i < minFreeSpinType - currentFreeSpinType; i++)
                                    {
                                        strResponse = await sendGambleRequest(httpClient);
                                        response    = JToken.Parse(strResponse);
                                        if (!IsNullOrEmpty(response["err"]))
                                            return null;

                                        if (IsNullOrEmpty(response["dt"]["si"]["fs"]))
                                            return null;

                                        _strLastID = response["dt"]["si"]["sid"].ToString();
                                    }
                                }

                                strResponse = await sendGambleEndRequest(httpClient);
                                response    = JToken.Parse(strResponse);
                                _strLastID  = response["dt"]["si"]["sid"].ToString();

                                double totalWin = response["dt"]["si"]["aw"].ToObject<double>();
                                SpinResponse spinResponse = new SpinResponse();
                                spinResponse.SpinType = 100;
                                spinResponse.TotalWin = totalWin;
                                spinResponse.Response = string.Join("\n", strResponseHistory);
                                responseList.Add(spinResponse);
                                startSpinWin = totalWin;
                                spinType = 200 + minFreeSpinType;
                                strResponseHistory.Clear();
                                isFirstFreeSpin = false;
                                continue;
                            }
                            isBreak = false;
                        }
                    }
                    if (!IsNullOrEmpty(response["dt"]["si"]["rwsp"]))
                        isBreak = false;

                    JObject siObj = response["dt"]["si"] as JObject;
                    _strLastID = siObj["sid"].ToString();
                    siObj.Remove("bl");
                    siObj.Remove("blab");
                    siObj.Remove("blb");
                    siObj.Remove("sid");
                    siObj.Remove("psid");

                    if(spinType >= 200)
                    {
                        response["dt"]["si"]["aw"] = Math.Round(response["dt"]["si"]["aw"].ToObject<double>() - startSpinWin, 2);
                    }
                    string strSpinData = siObj.ToString(Formatting.None);
                    strResponseHistory.Add(strSpinData);
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
