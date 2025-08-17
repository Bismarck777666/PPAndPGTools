using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using GITIGamingWebsite.Models;
using System.Configuration;
using GITIGamingWebsite.Commons;
using System.Security.Cryptography;
using System.Text;

namespace GITIGamingWebsite.Controllers
{
    [RoutePrefix("amatic")]
    public class AmaticGameController : Controller
    {
        private string SOCKETAPIURL     = ConfigurationManager.AppSettings["SOCKETAPIURL"];
        private string TOKENCIPHER      = ConfigurationManager.AppSettings["TOKENCIPHER"];

        ApplicationDbContext db     = new ApplicationDbContext();

        [Route("NotReadyPage")]
        public ActionResult NotReadyPage()
        {
            return View();
        }

        [Route("openGame")]
        public ActionResult OpenGame(int agentid, string userid, string token, string symbol, string currency, string lang, string lobbyurl)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(symbol) || agentid <= 0)
            {
                return RedirectToAction("NotReadyPage");
            }

            string strPassword  = "";
            DateTime expireTime = DateTime.Now;
            if (!deciperInfo(token, ref strPassword, ref expireTime) || expireTime <= DateTime.UtcNow)
            {
                return RedirectToAction("NotReadyPage");
            }

            string session = string.Format("{0}_{1}@{2}", agentid, userid, token);
            ViewBag.session         = session;
            ViewBag.socketurl       = SOCKETAPIURL;
            ViewBag.symbol          = symbol;
            ViewBag.lang            = lang;
            Currencies currencyNum  = CurrencyInfo.Instance.getCurrencyFromText(currency);
            ViewBag.currencytext    = CurrencyInfo.Instance.getCurrencyText(currencyNum);
            ViewBag.currencysymbol  = CurrencyInfo.Instance.getCurrencySymbol(currencyNum);
            ViewBag.currencyrate    = CurrencyInfo.Instance.getCurrencyRate(currencyNum) / 100.0;
            ViewBag.lobbyurl        = lobbyurl;
            ViewBag.error           = "";
            return View();
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
                string strTokenData = decryptString(HttpServerUtility.UrlTokenDecode(strCipherText), TOKENCIPHER);
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
    }
}