using BNGSpinFetcher;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class TheGreatChickenEscapeFetcher : GameSpinDataFetcher
    {
        private int[] _freeSpinTypeCounts = new int[] { 0, 0, 0, 0, 0 };

        public TheGreatChickenEscapeFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = false;
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

        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("crb_wheel"))
                return 4;

            string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] == "1")
                    return i;
            }
            return 0;
        }
        protected int selectFreeSpinType(int fromIndex)
        {
            int minCount = -1;
            int minIndex = 0;
            for (int i = fromIndex; i < 5; i++)
            {
                if (minCount == -1 || minCount > _freeSpinTypeCounts[i])
                {
                    minCount = _freeSpinTypeCounts[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        protected override async Task<string> doBonus(HttpClient httpClient, string strToken, int doBonusID)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doBonus"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("ind", doBonusID.ToString()),
                            new KeyValuePair<string, string>("mgckey", strToken),
            };
            if(doBonusID < 0)
            {
                postValues = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("action", "doBonus"),
                    new KeyValuePair<string, string>("symbol", _strGameSymbol),
                    new KeyValuePair<string, string>("index",  _index.ToString()),
                    new KeyValuePair<string, string>("counter",_counter.ToString()),
                    new KeyValuePair<string, string>("repeat", "0"),
                    new KeyValuePair<string, string>("mgckey", strToken),
                };
            }
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;

            if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
        }

        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient, string strToken)
        {
            int count = 0;
            do
            {
                List<SpinResponse> responses = await doSpin(httpClient, strToken);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                count++;

                if(responses.Count == 1 && responses[0].SpinType == 100)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                await Task.Delay(500);
                if (count >= 500)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";

            try
            {
                strResponse = await sendSpinRequest(httpClient, strToken);
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
                int     doBonusID          = -1;
                double  beforeFreeTotalWin = 0.0;
                int     selectedFreeOption = -1;
                bool    isEnteredBonusGame = false;
                bool    isMovedBigMoney    = false;
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
                        double totalWin = 0.0;
                        if (dicParamValues.ContainsKey("tw"))
                            totalWin = double.Parse(dicParamValues["tw"]);

                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "b")
                    {
                        if (isEnteredBonusGame)
                        {
                            if (selectedFreeOption == 0)
                            {
                                if(!isMovedBigMoney)
                                {
                                    strResponse = await doBonus(httpClient, strToken, 0);
                                    dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    strNextAction = dicParamValues["na"];
                                    if (dicParamValues.ContainsKey("crb_wheel"))
                                    {
                                        isMovedBigMoney = true;
                                        doBonusID = 0;
                                    }
                                }
                                else
                                {
                                    strResponse = await doBonus(httpClient, strToken, doBonusID);
                                    dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    strNextAction = dicParamValues["na"];
                                    if (strNextAction == "b")
                                    {
                                        strResponse = await doBonus(httpClient, strToken, doBonusID);
                                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                        strNextAction = dicParamValues["na"];
                                    }
                                    doBonusID++;
                                }
                            }
                            else if (selectedFreeOption == 1)
                            {
                                if (!isMovedBigMoney)
                                {
                                    strResponse = await doBonus(httpClient, strToken, 0);
                                    dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    strNextAction = dicParamValues["na"];

                                    if (dicParamValues.ContainsKey("crb_wheel"))
                                    {
                                        isMovedBigMoney = true;
                                        doBonusID = 0;
                                    }
                                }
                                else
                                {
                                    strResponse = await doBonus(httpClient, strToken, doBonusID);
                                    dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    strNextAction = dicParamValues["na"];
                                    if (strNextAction == "b")
                                    {
                                        strResponse = await doBonus(httpClient, strToken, doBonusID);
                                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                        strNextAction = dicParamValues["na"];
                                    }
                                    doBonusID++;
                                }
                            }
                            else if (selectedFreeOption == 4)
                            {
                                strResponse = await doBonus(httpClient, strToken, doBonusID);
                                dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                strNextAction = dicParamValues["na"];
                                if (strNextAction == "b")
                                {
                                    strResponse = await doBonus(httpClient, strToken, doBonusID);
                                    dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    strNextAction = dicParamValues["na"];
                                }
                                doBonusID++;
                            }
                        }
                        else
                        {
                            if (doBonusID == -1)
                            {
                                strResponse         = await doBonus(httpClient, strToken, 0);
                                dicParamValues      = splitAndRemoveCommonResponse(strResponse);
                                strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                                strResponse = await doBonus(httpClient, strToken, -1);
                                dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                strNextAction = dicParamValues["na"];
                                strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                doBonusID = 0;
                                continue;
                            }
                            if (selectedFreeOption == -1)
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
                                selectedFreeOption = selectFreeSpinType(freeSpinType);
                            }
                            int currentFreeSpinType = findFreeSpinType(dicParamValues);
                            if (currentFreeSpinType == selectedFreeOption)
                                doBonusID = 0;
                            else
                                doBonusID = 1;
                            if (currentFreeSpinType == 4)
                            {
                                await doBonus(httpClient, strToken, -1);
                                dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                strNextAction = dicParamValues["na"];
                                doBonusID = 0;
                                strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                isEnteredBonusGame = true;
                            }
                            else
                            {
                                strResponse = await doBonus(httpClient, strToken, doBonusID);
                                dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                strNextAction = dicParamValues["na"];

                                if (doBonusID == 0)
                                {
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                                    if (selectedFreeOption == 0 || selectedFreeOption == 1)
                                    {
                                        strResponse     = await doBonus(httpClient, strToken, -1);
                                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                                        strNextAction   = dicParamValues["na"];
                                        doBonusID       = 0;
                                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    }
                                    isEnteredBonusGame = true;
                                }
                                else if (doBonusID == 1)
                                {
                                    if (selectedFreeOption == 4 && currentFreeSpinType == 3 && strNextAction == "b")
                                    {
                                        strResponse = await doBonus(httpClient, strToken, -1);
                                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                        strNextAction = dicParamValues["na"];
                                        doBonusID = 0;
                                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                                        strResponse = await doBonus(httpClient, strToken, -1);
                                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                                        strNextAction = dicParamValues["na"];
                                        doBonusID = 0;
                                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                        
                                        isEnteredBonusGame = true;
                                        doBonusID = 0;
                                    }
                                }
                            }
                            if (strNextAction == "cb")
                                return responseList;
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
                            response.SpinType = 200 + selectedFreeOption;
                            response.TotalWin = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                            responseList[0].TotalWin = double.Parse(dicParamValues["tw"]);
                            _freeSpinTypeCounts[selectedFreeOption]++;
                        }
                        return responseList;
                    }
                    else if (strNextAction == "cb")
                    {
                        strResponse = await doCollectBonus(httpClient, strToken);
                        strResponseHistory.Add(combineResponse(splitAndRemoveCommonResponse(strResponse), beforeFreeTotalWin));
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
                            response.SpinType = 200 + selectedFreeOption;
                            response.TotalWin = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                            responseList[0].TotalWin = double.Parse(dicParamValues["tw"]);
                            _freeSpinTypeCounts[selectedFreeOption]++;
                        }
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
