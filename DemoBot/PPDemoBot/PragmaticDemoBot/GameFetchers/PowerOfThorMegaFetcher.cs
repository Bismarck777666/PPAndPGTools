using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class PowerOfThorMegaFetcher : GameSpinDataFetcher
    {
        public PowerOfThorMegaFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }
        protected override async Task<string> doBonus(HttpClient httpClient, string strToken, int doBonusID)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action",  "doBonus"),
                            new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                            new KeyValuePair<string, string>("index",   _index.ToString()),
                            new KeyValuePair<string, string>("counter", _counter.ToString()),
                            new KeyValuePair<string, string>("ind",     doBonusID.ToString()),
                            new KeyValuePair<string, string>("repeat",  "0"),
                            new KeyValuePair<string, string>("mgckey",  strToken),
            };

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;

            if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
        }

        private int getFreeSpinType(int fsmax)
        {
            switch(fsmax)
            {
                case 10:
                    return 0;
                case 14:
                    return 1;
                case 18:
                    return 2;
                case 22:
                    return 3;
                case 26:
                    return 4;
                case 30:
                    return 5;
            }
            return 10;

        }
        protected virtual KeyValuePair<string, string>[] buildDoSpinPurchaseRequest(string strToken)
        {
            if (!_hasAnteBet)
            {
                KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
                        {
                        new KeyValuePair<string, string>("action",  "doSpin"),
                        new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                        new KeyValuePair<string, string>("c",       Math.Round(_defaultC, 2).ToString()),
                        new KeyValuePair<string, string>("l",       _lineCount.ToString()),
                        new KeyValuePair<string, string>("index",   _index.ToString()),
                        new KeyValuePair<string, string>("counter", _counter.ToString()),
                        new KeyValuePair<string, string>("pur",  "0"),
                         new KeyValuePair<string, string>("repeat",  "0"),
                        new KeyValuePair<string, string>("mgckey",  strToken),
                        };
                return postValues;
            }
            else
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
                        new KeyValuePair<string, string>("pur",  "0"),
                        new KeyValuePair<string, string>("bl",      "0"),
                        new KeyValuePair<string, string>("mgckey",  strToken),
                        };
                return postValues;

            }
        }
        protected async Task<string> sendPurchaseSpinRequest(HttpClient httpClient, string strToken)
        {
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(buildDoSpinPurchaseRequest(strToken));
            HttpResponseMessage message = null;

            if (_isV3)
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

        protected override SortedDictionary<string, string> removeCommonParts(SortedDictionary<string, string> dicParamValues)
        {
            dicParamValues.Remove("puri");
            dicParamValues.Remove("purtr");

            return base.removeCommonParts(dicParamValues);
        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("fs"))
                return true;

            if (dicParams.ContainsKey("tmb_win"))
                return true;
            return false;
        }
        
        /*
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
                double beforeFreeTotalWin = 0.0;
                int selectedFreeOption = -1;
                bool started = false;
                do
                {
                    if (strNextAction == "s")
                    {
                        double totalWin = 0.0;
                        if (dicParamValues.ContainsKey("tw"))
                            totalWin = double.Parse(dicParamValues["tw"]);

                        strResponse     = await sendSpinRequest(httpClient, strToken);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];

                        if(dicParamValues.ContainsKey("fs") && dicParamValues["fs"] == "2" && !started)
                        {
                            SpinResponse response = new SpinResponse();
                            response.SpinType = 100;
                            response.Response = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin = totalWin;
                            response.RealWin  = totalWin;
                            response.FreeSpinType = getFreeSpinType(int.Parse(dicParamValues["fsmax"]));
                            strResponseHistory.Clear();
                            responseList.Add(response);
                            started = true;

                            beforeFreeTotalWin = totalWin;
                            selectedFreeOption = 200 + response.FreeSpinType;
                        }
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
        }*/
        
        
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";

            try
            {
                strResponse = await sendPurchaseSpinRequest(httpClient, strToken);
                SortedDictionary<string, string> dicParamValues = splitAndRemoveCommonResponse(strResponse);
                string strNextAction = dicParamValues["na"];

                while(strNextAction == "s" && dicParamValues.ContainsKey("tmb_win"))
                {
                    strResponse     = await sendSpinRequest(httpClient, strToken);
                    dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                    strNextAction   = dicParamValues["na"];
                }
                do
                {
                    if(strNextAction == "b")
                    {
                        int prb = 0;
                        if (dicParamValues["status"] == "1,0,0,0")
                            prb = 100;
                        else if (dicParamValues["status"] == "0,1,0,0")
                            prb = 80;
                        else if (dicParamValues["status"] == "0,0,1,0")
                            prb = 50;

                        if(_random.Next(0, 100) < prb)
                        {
                            strResponse = await doBonus(httpClient, strToken, 1);
                        }
                        else
                        {
                            strResponse = await doBonus(httpClient, strToken, 0);
                        }

                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];
                    }
                    else if(strNextAction == "s")
                    {
                        if (!dicParamValues.ContainsKey("fs"))
                            return null;
                        break;
                    }
                    else
                    {
                        return null;
                    }
                } while (true);

                //프리스핀이나 보너스로 이행한다.
                double beforeFreeTotalWin = 0.0;
                int selectedFreeOption = -1;
                bool started = false;
                do
                {
                    if (strNextAction == "s")
                    {
                        double totalWin = 0.0;
                        if (dicParamValues.ContainsKey("tw"))
                            totalWin = double.Parse(dicParamValues["tw"]);

                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        if (dicParamValues.ContainsKey("fs") && dicParamValues["fs"] == "2" && !started)
                        {                            
                            started = true;
                            beforeFreeTotalWin = totalWin;
                            selectedFreeOption = 200 + getFreeSpinType(int.Parse(dicParamValues["fsmax"]));
                        }
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
