using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PragmaticDemoBot
{
    class KoiPondFetcher : GameSpinDataFetcher
    {
        protected int _freeSpinType = 0;

        public KoiPondFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _strHostName = "demogamesfree-asia.pragmaticplay.net";
            _isV4        = true;
        }
        protected override int FreeSpinOptionCount
        {
            get { return 5; }
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

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
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
                    else if (strNextAction == "s")
                    {
                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "b")
                    {
                        if(dicParamValues.ContainsKey("trail"))
                        {
                            double totalWin = double.Parse(dicParamValues["tw"]);

                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = 100;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin       = totalWin;
                            response.RealWin        = totalWin;

                            strResponseHistory.Clear();
                            responseList.Add(response);
                            beforeFreeTotalWin = totalWin;
                            selectedFreeOption = 200 + _freeSpinType;

                            strResponse = await doBonus(httpClient, strToken, _freeSpinType);
                            _freeSpinType = (_freeSpinType + 1) % 5;
                        }
                        else
                        {
                            if (doBonusID == -1)
                                doBonusID = 0;
                            else
                                doBonusID++;
                            strResponse = await doBonus(httpClient, strToken, doBonusID);
                        }
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

        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            if (!dicParams.ContainsKey("fs_opt"))
                return 100;

            string strValue = dicParams["fs_opt"].Split(new string[] { "~" }, StringSplitOptions.None)[0];
            if (strValue == "15,1,1")
                return 0;
            else if (strValue == "19,1,1")
                return 1;
            else if (strValue == "23,1,1")
                return 2;

            return 100;
        }

    }
}
