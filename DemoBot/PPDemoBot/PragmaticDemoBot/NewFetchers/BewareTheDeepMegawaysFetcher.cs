using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PragmaticDemoBot;

namespace PragmaticDemoBot
{
    internal class BewareTheDeepMegawaysFetcher : EuroNoWinRespinFetcher
    {
        private int[] _freeSpinGroups = new int[] { 0, 100 };
        private int[] _freeSpinCounts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public BewareTheDeepMegawaysFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet, true, false)
        {

        }
        protected int selectMinFreeSpinGroup()
        {
            if (_freeSpinGroups[0] <= _freeSpinGroups[1])
                return 0;
            else
                return 1;
        }
        protected int selectMinFreeSpinType(int freeSpinGroup, int currentFreeSpinID)
        {
            int minCount = -1;
            int minIndex = 0;
            for(int i = 6 * freeSpinGroup + currentFreeSpinID; i <  6 * (freeSpinGroup + 1); i++)
            {
                if(minCount == -1 || minCount > _freeSpinCounts[i])
                {
                    minCount = _freeSpinCounts[i];
                    minIndex = i;
                }
            }
            return minIndex - 6 * freeSpinGroup;
        }
        protected int getBonusCount(SortedDictionary<string, string> dicParams)
        {
            string[] strSymbols = dicParams["s"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int bonusCount = 0;
            for(int i = 0; i <  strSymbols.Length; i++)
            {
                if (strSymbols[i] == "1")
                    bonusCount++;
            }
            if(bonusCount < 4 || bonusCount > 7)
            {

            }
            return bonusCount;
        }
        protected int getBonusIndex(int freeSpinGroup, SortedDictionary<string, string> dicParams)
        {
            string[] strParts = dicParams["trail"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i].StartsWith("fgri~"))
                {
                    int currentIndex = int.Parse(strParts[i].Replace("fgri~", ""));
                    return currentIndex;
                }
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
                strResponse = await sendSpinRequest(httpClient, strToken);
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
                double  beforeFreeTotalWin = 0.0;
                int     selectedFreeOption = -1;
                do
                {
                    if (strNextAction == "s")
                    {
                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        if (!dicParamValues.ContainsKey("na"))
                            Console.WriteLine(strResponse);

                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                        if (strNextAction == "s" && dicParamValues.ContainsKey("rs_t") && !dicParamValues.ContainsKey("fs")
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
                        double  totalWin        = double.Parse(dicParamValues["tw"]);

                        SpinResponse response   = new SpinResponse();
                        response.SpinType       = 100;
                        response.Response       = string.Join("\n", strResponseHistory.ToArray());
                        response.TotalWin       = totalWin;
                        response.RealWin        = totalWin;
                        response.FreeSpinType   = getBonusCount(dicParamValues) - 4;

                        strResponseHistory.Clear();
                        responseList.Add(response);

                        int freeSpinGroup       = selectMinFreeSpinGroup();
                        strResponse             = await doBonus(httpClient, strToken, freeSpinGroup);
                        dicParamValues          = splitAndRemoveCommonResponse(strResponse);
                        strNextAction           = dicParamValues["na"];

                        int currentIndex        = response.FreeSpinType + 2;
                        beforeFreeTotalWin      = totalWin;
                        if(currentIndex == 5)
                        {
                            selectedFreeOption = 200 + 6 * freeSpinGroup + currentIndex;
                            strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                        }
                        else
                        {
                            bool    isMinOrMax        = false;
                            int     minFreeSpinOption = selectMinFreeSpinType(freeSpinGroup, currentIndex);
                            while (currentIndex != minFreeSpinOption)
                            {

                                strResponse     = await doBonus(httpClient, strToken, 0);
                                dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                                strNextAction   = dicParamValues["na"];
                                currentIndex    = getBonusIndex(freeSpinGroup, dicParamValues);
                                if(currentIndex == 0 || currentIndex == 5)
                                {
                                    selectedFreeOption = 200 + 6 * freeSpinGroup + currentIndex;
                                    strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                                    isMinOrMax = true;
                                    break;
                                }
                                minFreeSpinOption = selectMinFreeSpinType(freeSpinGroup, currentIndex);
                            }
                            if(!isMinOrMax)
                            {
                                strResponse         = await doBonus(httpClient, strToken, 1);
                                dicParamValues      = splitAndRemoveCommonResponse(strResponse);
                                strNextAction       = dicParamValues["na"];
                                selectedFreeOption  = 200 + 6 * freeSpinGroup + currentIndex;
                                strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));

                            }
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
                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = selectedFreeOption;
                            response.TotalWin       = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
                            responseList.Add(response);
                            responseList[0].TotalWin = double.Parse(dicParamValues["tw"]);
                            _freeSpinCounts[selectedFreeOption - 200]++;
                            if (selectedFreeOption <= 205)
                                _freeSpinGroups[0]++;
                            else
                                _freeSpinGroups[1]++;
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
