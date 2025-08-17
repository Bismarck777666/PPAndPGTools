using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace PPHistoryIconsDownloader
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //DirectoryInfo dirInfo = new DirectoryInfo("C:\\PragmaticPlay\\ClientPublish\\PPGame\\common\\game-history-client");
            //walkDirectoryTree(dirInfo);

            downloadImages();
            //downloadHistoryLanguages();
        }

        static void walkDirectoryTree(DirectoryInfo  root)
        {
            FileInfo[] files = null;
            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
            }
            if(files != null)
            {
                foreach (FileInfo fi in files)
                {
                    procFileInfo(fi);
                }
            }
            DirectoryInfo[]  subDirs = root.GetDirectories();
            foreach (DirectoryInfo dirInfo in subDirs)
                walkDirectoryTree(dirInfo);
        }
        static void procFileInfo(FileInfo fileInfo)
        {
            try
            {
                if (fileInfo.Extension != ".png")
                    return;

                string strFilePath = fileInfo.FullName;
                int index = strFilePath.IndexOf("game-history-client");
                if (index < 0)
                    return;

                if (strFilePath.EndsWith("@2x.png"))
                    return;

                string strURL = strFilePath.Substring(index).Replace("\\", "/");
                strURL = "https://common-static-aka.pragmaticplay.net/gs2c/common/" + strURL;
                strURL = strURL.Substring(0, strURL.Length - 4) + "@2x.png";
                strFilePath = strFilePath.Substring(0, strFilePath.Length - 4) + "@2x.png";

                if (!File.Exists(strFilePath))
                    downloadFile(strURL, strFilePath);
            }
            catch
            {

            }
        }

        private static void downloadFile(string strURL, string strFileName)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetAsync(strURL).Result;
                response.EnsureSuccessStatusCode();

                string strFolder1 = strFileName.Substring(0, strFileName.LastIndexOf("\\"));
                string strFolder2 = strFolder1.Substring(0, strFileName.LastIndexOf("\\"));
                Directory.CreateDirectory(strFolder2);
                Directory.CreateDirectory(strFolder1);

                using (var fs = new FileStream(strFileName, FileMode.Create))
                {
                    response.Content.CopyToAsync(fs).Wait();
                }
                Console.WriteLine(strFileName);
            }
            catch (Exception ex)
            {

            }
        }

        static void downloadImages()
        {
            List<string> symbols = new List<string>()
            {
                //"vs10bbbbrnd",
                //"vs10diamondrgh",
                //"vs10fingerlfs",
                //"vs25pfarmfp",
                //"vs25goldpartya",
                //"vs20gemfirefor",
                //"vs20bigmass",
                //"vs20gtsofhades",
                //"vs12trpcnhour",
                //"vswaysaztecgm",
                //"vs20alieninv",
                //"vswaysolwfp",
                //"vswaysmjwl",
                //"vs20swbonsup",
                //"vs10chillihmr",
                //"vs1mjokfp",
                //"vs10piggybank",
                //"vs9gemtrio",
                //"vswayspompmr2",
                //"vswaysargonts",
                //"vs10bbrrp",
            };

            foreach (string symbol in symbols)
            {
                string strDirect = string.Format("images/{0}/spin", symbol);
                if (!System.IO.Directory.Exists(@strDirect))
                {
                    System.IO.Directory.CreateDirectory(@strDirect);
                }
                for (int i = 1; i < 20; i++)
                {
                    string strFileName = string.Format("images/{0}/spin/{1}.png", symbol, i);
                    string strUrl = string.Format("https://blackstone-hk1.ppgames.net/gs2c/common/game-history-client/images/{0}/spin/{1}.png", symbol, i);

                    if (URLExists(strUrl))
                    {
                        try
                        {
                            HttpClient client = new HttpClient();
                            var response = client.GetAsync(strUrl).Result;
                            response.EnsureSuccessStatusCode();

                            using (var fs = new FileStream(strFileName, FileMode.Create))
                            {
                                response.Content.CopyToAsync(fs).Wait();
                            }
                            Console.WriteLine(strFileName);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    strFileName = string.Format("images/{0}/spin/{1}@2x.png", symbol, i);
                    strUrl      = string.Format("https://blackstone-hk1.ppgames.net/gs2c/common/game-history-client/images/{0}/spin/{1}@2x.png", symbol, i);

                    if (URLExists(strUrl))
                    {
                        try
                        {
                            HttpClient client = new HttpClient();
                            var response = client.GetAsync(strUrl).Result;
                            response.EnsureSuccessStatusCode();

                            using (var fs = new FileStream(strFileName, FileMode.Create))
                            {
                                response.Content.CopyToAsync(fs).Wait();
                            }
                            Console.WriteLine(strFileName);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }
            }
        }

        static void downloadHistoryLanguages()
        {
            List<string> list = new List<string>() { "de", "en", "es", "fr", "id", "it", "ko", "pt", "ru", "th", "tr", "uk", "zh" };

            if (!System.IO.Directory.Exists(@"i18n"))
                System.IO.Directory.CreateDirectory(@"i18n");
            
            foreach (string lang in list)
            {
                string strFileName = string.Format("i18n/{0}.json", lang);
                string strUrl = string.Format("https://blackstone-hk1.ppgames.net/gs2c/common/game-history-client/i18n/{0}.json", lang);

                if (URLExists(strUrl))
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        var response = client.GetAsync(strUrl).Result;
                        response.EnsureSuccessStatusCode();

                        using (var fs = new FileStream(strFileName, FileMode.Create))
                        {
                            response.Content.CopyToAsync(fs).Wait();
                        }
                        Console.WriteLine(strFileName);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private static bool URLExists(string url)
        {
            bool result = false;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 2000; // miliseconds
            webRequest.Method = "HEAD";

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch (WebException webException)
            {
                Console.WriteLine(url + " doesn't exist: " + webException.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }
    }
}
