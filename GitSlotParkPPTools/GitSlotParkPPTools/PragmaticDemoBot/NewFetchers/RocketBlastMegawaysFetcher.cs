using BNGSpinFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class RocketBlastMegawaysFetcher : GameSpinDataFetcher
    {
        private int[] _freeSpinTypeCounts = new int[] { 0, 100, 100, 100, 100, 100};
        public RocketBlastMegawaysFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
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

            List<KeyValuePair<string, string>> postList = new List<KeyValuePair<string, string>>(postValues);
            if (_hasAnteBet)
                postList.Add(new KeyValuePair<string, string>("bl", "0"));
            return postList.ToArray();
        }
        protected async Task<string> sendPurSpinRequest(HttpClient httpClient, string strToken)
        {
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(buildDoPurSpinRequest(strToken));
            HttpResponseMessage message = null;

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
                        new KeyValuePair<string, string>("mgckey",  strToken),
                                    };
            return postValues;
        }
        protected int selectFreeSpinType(int fromIndex)
        {
            int minCount = -1;
            int minIndex = 0;
            for (int i = fromIndex; i < 6; i++)
            {
                if (minCount == -1 || minCount > _freeSpinTypeCounts[i])
                {
                    minCount = _freeSpinTypeCounts[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }
        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            string strTrail = dicParams["trail"];
            string[] strParts = strTrail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

            int fsCount = int.Parse(strParts[1]);
            switch (fsCount)
            {
                case 10:
                    return 0;
                case 15:
                    return 1;
                case 20:
                    return 2;
                case 25:
                    return 3;
                case 30:
                    return 4;
                case 35:
                    return 5;
            }
            return 100;
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
                    return null;

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
                        double totalWin = 0.0;
                        if (dicParamValues.ContainsKey("tw"))
                            totalWin = double.Parse(dicParamValues["tw"]);

                        if (dicParamValues.ContainsKey("fs") && dicParamValues["fs"] == "1" && selectedFreeOption == -1)
                        {

                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = 100;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin       = totalWin;
                            response.RealWin        = totalWin;
                            response.FreeSpinType   = findFreeSpinType(dicParamValues);
                            strResponseHistory.Clear();
                            responseList.Add(response);
                            beforeFreeTotalWin      = totalWin;
                            selectedFreeOption      = findFreeSpinType(dicParamValues);
                        }

                        strResponse         = await sendSpinRequest(httpClient, strToken);
                        dicParamValues      = splitAndRemoveCommonResponse(strResponse);
                        strNextAction       = dicParamValues["na"];
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
                        int freeSpinType = findFreeSpinType(dicParamValues);
                        if (selectedFreeOption == -1)
                        {
                            double totalWin     = double.Parse(dicParamValues["tw"]);
                            dicParamValues      = splitAndRemoveCommonResponse(strResponse);
                            strNextAction       = dicParamValues["na"];

                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = 100;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
                            response.TotalWin       = totalWin;
                            response.RealWin        = totalWin;
                            response.FreeSpinType   = freeSpinType;
                            strResponseHistory.Clear();
                            responseList.Add(response);
                            beforeFreeTotalWin      = totalWin;
                            selectedFreeOption      = selectFreeSpinType(freeSpinType);
                        }
                        if (freeSpinType == selectedFreeOption)
                            doBonusID = 0;
                        else
                            doBonusID = 1;

                        if (freeSpinType < 5)
                        {
                            strResponse     = await doBonus(httpClient, strToken, doBonusID);
                            dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                            strNextAction   = dicParamValues["na"];
                            if (strNextAction != "b" && !dicParamValues.ContainsKey("fsmax"))
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
                            if (selectedFreeOption < 6)
                                _freeSpinTypeCounts[selectedFreeOption]++;

                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = selectedFreeOption + 200;
                            response.TotalWin       = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
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
                    await Task.Delay(200);
                } while (true);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                //Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                return null;
            }
        }

    }
}
