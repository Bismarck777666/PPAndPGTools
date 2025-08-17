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
    class SecretOfCleopatraFetcher : PGSpinDataFetcher
    {
        protected int[] _freeSpinCounts = new int[] { 10, 8, 5, 3, 10, 10, 10, 0, 10, 10, 10, 0 };
        public SecretOfCleopatraFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
        {

        }
        protected int getFreeSpinType(int totalFreeCount)
        {
            switch(totalFreeCount)
            {
                case 10:
                    return 0;
                case 12:
                    return 1;
                case 14:
                    return 2;
            }
            return 0;
        }
        protected int minCountFreeSpinMultiple(int freeSpinType)
        {
            int startId  = freeSpinType * 4;
            int minCount = -1;
            int minIndex = 0;
            for(int i = 0; i < 4; i++)
            {
                if (minCount == -1 || _freeSpinCounts[startId + i] < minCount)
                {
                    minCount = _freeSpinCounts[startId + i];
                    minIndex = i;
                }
            }
            return minIndex;
        }
        protected async Task<string> sendGambleRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin/Bonus?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("gt",  "2"),
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
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin/Bonus?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("gt",  "1"),
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
            double startSpinWin = 0.0;
            string strResponse  = "";
            int spinType = 0;
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
                    if (spinType >= 200)
                        siObj["aw"] = Math.Round(siObj["aw"].ToObject<double>() - startSpinWin, 2);

                    string strSpinData = siObj.ToString(Formatting.None);
                    strResponseHistory.Add(strSpinData);
                    if(nextState == 3)
                    {
                        int freeSpinCount    = (int) siObj["fs"]["ts"];
                        if (freeSpinCount >= 16)
                            return null;
                        int freeSpinType     = getFreeSpinType(freeSpinCount);
                        int freeSpinMultiple = (int)siObj["fs"]["fsm"]; 
                        int minFreeMultiple  = minCountFreeSpinMultiple(freeSpinType) + 1;

                        if (freeSpinType < 2)
                            return null;

                        double totalWin             = response["dt"]["si"]["aw"].ToObject<double>();
                        SpinResponse spinResponse   = new SpinResponse();
                        spinResponse.SpinType       = 100;
                        spinResponse.TotalWin       = totalWin;
                        spinResponse.Response       = string.Join("\n", strResponseHistory);
                        spinResponse.FreeSpinType   = freeSpinType;
                        //responseList.Add(spinResponse);
                        startSpinWin                = totalWin;
                        spinType                    = 200 + freeSpinType * 4 + minFreeMultiple - 1;
                        strResponseHistory.Clear();

                        while (freeSpinMultiple != minFreeMultiple)
                        {
                            strResponse = await sendGambleRequest(httpClient);
                            response    = JToken.Parse(strResponse);
                            if (!IsNullOrEmpty(response["err"]))
                                return null;
                            
                            int nst = response["dt"]["si"]["nst"].ToObject<int>();
                            if (nst == 1)
                                return null;

                            freeSpinMultiple = (int)response["dt"]["si"]["fs"]["fsm"];
                            _strLastID = response["dt"]["si"]["sid"].ToString();

                        }
                        if (freeSpinMultiple < 4)
                        {
                            strResponse =  await sendGambleEndRequest(httpClient);
                            response = JToken.Parse(strResponse);
                            _strLastID = response["dt"]["si"]["sid"].ToString();
                        }
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
