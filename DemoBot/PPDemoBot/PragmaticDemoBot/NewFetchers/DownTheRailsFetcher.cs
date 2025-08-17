using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class DownTheRailsFetcher : GameSpinDataFetcher
    {
        private int[] _freeSpinTypeCounts = new int[] { 100, 100, 0, 100, 100 };

        public DownTheRailsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = true;
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
            if (dicParams["trail"].Contains("fs_t~pv;"))
                return 0;
            else if (dicParams["trail"].Contains("fs_t~kc;"))
                return 1;
            else if (dicParams["trail"].Contains("fs_t~cw;"))
                return 2;
            else if (dicParams["trail"].Contains("fs_t~bp;"))
                return 3;
            else
                return 4;
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
                        double totalWin = 0.0;
                        if (dicParamValues.ContainsKey("tw"))
                            totalWin = double.Parse(dicParamValues["tw"]);

                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        if (dicParamValues.ContainsKey("rs_p") && dicParamValues["rs_p"] == "0")
                        {
                            if (responseList.Count == 0)
                            {
                                SpinResponse response = new SpinResponse();
                                response.SpinType     = 100;
                                response.Response     = string.Join("\n", strResponseHistory.ToArray());
                                response.TotalWin     = totalWin;
                                response.RealWin      = totalWin;
                                response.FreeSpinType = 4;
                                strResponseHistory.Clear();
                                responseList.Add(response);
                                selectedFreeOption = 4;
                                beforeFreeTotalWin = totalWin;
                            }
                        }
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "b")
                    {
                        if (selectedFreeOption == -1)
                        {
                            int freeSpinType        = findFreeSpinType(dicParamValues);
                            double totalWin         = double.Parse(dicParamValues["tw"]);
                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = 100;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin       = totalWin;
                            response.RealWin        = totalWin;
                            response.FreeSpinType   = freeSpinType;
                            strResponseHistory.Clear();
                            responseList.Add(response);
                            beforeFreeTotalWin = totalWin;
                            selectedFreeOption = selectFreeSpinType(freeSpinType);
                        }
                        int currentFreeSpinType = findFreeSpinType(dicParamValues);
                        if (currentFreeSpinType == selectedFreeOption)
                            doBonusID = 1;
                        else
                            doBonusID = 0;

                        strResponse     = await doBonus(httpClient, strToken, doBonusID);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];

                        if (strNextAction == "cb")
                            return null;
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
