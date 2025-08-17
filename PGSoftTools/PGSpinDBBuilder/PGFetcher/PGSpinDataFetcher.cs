using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;


namespace PGSpinDBBuilder
{
    class PGSpinDataFetcher 
    {
        protected int           _gameID;
        protected float         _betSize;
        protected int           _betLevel;
        protected string        _strGameSymbol;

        protected bool          _mustStop = false;

        protected string _proxyInfo     = null;
        protected string _proxyUserID   = null;
        protected string _proxyPassword = null;
        private Random   _random        = new Random((int)DateTime.Now.Ticks);
        protected string _strToken      = null;
        protected string _strLastID     = "0";

        public PGSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _gameID         = gameID;
            _betSize        = betSize;
            _betLevel       = betLevel;
        }
        protected string genRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private string findStringBetwwen(string strSource, string strStart, string strEnd)
        {
            int startIndex = strSource.IndexOf(strStart) + strStart.Length;
            int endIndex   = strSource.IndexOf(strEnd, startIndex);
            return strSource.Substring(startIndex, endIndex - startIndex);
        }
        public virtual async Task startFetch()
        {
            /*
            var proxy = new WebProxy
            {
                Address = new Uri(string.Format("http://{0}", _proxyInfo)),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                userName: _proxyUserID,
                password: _proxyPassword)
            };
            */
            var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.Proxy = proxy;
            do
            {
                try
                {

                    _strLastID = "0";
                    HttpClient httpClient = new HttpClient(httpClientHandler);
                    httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.pgsoft-games.com/");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://m.pgsoft-games.com");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"106\", \"Google Chrome\";v=\"106\", \"Not; A = Brand\";v=\"99\"");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");

                    string strVerifySessionURL = string.Format("https://api.pgsoft-games.com/web-api/auth/session/v2/verifySession?traceId={0}", genRandomId(8));
                    FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("vc", "2"),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("l", "en"),
                            new KeyValuePair<string, string>("gi", _gameID.ToString()),
                            new KeyValuePair<string, string>("tk", "null"),
                            new KeyValuePair<string, string>("otk", "ca7094186b309ee149c55c8822e7ecf2"),
                    });


                    HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
                    message.EnsureSuccessStatusCode();

                    string strContent = await message.Content.ReadAsStringAsync();
                    JToken jToken = JToken.Parse(strContent);
                    _strToken = jToken["dt"]["tk"].ToString();
                    string strGameURL = jToken["dt"]["geu"].ToString();
                    _strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

                    string strGetURL = string.Format("https://api.pg-demo.com/web-api/game-proxy/v2/GameName/Get?traceId={0}", genRandomId(8));
                    postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                            new KeyValuePair<string, string>("lang", "en"),
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("atk", _strToken),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("gid", _gameID.ToString())
                    });
                    message = await httpClient.PostAsync(strGetURL, postContent);
                    message.EnsureSuccessStatusCode();
                    strContent = await message.Content.ReadAsStringAsync();

                    strGetURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", _strGameSymbol, genRandomId(8));
                    postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                    {
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("atk", _strToken),
                            new KeyValuePair<string, string>("pf", "1"),
                    });
                    message = await httpClient.PostAsync(strGetURL, postContent);
                    message.EnsureSuccessStatusCode();
                    strContent = await message.Content.ReadAsStringAsync();


                    _strLastID = "0";
                    DoSpinsResults result = await doSpins(httpClient);
                    if (result == DoSpinsResults.NEEDRESTARTSESSION)
                    {
                        Console.WriteLine("restart session");
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            } while (true);
            Console.WriteLine("close session");
        }

        protected virtual async Task<DoSpinsResults> doSpins(HttpClient httpClient)
        {
            int count = 0;
            do
            {
                List<SpinResponse> responses = await doSpin(httpClient);
                if (responses == null)
                    return DoSpinsResults.NEEDRESTARTSESSION;


                SpinDataQueue.Instance.addSpinDataToQueue(responses);
                if (responses.Count == 1 && responses[0].SpinType == 100)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                count++;
                await Task.Delay(500);
                if (count >= 500)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }
        protected virtual async Task<string> sendSpinRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
            });
            HttpResponseMessage message = await httpClient.PostAsync(strURL, postContent);
            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();
            return strContent;
        }
        protected static bool IsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
        protected virtual async Task<List<SpinResponse>> doSpin(HttpClient httpClient)
        {
            List<string>        strResponseHistory  = new List<string>();
            List<SpinResponse>  responseList        = new List<SpinResponse>();
            string strResponse  = "";
            int    spinType     = 0;
            try
            {
                do
                {
                    strResponse     = await sendSpinRequest(httpClient);
                    JToken response = JToken.Parse(strResponse);
                    bool isBreak    = true;
                    if(!IsNullOrEmpty(response["err"]))
                    {
                        return null;
                    }
                    if(!IsNullOrEmpty(response["dt"]["si"]["fs"]))
                    {
                        if (spinType == 0)
                            spinType = 1;

                        int remainSpin = response["dt"]["si"]["fs"]["s"].ToObject<int>();
                        if (remainSpin > 0)
                        {
                            isBreak = false;
                        }
                    }
                    if (!IsNullOrEmpty(response["dt"]["si"]["rwsp"]))
                        isBreak = false;

                    JObject siObj = response["dt"]["si"] as JObject;
                    _strLastID = siObj["sid"].ToString();
                    siObj.Remove("bl");
                    siObj.Remove("blab");
                    siObj.Remove("blb");
                    siObj.Remove("sid");
                    siObj.Remove("psid");

                    string strSpinData = siObj.ToString(Formatting.None);
                    strResponseHistory.Add(strSpinData);
                    if (isBreak)
                    {
                        double totalWin = response["dt"]["si"]["aw"].ToObject<double>();
                        SpinResponse  spinResponse = new SpinResponse();
                        spinResponse.SpinType = spinType;
                        spinResponse.TotalWin = totalWin;
                        spinResponse.Response = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);                       
                        return responseList;
                    }
                } while (true);               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
            }
            return null;

        }
        public void doStop()
        {
            _mustStop = true;
        }

    }
    public enum DoSpinsResults
    {
        USERSTOPPED         = 0,
        NEEDRESTARTSESSION  = 1,
    }
    public class SpinResponse
    {
        public double   TotalWin    { get; set; }
        public double   RealWin     { get; set; }
        public string   NextAction  { get; set; }
        public int      SpinType    { get; set; }
        public string   Response    { get; set; }
        public int      FreeSpinType { get; set; }

    }
    public class GroupSpinResponse
    {
        public List<SpinResponse>   ResponseList { get; set; }
        public string               Response     { get; set; }
        public double               TotalWin     { get; set; }

    }
}
