using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    internal class EmeraldKingClassicFetcher : GameSpinDataFetcher
    {
        public EmeraldKingClassicFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
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

        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient, string strToken)
        {
            int count = 0;
            do
            {
                GroupedSpinDataResponse groupedSpinResponse = new GroupedSpinDataResponse();
                groupedSpinResponse.TotalWin = 0.0;
                groupedSpinResponse.TotalWins = new List<double>();
                groupedSpinResponse.Responses = new List<string>();
                do
                {
                    List<SpinResponse> responses = await doSpin(httpClient, strToken);
                    if (responses == null)
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    if (responses.Count != 1)
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    SpinResponse response = responses[0];
                    string[] strResponses = response.Response.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    string strFirstResponse = strResponses[0];
                    SortedDictionary<string, string> dicParams = splitAndRemoveCommonResponse(strFirstResponse);

                    string   strIndexInfo = dicParams["accv"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string[] strParts     = strIndexInfo.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if(strParts.Length != 2)
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    int endLevel = int.Parse(strParts[0]);
                    groupedSpinResponse.TotalWin += response.TotalWin;
                    groupedSpinResponse.TotalWins.Add(response.TotalWin);
                    groupedSpinResponse.Responses.Add(response.Response);
                    if (endLevel == 1)
                        break;
                } while (true);

                groupedSpinResponse.SpinCount = groupedSpinResponse.TotalWins.Count;
                SpinDataQueue.Instance.addSpinDataToQueue(groupedSpinResponse);
                count++;

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
                        if (doBonusID == -1)
                            doBonusID = 0;
                        else
                            doBonusID++;

                        strResponse = await doBonus(httpClient, strToken, doBonusID);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        if (strNextAction != "b")
                            doBonusID = -1;

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
