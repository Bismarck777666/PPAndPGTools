using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class SixJokerFetcher : EuroNoWinRespinFetcher
    {
        public SixJokerFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet, bool isV4, bool isAsia = false) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet, isV4,isAsia)
        {
            _isV4 = isV4;
            if (isAsia)
                _strHostName = "demogamesfree-asia.pragmaticplay.net";
        }
        protected override KeyValuePair<string, string>[] buildDoSpinRequest(string strToken)
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
                        new KeyValuePair<string, string>("bl",      "4"),
                        new KeyValuePair<string, string>("mgckey",  strToken),
                        };
            return postValues;
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
                    response.SpinType = findSpinType(dicParamValues);
                    response.TotalWin = double.Parse(dicParamValues["tw"]);
                    response.Response = string.Join("\n", strResponseHistory);
                    responseList.Add(response);
                    return responseList;
                }
                else if (strNextAction == "s" && !isFreeOrBonus(dicParamValues))
                {
                    //윈값이 0인 경우
                    SpinResponse response = new SpinResponse();
                    response.SpinType = findSpinType(dicParamValues);
                    response.TotalWin = double.Parse(dicParamValues["tw"]);
                    response.Response = string.Join("\n", strResponseHistory);
                    responseList.Add(response);
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
                    else if (strNextAction == "c")
                    {
                        await doCollect(httpClient, strToken);

                        SpinResponse response = new SpinResponse();
                        response.SpinType = findSpinType(dicParamValues);
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

        protected override int findSpinType(SortedDictionary<string, string> dicParams)
        {
            return 4;
        }
    }
}
