using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PragmaticDemoBot
{
    class CashElevatorFetcher : GameSpinDataFetcher
    {
        private int _currentFloor = 3;
        public CashElevatorFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }
        private int getFloor(string strAccv)
        {
            string[] strParts = strAccv.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(strParts[2]);
        }
        protected override async Task<bool> doInit(HttpClient httpClient, string strGameSymbol, string strToken)
        {
            _currentFloor = 3;
            return await base.doInit(httpClient, strGameSymbol, strToken);
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
            HttpResponseMessage message = null;

            if(_isV4)
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

                    CashElevatorSpinResponse response = new CashElevatorSpinResponse();
                    response.SpinType   = 0;
                    response.TotalWin   = double.Parse(dicParamValues["tw"]);
                    response.Response   = string.Join("\n", strResponseHistory);
                    response.BeginFloor = _currentFloor;
                    response.EndFloor   = getFloor(dicParamValues["accv"]);                    
                    responseList.Add(response);
                    _currentFloor       = response.EndFloor;
                    return responseList;
                }
                else if (strNextAction == "s" && !isFreeOrBonus(dicParamValues))
                {
                    //윈값이 0인 경우
                    CashElevatorSpinResponse response = new CashElevatorSpinResponse();
                    response.SpinType = 0;
                    response.TotalWin = double.Parse(dicParamValues["tw"]);
                    response.Response = string.Join("\n", strResponseHistory);
                    response.BeginFloor = _currentFloor;
                    response.EndFloor = getFloor(dicParamValues["accv"]);
                    responseList.Add(response);
                    _currentFloor = response.EndFloor;
                    return responseList;
                }

                //프리스핀이나 보너스로 이행한다.
                double beforeFreeTotalWin = 0.0;
                do
                {
                    if (strNextAction == "s")
                    {
                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "b")
                    {
                        strResponse = await doBonus(httpClient, strToken, 0);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "c")
                    {
                        await doCollect(httpClient, strToken);

                        CashElevatorSpinResponse response = new CashElevatorSpinResponse();
                        response.SpinType = findSpinType(dicParamValues);
                        response.TotalWin = double.Parse(dicParamValues["tw"]);
                        response.Response = string.Join("\n", strResponseHistory.ToArray());
                        response.BeginFloor = _currentFloor;
                        response.EndFloor = getFloor(dicParamValues["accv"]);
                        responseList.Add(response);
                        _currentFloor = response.EndFloor;
                        return responseList;
                    }
                    else if (strNextAction == "cb")
                    {
                        strResponse = await doCollectBonus(httpClient, strToken);
                        strResponseHistory.Add(combineResponse(splitAndRemoveCommonResponse(strResponse), beforeFreeTotalWin));

                        CashElevatorSpinResponse response = new CashElevatorSpinResponse();
                        response.SpinType = 0;
                        response.TotalWin = double.Parse(dicParamValues["tw"]);
                        response.Response = string.Join("\n", strResponseHistory.ToArray());
                        response.BeginFloor = _currentFloor;
                        response.EndFloor = getFloor(dicParamValues["accv"]);
                        responseList.Add(response);
                        _currentFloor = response.EndFloor;
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
