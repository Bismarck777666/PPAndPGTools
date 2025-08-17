using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot.NewFetchers
{
    class SpiritOfAdventureFetcher : GameSpinDataFetcher
    {
        private static object _lockObj = new object();
        public SpiritOfAdventureFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet, bool isV4) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = isV4;
        }
        protected KeyValuePair<string, string>[] buildDoPurSpinRequest(string strToken)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
                    {
                    new KeyValuePair<string, string>("action",  "doSpin"),
                    new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                    new KeyValuePair<string, string>("c",       Math.Round(_defaultC, 2).ToString()),
                        new KeyValuePair<string, string>("l",       _lineCount.ToString()),
                    new KeyValuePair<string, string>("index",   _index.ToString()),
                    new KeyValuePair<string, string>("counter", _counter.ToString()),
                    new KeyValuePair<string, string>("repeat",  "0"),
                    new KeyValuePair<string, string>("pur",     "0"),
                    new KeyValuePair<string, string>("mgckey",  strToken),
                    };

            List<KeyValuePair<string, string>> postList = new List<KeyValuePair<string, string>>(postValues);
            if (_hasAnteBet)
                postList.Add(new KeyValuePair<string, string>("bl", "0"));
            return postList.ToArray();
        }

        protected async Task<string> sendPurSpinRequest(HttpClient httpClient, string strToken)
        {
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(buildDoPurSpinRequest(strToken));
            HttpResponseMessage message = null;

            if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();

            _index++;
            _counter += 2;

            if (strContent.Contains("action invalid"))
                throw new Exception("Action Invalid Found");

            return strContent;
        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            bool isFreeBonus = base.isFreeOrBonus(dicParams);
            if (isFreeBonus)
                return true;

            if (dicParams.ContainsKey("rs_p"))
                return true;
            return false;
        }
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";

            try
            {
                strResponse = await sendPurSpinRequest(httpClient, strToken);
                SortedDictionary<string, string> dicParamValues = splitAndRemoveCommonResponse(strResponse);
                string strNextAction = dicParamValues["na"];

                strResponseHistory.Add(combineResponse(dicParamValues));
                if (strNextAction == "c")
                {
                    await doCollect(httpClient, strToken);

                    SpinResponse response = new SpinResponse();
                    response.SpinType = 0;
                    response.TotalWin = double.Parse(dicParamValues["tw"]);
                    response.Response = string.Join("\n", strResponseHistory);
                    responseList.Add(response);
                    return responseList;
                }
                else if (strNextAction == "s" && !isFreeOrBonus(dicParamValues))
                {
                    //윈값이 0인 경우
                    SpinResponse response = new SpinResponse();
                    response.SpinType = 0;
                    response.TotalWin = double.Parse(dicParamValues["tw"]);
                    response.Response = string.Join("\n", strResponseHistory);
                    responseList.Add(response);
                    return responseList;
                }

                //프리스핀이나 보너스로 이행한다.
                int doBonusID = -1;
                double beforeFreeTotalWin = 0.0;
                int selectedFreeOption = -1;
                do
                {
                    if (strNextAction == "m")
                    {
                        strResponse = await doMysteryScatter(httpClient, strToken, doBonusID);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "fso")
                    {
                        int freeSpinType = findFreeSpinType(dicParamValues);
                        double totalWin = double.Parse(dicParamValues["tw"]);

                        SpinResponse response = new SpinResponse();
                        response.SpinType = 100;
                        response.Response = string.Join("\n", strResponseHistory.ToArray());
                        response.TotalWin = totalWin;
                        response.RealWin = totalWin;
                        response.FreeSpinType = freeSpinType;

                        strResponseHistory.Clear();
                        responseList.Add(response);

                        beforeFreeTotalWin = totalWin;
                        int freeSpinOption = selectFreeOption(freeSpinType);
                        selectedFreeOption = 200 + freeSpinType * FreeSpinOptionCount + freeSpinOption;
                        strResponse = await doFreeSpinOption(httpClient, strToken, freeSpinOption);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "s")
                    {
                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "b")
                    {

                        double      pw          = double.Parse(dicParamValues["pw"]);
                        string[]    candidates  = dicParamValues["trail"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        double      candidate1  = double.Parse(candidates[0]);
                        double      candidate2  = double.Parse(candidates[1]);
                        double      percent     = double.Parse(dicParamValues["trail"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                        double odd1 = candidate1 / pw;
                        double odd2 = candidate2 / pw;

                        strResponse    = await doBonus(httpClient, strToken, 1);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        double realWin = double.Parse(dicParamValues["tw"]);
                        double realOdd = realWin / pw;

                        lock(_lockObj)
                        {
                            if(Math.Round(odd1, 2) >= 0.1)
                                System.IO.File.AppendAllText("prob.txt", string.Format("{0},{1},{2},{3}\r\n", Math.Round(odd1, 2), Math.Round(odd2, 2), Math.Round(realOdd, 1), Math.Round(percent, 2)));
                        }

                    }
                    else if (strNextAction == "c")
                    {
                        await doCollect(httpClient, strToken);

                        if (selectedFreeOption == -1)
                        {
                            SpinResponse response = new SpinResponse();
                            response.SpinType = findSpinType(dicParamValues);
                            response.TotalWin = double.Parse(dicParamValues["tw"]);
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                        }
                        else
                        {
                            SpinResponse response = new SpinResponse();
                            response.SpinType = selectedFreeOption;
                            response.TotalWin = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                            responseList[0].TotalWin = double.Parse(dicParamValues["tw"]);
                        }
                        return responseList;
                    }
                    else if (strNextAction == "cb")
                    {
                        strResponse = await doCollectBonus(httpClient, strToken);
                        strResponseHistory.Add(combineResponse(splitAndRemoveCommonResponse(strResponse), beforeFreeTotalWin));

                        SpinResponse response = new SpinResponse();
                        response.SpinType = 0;
                        response.TotalWin = double.Parse(dicParamValues["tw"]);
                        response.Response = string.Join("\n", strResponseHistory.ToArray());
                        responseList.Add(response);
                        return responseList;
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                return null;
            }
        }
    }
}
