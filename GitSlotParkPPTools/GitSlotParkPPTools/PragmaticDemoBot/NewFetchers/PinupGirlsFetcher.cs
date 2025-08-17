using BNGSpinFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    public class PinupGirlsFetcher : GameSpinDataFetcher
    {
        public PinupGirlsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
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
                    new KeyValuePair<string, string>("pur",     "0"),
                    new KeyValuePair<string, string>("mgckey",  strToken),
                    };

            return postValues;
        }

        protected async Task<string> sendPurSpinRequest(HttpClient httpClient, string strToken)
        {
            FormUrlEncodedContent   postContent = new FormUrlEncodedContent(buildDoPurSpinRequest(strToken));
            HttpResponseMessage     message     = null;

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
        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient, string strToken)
        {
            int count = 0;
            do
            {
                PinupGirlsResponse girlsResponse = new PinupGirlsResponse();
                girlsResponse.TotalWin = 0.0;
                girlsResponse.TotalWins = new List<double>();
                girlsResponse.SpinTypes = new List<int>();
                girlsResponse.Responses = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    List<SpinResponse> responses = await doSpin(httpClient, strToken);
                    if (responses == null)
                        return DoSpinsResults.NEEDRESTARTSESSION;
                    
                    if(responses.Count != 1)
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    SpinResponse response           = responses[0];
                    string[]     strResponses       = response.Response.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    string       strFirstResponse   = strResponses[0];
                    SortedDictionary<string, string> dicParams = splitAndRemoveCommonResponse(strFirstResponse);

                    string strIndex = dicParams["accv"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if(i != int.Parse(strIndex))
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    girlsResponse.TotalWin += response.TotalWin;
                    girlsResponse.TotalWins.Add(response.TotalWin);
                    girlsResponse.SpinTypes.Add(response.SpinType);
                    girlsResponse.Responses.Add(response.Response);
                }

                SpinDataQueue.Instance.addSpinDataToQueue(girlsResponse);
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
            List<string>        strResponseHistory  = new List<string>();
            List<SpinResponse>  responseList        = new List<SpinResponse>();
            string              strResponse         = "";
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
