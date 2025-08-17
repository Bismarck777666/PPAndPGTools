using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragmaticDemoBot;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PragmaticDemoBot
{
    class BigBassDayAtTheRacesFetcher : EuroNoWinRespinFetcher
    {
        public BigBassDayAtTheRacesFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true)
        {

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
                if (!dicParamValues.ContainsKey("na"))
                    Console.WriteLine(strResponse);

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
                        if (!dicParamValues.ContainsKey("na"))
                            Console.WriteLine(strResponse);

                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                        if (strNextAction == "s" && dicParamValues.ContainsKey("rs_t") && !dicParamValues.ContainsKey("fs") && !dicParamValues.ContainsKey("rs_p")
                            && (double.Parse(dicParamValues["tw"]) == 0.0))
                        {
                            SpinResponse response = new SpinResponse();
                            response.SpinType = findSpinType(dicParamValues);
                            response.TotalWin = double.Parse(dicParamValues["tw"]);
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                            return responseList;
                        }

                    }
                    else if (strNextAction == "b")
                    {

                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        var gParam      = JToken.Parse(dicParamValues["g"]);
                        var status      = gParam["bg_0"]["status"].ToString();
                        var stArrray    = status.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        doBonusID = -1;
                        for (int i = 0; i < stArrray.Length; i++)
                        {
                            if (stArrray[i] == "0")
                            {
                                doBonusID = i;
                                break;
                            }
                        }
                        strResponse = await doBonus(httpClient, strToken, doBonusID);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
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
