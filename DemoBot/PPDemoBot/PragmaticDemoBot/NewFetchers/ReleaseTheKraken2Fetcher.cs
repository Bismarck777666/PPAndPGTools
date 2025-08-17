using PragmaticDemoBot;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class ReleaseTheKraken2Fetcher : GameSpinDataFetcher
    {
        public ReleaseTheKraken2Fetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = true;
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
                    new KeyValuePair<string, string>("pur",     "1"),
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

        private int findFreeSpinType(int freeSpinCount, int multiplier)
        {
            int[] freeSpinCounts = new int[] { 4, 5, 5, 6, 6, 8, 8, 8, 8, 10, 10, 10, 12, 12, 12, 15, 15, 20, 20, 20};
            int[] multipliers    = new int[] { 2, 1, 2, 1, 2, 1, 2, 4, 6, 2,  4,  6,  2,  4,  6,  4,  10, 6,  8,  10};
            for(int i = 0; i  < freeSpinCounts.Length; i++)
            {
                if (freeSpinCounts[i] == freeSpinCount && multipliers[i] == multiplier)
                    return i;
            }
            return 0;
        }
        private int findFirstStatus(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] == "0")
                    return i;
            }
            return 0;
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
                List<int> freeSpinTypes = new List<int>();
                do
                {
                    if (strNextAction == "m")
                    {
                        strResponse = await doMysteryScatter(httpClient, strToken, doBonusID);
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
                        var gParam = JToken.Parse(dicParamValues["g"]);
                        string strAsk = gParam["bg_0"]["ask"].ToString();
                        if (strAsk == "0")
                        {
                            doBonusID = 1;
                        }
                        else if (strAsk == "2")
                        {
                            doBonusID = findFirstStatus(gParam["bg_0"]["status"].ToString());
                        }
                        else if (strAsk == "1")
                        {
                            string strTrail = gParam["bg_0"]["trail"].ToString();
                            string strOfferPrize = strTrail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            if (strOfferPrize.StartsWith("offer_prize"))
                            {
                                string strOfferInfo = strOfferPrize.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                string[] strParts = strOfferInfo.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);

                                int offerFreeCount = int.Parse(strParts[0].Replace("f", ""));
                                int offerMultiplier = int.Parse(strParts[1].Replace("m", ""));

                                int freeSpinType = findFreeSpinType(offerFreeCount, offerMultiplier);
                                freeSpinTypes.Add(freeSpinType);
                            }
                            doBonusID = 1;
                        }
                        else if (strAsk == "3")
                        {
                            string strTrail = gParam["bg_0"]["trail"].ToString();
                            string strPrizes = strTrail.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string[] strPrizeList = strPrizes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strWins = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < strPrizeList.Length; i++)
                            {
                                bool found = false;
                                for(int j = 0; j < strWins.Length; j++)
                                {
                                    if (strPrizeList[i] == strWins[j])
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if(!found)
                                {
                                    string[] strParts   = strPrizeList[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                                    int offerFreeCount  = int.Parse(strParts[0].Replace("f", ""));
                                    int offerMultiplier = int.Parse(strParts[1].Replace("m", ""));
                                    int freeSpinType = findFreeSpinType(offerFreeCount, offerMultiplier);
                                    freeSpinTypes.Add(freeSpinType);
                                }
                            }
                            doBonusID = 1;
                        }
                        strResponse     = await doBonus(httpClient, strToken, doBonusID);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];
                        if (strNextAction == "s")
                        {
                            gParam = JToken.Parse(dicParamValues["g"]);
                            string strSelectedPrize = gParam["bg_0"]["trail"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string strSelFreeSpin = strSelectedPrize.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            string[] strParts = strSelFreeSpin.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            int offerFreeCount = int.Parse(strParts[0].Replace("f", ""));
                            int offerMultiplier = int.Parse(strParts[1].Replace("m", ""));
                            int freeSpinType = findFreeSpinType(offerFreeCount, offerMultiplier);
                            if (freeSpinTypes.Contains(freeSpinType))
                            {
                                int index = 0;
                                for (int i = 0; i < 2; i++)
                                {
                                    if (freeSpinTypes[freeSpinTypes.Count - 1 - i] == freeSpinType)
                                    {
                                        index = freeSpinTypes.Count - 1 - i;
                                        break;
                                    }
                                }
                                if(index == 0)
                                {

                                }
                                freeSpinTypes.RemoveAt(index);
                                freeSpinTypes.Insert(0, freeSpinType);
                            }
                            else
                            {

                            }
                            double totalWin = double.Parse(dicParamValues["tw"]);

                            SpinResponse response = new SpinResponse();
                            response.SpinType = 100;
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin = totalWin;
                            response.RealWin = totalWin;
                            response.FreeSpinTypes = string.Join(",", freeSpinTypes.ToArray());

                            strResponseHistory.Clear();
                            responseList.Add(response);

                            beforeFreeTotalWin = totalWin;
                            selectedFreeOption = 200 + freeSpinType;
                            strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                        }
                        else
                        {
                            strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
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
