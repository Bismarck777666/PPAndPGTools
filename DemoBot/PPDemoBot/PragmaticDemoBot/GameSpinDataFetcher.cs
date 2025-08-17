using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace PragmaticDemoBot
{
    public class GameSpinDataFetcher : ISpinDataFetcher
    {
        protected int       _index          = 2;
        protected int       _counter        = 3;
        protected bool      _mustStop       = false;
        protected string    _proxyInfo      = null;
        protected string    _proxyUserID    = null;
        protected string    _proxyPassword  = null;
        protected string    _strGameSymbol  = null;
        protected double    _defaultC       = 0.01;
        protected int       _lineCount      = 20;
        protected string    _clientVersion  = null;
        protected double    _realBet        = 0.0;
        protected bool      _hasAnteBet     = false;
        protected string    _strHostName    = "demogamesfree.pragmaticplay.net";
        //protected string _strHostName       = "demogamesfree.ppgames.net";
        protected bool      _isV3           = true;
        protected bool      _isV4           = false;
        protected bool      _isV5           = false;
        protected Random    _random         = new Random((int)DateTime.Now.Ticks);

        protected Dictionary<int, int>  _lastSelectedOptions    = new Dictionary<int, int>();
        protected bool                  _freeOptionSelDirection = true;
        
        
        public GameSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _clientVersion  = strClientVersion;
            _realBet        = realBet;
            _hasAnteBet     = hasAnteBet;
        }

        protected virtual int FreeSpinOptionCount
        {
            get { return 0; }
        }

        protected virtual int selectFreeOption(int freeSpinType)
        {
            if(!_lastSelectedOptions.ContainsKey(freeSpinType))
            {
                if (_freeOptionSelDirection)
                {
                    _lastSelectedOptions[freeSpinType] = 0;
                    return 0;
                }
                else
                {
                    _lastSelectedOptions[freeSpinType] = FreeSpinOptionCount - 1;
                    return FreeSpinOptionCount - 1;
                }
            }
            if(_freeOptionSelDirection)
            {
                _lastSelectedOptions[freeSpinType]++;
                if (_lastSelectedOptions[freeSpinType] >= FreeSpinOptionCount)
                    _lastSelectedOptions[freeSpinType] = 0;

                return _lastSelectedOptions[freeSpinType];
            }
            else
            {
                _lastSelectedOptions[freeSpinType]--;
                if (_lastSelectedOptions[freeSpinType] < 0)
                    _lastSelectedOptions[freeSpinType] = 0;

                return _lastSelectedOptions[freeSpinType];
            }

        }
        public override void doStop()
        {
            _mustStop = true;
        }
        protected async Task<string> findToken(HttpClient httpClient, string strGameURL)
        {
            try
            {
                HttpResponseMessage message = await httpClient.GetAsync(strGameURL);
                string strContent   = await message.Content.ReadAsStringAsync();
                message.EnsureSuccessStatusCode();

                if(strGameURL.StartsWith("https://demogamesfree.pragmaticplay.net"))
                {
                    string  strRedirectURL = message.RequestMessage.RequestUri.ToString();
                    int     startIndex     = strRedirectURL.IndexOf("&mgckey=") + "&mgckey=".Length;
                    int     endIndex       = strRedirectURL.IndexOf("&", startIndex);
                    if (endIndex < 0)
                        return strRedirectURL.Substring(startIndex);
                    else
                        return strRedirectURL.Substring(startIndex, endIndex - startIndex);
                }
                else if (strGameURL.StartsWith("https://demogamesfree.ppgames.net"))
                {
                    string strRedirectURL = message.RequestMessage.RequestUri.ToString();
                    int startIndex = strRedirectURL.IndexOf("&mgckey=") + "&mgckey=".Length;
                    int endIndex = strRedirectURL.IndexOf("&", startIndex);
                    if (endIndex < 0)
                        return strRedirectURL.Substring(startIndex);
                    else
                        return strRedirectURL.Substring(startIndex, endIndex - startIndex);
                }
                else
                {
                    string strPattern = string.Format("data-game-src=\"https://{0}/gs2c/openGame.do?", _strHostName);
                    int startIndex = strContent.IndexOf(strPattern) + 15;
                    int endIndex = strContent.IndexOf("\"", startIndex);

                    string strOpenURL = strContent.Substring(startIndex, endIndex - startIndex);
                    message = await httpClient.GetAsync(strOpenURL);
                    message.EnsureSuccessStatusCode();
                    string strGameStartURL = message.RequestMessage.RequestUri.ToString();
                    strPattern = "mgckey=";
                    startIndex = strGameStartURL.IndexOf(strPattern) + strPattern.Length;
                    string strToken = strGameStartURL.Substring(startIndex);
                    return strToken;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception has been occurred in GameSpinDataFetcher::findToken {0}", ex);
                return null;
            }
        }
        protected virtual async Task<bool> doInit(HttpClient httpClient, string strGameSymbol, string strToken)
        {
            try
            {
                KeyValuePair<string, string>[] initPostValues = new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("action",  "doInit"),
                        new KeyValuePair<string, string>("symbol",  strGameSymbol),
                        new KeyValuePair<string, string>("cver",    _clientVersion),
                        new KeyValuePair<string, string>("index",   "1"),
                        new KeyValuePair<string, string>("counter", "1"),
                        new KeyValuePair<string, string>("repeat",  "0"),
                        new KeyValuePair<string, string>("mgckey",  strToken),
                    };

                FormUrlEncodedContent   postContent     = new FormUrlEncodedContent(initPostValues);
                HttpResponseMessage message = null;
                if (_isV5)
                    message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
                else if (_isV4)
                    message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
                else if (_isV3)
                    message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
                else
                    message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

                string strContent = await message.Content.ReadAsStringAsync();
                message.EnsureSuccessStatusCode();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception has been occurred in GameSpinDataFetcher::doInit {0}", ex);
                return false;
            }
        }
        public override async Task startFetch(string strGameSymbol, string strGameURL, double defaultC, int lineCount)
        {
            try
            {
                _strGameSymbol  = strGameSymbol;
                _defaultC       = defaultC;
                _lineCount      = lineCount;
                var proxy = new WebProxy
                {

                   Address = new Uri(string.Format("http://{0}", _proxyInfo)),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials   = false,
                    Credentials             = new NetworkCredential(
                    userName: _proxyUserID,
                    password: _proxyPassword)
                };

                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.Proxy = proxy;
                do
                {
                    HttpClient httpClient = new HttpClient(httpClientHandler);

                    string strToken = await findToken(httpClient, strGameURL);
                    if(strToken == null)
                    {
                        Console.WriteLine("Finding Token failed. retry now");
                        await Task.Delay(1000);
                        continue;
                    }

                    if(!await doInit(httpClient, strGameSymbol, strToken))
                    {
                        Console.WriteLine("Do Init failed. retry now");
                        await Task.Delay(1000);
                        continue;
                    }

                    DoSpinsResults result = await doSpins(httpClient, strToken);
                    if(result == DoSpinsResults.NEEDRESTARTSESSION)
                    {
                        Console.WriteLine("restart session");
                        continue;
                    }
                    else
                    {
                        break;
                    }
                } while (true);
                Console.WriteLine("close session");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        protected virtual async Task<DoSpinsResults> doSpins(HttpClient httpClient, string strToken)
        {
            int count = 0;
            do
            {
                List<SpinResponse> responses = await doSpin(httpClient, strToken);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                count++;

                await Task.Delay(1000);
                if (count >= 500)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }
        protected async Task<string> sendSpinRequest(HttpClient httpClient, string strToken)
        {
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(buildDoSpinRequest(strToken));
            HttpResponseMessage message = null;

            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
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
        protected virtual KeyValuePair<string, string>[] buildDoSpinRequest(string strToken)
        {
            if(!_hasAnteBet)
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
                        new KeyValuePair<string, string>("bl",      "0"),
                        new KeyValuePair<string, string>("mgckey",  strToken),
                        };
                return postValues;

            }
        }
        protected virtual SortedDictionary<string, string> removeCommonParts(SortedDictionary<string, string> dicParamValues)
        {
            SortedDictionary<string, string> resultParams = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in dicParamValues)
                resultParams.Add(pair.Key, pair.Value);

            resultParams.Remove("index");
            resultParams.Remove("counter");
            resultParams.Remove("balance");
            resultParams.Remove("balance_cash");
            resultParams.Remove("balance_bonus");
            resultParams.Remove("stime");
            resultParams.Remove("sh");
            resultParams.Remove("c");
            resultParams.Remove("l");
            resultParams.Remove("sver");
            if (_hasAnteBet)
                resultParams.Remove("bl");
            return resultParams;
        }
        protected string combineResponse(SortedDictionary<string, string> dicParams)
        {
            List<string> stringList = new List<string>();
            foreach (KeyValuePair<string, string> pair in dicParams)
            {
                stringList.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", stringList.ToArray());
        }
        protected string combineResponse(SortedDictionary<string, string> dicParams, double minusTotalWin)
        {            
            List<string> stringList = new List<string>();
            foreach (KeyValuePair<string, string> pair in dicParams)
            {
                if(pair.Key == "tw" && minusTotalWin > 0.0)
                {
                    double totalWin = double.Parse(pair.Value) - minusTotalWin;
                    stringList.Add(string.Format("tw={0}", Math.Round(totalWin, 2).ToString()));
                }
                else
                {
                    stringList.Add(string.Format("{0}={1}", pair.Key, pair.Value));
                }
            }
            return string.Join("&", stringList.ToArray());
        }
        protected SortedDictionary<string, string> splitAndRemoveCommonResponse(string strResponse)
        {
            string[] strEntries = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            SortedDictionary<string, string> dicParamValues = new SortedDictionary<string, string>();
            for (int i = 0; i < strEntries.Length; i++)
            {
                string[] strParams = strEntries[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParams.Length == 2)
                {
                    dicParamValues.Add(strParams[0], strParams[1]);
                }
                else
                {
                    dicParamValues.Add(strParams[0], "");
                }
            }
            return removeCommonParts(dicParamValues);
        }
        protected async Task doCollect(HttpClient httpClient, string strToken)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doCollect"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("mgckey", strToken),
            };

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;
            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;

        }
        protected virtual bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("fs"))
                return true;

            if (dicParams.ContainsKey("tmb"))
                return true;
            return false;
        }
        protected virtual int findSpinType(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("fs_total"))
                return 1;
            else
                return 0;
        }

        protected virtual async Task<string> doBonus(HttpClient httpClient, string strToken, int doBonusID)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doBonus"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("mgckey", strToken),
                            new KeyValuePair<string, string>("ind", doBonusID.ToString()),
            };

            List<KeyValuePair<string, string>> postParams = new List<KeyValuePair<string, string>>(postValues);
            if (doBonusID < 0)
                postParams.RemoveAt(postParams.Count - 1);
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postParams.ToArray());
            HttpResponseMessage message = null;

            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
        }
        protected virtual async Task<string> doMysteryScatter(HttpClient httpClient, string strToken, int doBonusID)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doMysteryScatter"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("mgckey", strToken),
            };

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;

            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
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
        protected async Task<string> doCollectBonus(HttpClient httpClient, string strToken)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doCollectBonus"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("mgckey", strToken),
            };

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;
            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
        }
        protected virtual int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            return 0;
        }
        protected virtual async Task<string> doFreeSpinOption(HttpClient httpClient, string strToken, int option)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("action", "doFSOption"),
                new KeyValuePair<string, string>("symbol", _strGameSymbol),
                new KeyValuePair<string, string>("index",  _index.ToString()),
                new KeyValuePair<string, string>("counter",_counter.ToString()),
                new KeyValuePair<string, string>("repeat", "0"),
                new KeyValuePair<string, string>("mgckey", strToken),
                new KeyValuePair<string, string>("ind", option.ToString()),
            };

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;
            if (_isV5)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v5/gameService", _strHostName), postContent);
            else if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;

            return await message.Content.ReadAsStringAsync();
        }
        protected virtual async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken)
        {
            List<string>        strResponseHistory  = new List<string>();
            List<SpinResponse>  responseList        = new List<SpinResponse>();
            string              strResponse = "";

            try
            {
                strResponse                                         = await sendSpinRequest(httpClient, strToken);
                SortedDictionary<string, string>    dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                string                              strNextAction   = dicParamValues["na"];
                
                strResponseHistory.Add(combineResponse(dicParamValues));
                if (strNextAction == "c")
                {
                    await doCollect(httpClient, strToken);

                    SpinResponse response   = new SpinResponse();
                    response.SpinType       = 0;
                    response.TotalWin       = double.Parse(dicParamValues["tw"]);
                    response.Response       = string.Join("\n", strResponseHistory);
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
                int     doBonusID           = -1;
                double  beforeFreeTotalWin  = 0.0;
                int     selectedFreeOption  = -1;
                do
                {
                    if(strNextAction == "m")
                    {
                        strResponse     = await doMysteryScatter(httpClient, strToken, doBonusID);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if(strNextAction == "fso")
                    {
                        int     freeSpinType   = findFreeSpinType(dicParamValues);
                        double  totalWin       = double.Parse(dicParamValues["tw"]);

                        SpinResponse response  = new SpinResponse();
                        response.SpinType      = 100;
                        response.Response      = string.Join("\n", strResponseHistory.ToArray());
                        response.TotalWin      = totalWin;
                        response.RealWin       = totalWin;
                        response.FreeSpinType  = freeSpinType;

                        strResponseHistory.Clear();
                        responseList.Add(response);

                        beforeFreeTotalWin      = totalWin;
                        int freeSpinOption      = selectFreeOption(freeSpinType);
                        selectedFreeOption      = 200 + freeSpinType * FreeSpinOptionCount + freeSpinOption;
                        strResponse             = await doFreeSpinOption(httpClient, strToken, freeSpinOption);
                        dicParamValues          = splitAndRemoveCommonResponse(strResponse);
                        strNextAction           = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "s")
                    {
                        strResponse     = await sendSpinRequest(httpClient, strToken);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if(strNextAction == "b")
                    {
                        if (doBonusID == -1)
                            doBonusID = 0;
                        else
                            doBonusID++;

                        strResponse     = await doBonus(httpClient, strToken, doBonusID);
                        dicParamValues  = splitAndRemoveCommonResponse(strResponse);
                        strNextAction   = dicParamValues["na"];
                        if (strNextAction != "b")
                            doBonusID = -1;

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "c")
                    {
                        await doCollect(httpClient, strToken);

                        if(selectedFreeOption == -1)
                        {
                            SpinResponse response   = new SpinResponse();
                            response.SpinType       = findSpinType(dicParamValues);
                            response.TotalWin       = double.Parse(dicParamValues["tw"]);
                            response.Response       = string.Join("\n", strResponseHistory.ToArray());
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
                        }
                        return responseList;
                    }
                    else if(strNextAction == "cb")
                    {
                        strResponse = await doCollectBonus(httpClient, strToken);
                        strResponseHistory.Add(combineResponse(splitAndRemoveCommonResponse(strResponse), beforeFreeTotalWin));

                        SpinResponse response = new SpinResponse();
                        response.SpinType     = 0;
                        response.TotalWin     = double.Parse(dicParamValues["tw"]);
                        response.Response     = string.Join("\n", strResponseHistory.ToArray());
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

    public enum DoSpinsResults
    {
        USERSTOPPED         = 0,
        NEEDRESTARTSESSION  = 1,
    }

    public class PinupGirlsResponse
    {
        public double           TotalWin    { get; set; }
        public List<int>        SpinTypes   { get; set; }
        public List<double>     TotalWins   { get; set; }
        public List<string>     Responses   { get; set; }
        public double           TotalOdd    { get; set; }
        public List<double>     SpinOdds    { get; set; }

    }
    public class GroupedSpinDataResponse
    {
        public double       TotalWin    { get; set; }
        public List<double> TotalWins   { get; set; }
        public List<string> Responses   { get; set; }
        public double       TotalOdd    { get; set; }
        public List<double> SpinOdds    { get; set; }
        public int          SpinCount   { get; set; }

    }
    public class SpinResponse
    {
        public double   TotalWin        { get; set; }
        public double   RealWin         { get; set; }
        public int      SpinType        { get; set; }
        public string   Response        { get; set; }
        public int      FreeSpinType    { get; set; }
        public string   FreeSpinTypes   { get; set; }
    }
    public class BroncoSpinResponse : SpinResponse
    {
        public int WildCount { get; set; }
        public bool IsLast { get; set; }
    }

    public class CashElevatorSpinResponse : SpinResponse
    {
        public int BeginFloor { get; set; }
        public int EndFloor { get; set; }
    }

}
