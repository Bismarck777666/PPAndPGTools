using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using GITIGamingWebsite.Common;
using Org.BouncyCastle.Crypto.Parameters;
using System.Net;

namespace GITIGamingWebsite.Controllers
{
    [RoutePrefix("pp")]
    public class PPGameController : Controller
    {
        private string PUBLICKEY    = ConfigurationManager.AppSettings["PUBLICKEY"];
        private string PRIVATEKEY   = ConfigurationManager.AppSettings["PRIVATEKEY"];
        private string SESSIONKEY0  = ConfigurationManager.AppSettings["SESSIONKEY0"];
        private string SESSIONKEY1  = ConfigurationManager.AppSettings["SESSIONKEY1"];

        public class PPAPIAuthResponse
        {
            public string result        { get; set; }
            public string sessiontoken  { get; set; }
            public string currency      { get; set; }
            public string gamename      { get; set; }
            public string gamedata      { get; set; }
            public PPAPIAuthResponse()
            {
                result          = string.Empty;
                sessiontoken    = string.Empty;
                currency        = string.Empty;
                gamedata        = string.Empty;
                gamename        = string.Empty;
            }
        }

        private string decryptString(byte[] cipherData, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = cipherData;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        private bool deciperInfo(string strCipherText, ref string strPassword, ref DateTime expireTime)
        {
            try
            {
                string strTokenData = decryptString(HttpServerUtility.UrlTokenDecode(strCipherText), "fd973747b689447fb360c89f9612baa8");
                string[] strParts = strTokenData.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length != 2)
                    return false;

                strPassword = strParts[0];
                expireTime = DateTime.ParseExact(strParts[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("openGame")]
        public async Task<ActionResult> OpenGame(int agentid, string userid, string token, string symbol, string lang, string lobbyurl)
        {
            if (agentid <= 0 || string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(symbol))
            {
                ViewBag.error = "Access Denied";
                return View();
            }
            string strPassword = "";
            DateTime expireTime = DateTime.Now;
            if (!deciperInfo(token, ref strPassword, ref expireTime) || expireTime <= DateTime.UtcNow)
            {
                ViewBag.error = "Access Denied1";
                return View();
            }

            string strDomain              = getRootDomain();
            //string strAPIURL              = string.Format("https://server.{0}", strDomain);
            string strAPIURL              = string.Format("http://192.168.88.34:8888", strDomain);
            PPAPIAuthResponse apiResponse = await callAuthTokenRequest(agentid, userid, strPassword, symbol, strAPIURL);
            if (apiResponse.result != "success")
            {
                ViewBag.error = string.Format("Access Denied2 {0}", strAPIURL);
                return View();
            }
            ViewBag.gametitle       = apiResponse.gamename;
            ViewBag.session         = apiResponse.sessiontoken;
            //ViewBag.baseurl         = string.Format("https://igbsoft.{0}", strDomain);
            //ViewBag.apiurl          = string.Format("https://server.{0}",  strDomain);
            //ViewBag.replayurl       = string.Format("https://replay.{0}",  strDomain);

            //ViewBag.baseurl         = string.Format("https://58ee05c640.{0}", strDomain);
            //ViewBag.apiurl          = string.Format("https://server.{0}",  strDomain);
            //ViewBag.replayurl       = string.Format("https://58ee05c641.{0}",  strDomain);

            ViewBag.baseurl         = string.Format("http://localhost:56180",   strDomain);
            ViewBag.apiurl          = string.Format("http://192.168.88.34:8888",strDomain);
            //ViewBag.apiurl          = string.Format("http://localhost:8888",strDomain);
            ViewBag.replayurl       = string.Format("http://localhost:56180",   strDomain);
            
            ViewBag.symbol          = symbol;
            ViewBag.supportreplay   = int.Parse(apiResponse.gamedata);
            if (string.IsNullOrEmpty(lang))
                ViewBag.lang = "en";
            else
                ViewBag.lang = lang;
            ViewBag.currency        = apiResponse.currency;
            ViewBag.error           = "";

            SessionKeyData sessionKeyData = new SessionKeyData();
            sessionKeyData.serverSeconds    = getCurrentUnixTimestamp();
            //sessionKeyData.serverHostname   = new Uri(string.Format("https://igbsoft.{0}", strDomain)).Host;
            //sessionKeyData.referer          = string.Format("https://igbsoft.{0}", strDomain);

            //sessionKeyData.serverHostname   = new Uri(string.Format("https://58ee05c640.{0}", strDomain)).Host;
            //sessionKeyData.referer          = string.Format("https://58ee05c640.{0}", strDomain);

            sessionKeyData.serverHostname   = new Uri(string.Format("http://localhost:56180", strDomain)).Host;
            sessionKeyData.referer          = string.Format("http://localhost:56180", strDomain);
            sessionKeyData.userAgent        = Request.UserAgent;

            ViewBag.sessionkey2     = generateSessionKey2(sessionKeyData);
            ViewBag.RSAPublicKey    = PUBLICKEY;
            ViewBag.RSASessionKey   = generateRSASessionKey(sessionKeyData);
            ViewBag.lobbyurl        = string.IsNullOrEmpty(lobbyurl) ? "" : lobbyurl;
            return View();
        }

        private string getRootDomain()
        {
            string strDomain = Request.Url.Authority;
            List<string> domainParts = new List<string>(strDomain.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries));
            domainParts.RemoveAt(0);
            strDomain = string.Join(".", domainParts.ToArray());
            int position = strDomain.LastIndexOf(":");
            if (position > 0)
                strDomain = strDomain.Substring(0, position);
            return strDomain;
        }
        private string generateSessionKey2(SessionKeyData sessionKeyData)
        {
            long timeStamp      = getCurrentUnixTimestamp();
            AES aesCrypt        = new AES();
            byte[] secretKey    = aesCrypt.convertBase64ToByteArray(SESSIONKEY0);
            byte[] secretIV     = aesCrypt.convertBase64ToByteArray(SESSIONKEY1);

            string sessionKey2  = aesCrypt.doEncrypt(JsonConvert.SerializeObject(sessionKeyData), secretKey, secretIV);
            return sessionKey2;
        }
        private string generateRSASessionKey(SessionKeyData sessionKeyData)
        {
            try
            {
                string sessionKeyJsonStr = JsonConvert.SerializeObject(sessionKeyData);

                List<string> strParts = new List<string>();
                while (!string.Equals(sessionKeyJsonStr, ""))
                {
                    string str = "";
                    if (sessionKeyJsonStr.Length > 117)
                    {
                        str = sessionKeyJsonStr.Substring(0, 117);
                        sessionKeyJsonStr = sessionKeyJsonStr.Substring(117);
                    }
                    else
                    {
                        str = sessionKeyJsonStr;
                        sessionKeyJsonStr = "";
                    }
                    strParts.Add(str);
                }

                List<string> rsaResults = new List<string>();
                RSAKeys rsaCryptor = new RSAKeys();

                for (int i = 0; i < strParts.Count; i++)
                {
                    RsaPrivateCrtKeyParameters privateKey = rsaCryptor.ImportPrivateKey(PRIVATEKEY);
                    byte[] encryptByte = rsaCryptor.EncryptData(privateKey, strParts[i]);
                    string resultText = Convert.ToBase64String(encryptByte);
                    rsaResults.Add(resultText);
                }

                return string.Join(":", rsaResults.ToArray());
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        private long getCurrentUnixTimestamp()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        [Route("replayGame")]
        public ActionResult ReplayGame(string token, string symbol, int envID, long roundID, string lang, string currency)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(symbol) || roundID == 0 || envID == 0)
            {
                ViewBag.error = "Access Denied";
                return View();
            }
            string strDomain = getRootDomain();

            ViewBag.session  = token;
            //ViewBag.baseurl  = string.Format("https://igbsoft.{0}", strDomain);
            //ViewBag.baseurl  = string.Format("https://58ee05c640.{0}", strDomain);
            //ViewBag.apiurl   = string.Format("https://server.{0}",  strDomain);
            ViewBag.baseurl  = string.Format("http://localhost:56180",   strDomain);
            ViewBag.apiurl   = string.Format("http://192.168.88.34:8888",strDomain);
            ViewBag.symbol   = symbol;
            ViewBag.envID    = envID;
            ViewBag.roundID  = roundID;
            ViewBag.lang     = lang;
            ViewBag.currency = currency;
            ViewBag.error    = "";
            return View();
        }
        
        [Route("lastGameHistory")]
        public ActionResult LastGameHistory(string mgckey, string symbol, string lang)
        {
            if (string.IsNullOrEmpty(mgckey))
                return RedirectToAction("NotLoggedInPage");

            string[] tokenArray = mgckey.Split('@');

            if(tokenArray.Length < 2)
                return RedirectToAction("NotLoggedInPage");

            string strDomain        = getRootDomain();
            ViewBag.session         = mgckey;
            //ViewBag.apiurl          = string.Format("https://server.{0}", strDomain);
            ViewBag.apiurl          = string.Format("http://{0}", "192.168.88.34:8888");
            ViewBag.symbol          = symbol;
            ViewBag.lang            = lang;
            ViewBag.error           = "";
            return View();
        }
        
        [Route("NotReadyPage")]
        public ActionResult NotReadyPage()
        {
            return View();
        }
        
        [Route("NotLoggedInPage")]
        public ActionResult NotLoggedInPage()
        {
            return View();
        }
        private async Task<PPAPIAuthResponse> callAuthTokenRequest(int agentID, string strUserID, string strPassword, string symbol, string apiURL)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                httpClient.Timeout    = TimeSpan.FromMinutes(10);
                string tokenURL       = string.Format("{0}/gitapi/pp/service/auth.do?agentid={1}&userid={2}&password={3}&symbol={4}", apiURL, agentID, strUserID, strPassword, symbol);

                HttpResponseMessage response = await httpClient.GetAsync(tokenURL);
                response.EnsureSuccessStatusCode();

                string              strResponse = await response.Content.ReadAsStringAsync();
                PPAPIAuthResponse   apiResponse = JsonConvert.DeserializeObject<PPAPIAuthResponse>(strResponse);
                return apiResponse;
            }
            catch(Exception ex)
            {
                return new PPAPIAuthResponse();
            }
        }        
    }
    public class SessionKeyData
    {
        public string   referer             { get; set; }
        public string   serverHostname      { get; set; }
        public long     serverSeconds       { get; set; }
        public string   userAgent           { get; set; }
    }
}