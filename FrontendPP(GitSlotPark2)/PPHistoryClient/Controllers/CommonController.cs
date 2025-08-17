using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using NLog;

namespace PGClientDown.Controllers
{
    public class CommonController : Controller
    {
        private static readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public async Task<ActionResult> GetAllResource(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "400");

            //_log.Info($"incoming url : {url}");

            var cacheDir = Server.MapPath("~/wwwroot");
            var localPath = Path.Combine(cacheDir, url.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (System.IO.File.Exists(localPath))
            {
                //_log.Warn($"file exist : {url}");

                var bytes = System.IO.File.ReadAllBytes(localPath);
                var contentType = GetContentType(localPath);
                return File(bytes, contentType);
            }

            HttpClient _httpClient;
            var proxy = new WebProxy
            {

                Address = new Uri(string.Format("http://{0}", "213.182.195.235:50100")),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    userName: "bismarckotp",
                    password: "2VUU8GmnMu")
            };
            
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            handler.Proxy = proxy;

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"134\", \"Not:A-Brand\";v=\"24\", \"Google Chrome\";v=\"134\"");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");

            url = url.Replace("PPGame/", "");
            var remoteUrl = $"https://blackstone-hk1.ppgames.net/gs2c/{url}";
            var response = await _httpClient.GetAsync(remoteUrl);

            _log.Info($"incoming url : {url} Response : {response}");

            if (!response.IsSuccessStatusCode)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "404");

            var contentBytes = await response.Content.ReadAsByteArrayAsync();
            var contentTypeRemote = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

            var dir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);


            System.IO.File.WriteAllBytes(localPath, contentBytes);

            return File(contentBytes, contentTypeRemote);
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            switch (ext.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".svg":
                    return "image/svg+xml";
                case ".js":
                    return "application/javascript";
                case ".css":
                    return "text/css";
                case ".html":
                    return "text/html";
                case ".json":
                    return "application/json";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
