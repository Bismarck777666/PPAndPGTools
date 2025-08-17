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

            _log.Info($"incoming url : {url}");

            var cacheDir = Server.MapPath("~/wwwroot");
            var localPath = Path.Combine(cacheDir, url.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (System.IO.File.Exists(localPath))
            {
                _log.Warn($"file exist : {url}");

                var bytes = System.IO.File.ReadAllBytes(localPath);
                var contentType = GetContentType(localPath);
                return File(bytes, contentType);
            }

            HttpClient _httpClient;
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            _httpClient = new HttpClient(handler);
            //_httpClient.DefaultRequestHeaders.Add("Referer", "https://m.zmcyu9ypy.com/");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"134\", \"Not:A-Brand\";v=\"24\", \"Google Chrome\";v=\"134\"");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");

            //https://nrgs-b2b-stg.gt-cdn.net/ngigs2/bookoframagic/2024-11-29_065528_113.19.0_html5-desktop/com/ngigs2/novosdk/bookoframagic/client/res/game/banner/anim/book/banner_page_sym07_grey/banner_page_sym07_grey_004.png.webp
            //var remoteUrl = $"https://static.zmcyu9ypy.com/{url}";
            var remoteUrl = $"https://nrgs-b2b.greentube.com/{url}{Request.QueryString}";
            //var remoteUrl = $"https://greentube-cdn-games.gt-cdn.net/{url}{Request.QueryString}";
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
                default:
                    return "application/octet-stream";
            }
        }
    }
}
