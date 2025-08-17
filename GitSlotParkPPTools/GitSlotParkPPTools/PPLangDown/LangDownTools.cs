using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PPLangDown
{
    class LangDownTools
    {
        private static LangDownTools    _sInstance  = new LangDownTools();
        public static LangDownTools     Instance    => _sInstance;

        public async Task downloadLogoInfo()
        {
            string url = "https://common-static.pragmaticplay.net/gs2c/common/v4/games-html5/operator_logos/logo_info.js";
            string filePath = "logo_info_utf8.js";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // URL에서 데이터 다운로드
                    byte[] data = await client.GetByteArrayAsync(url);

                    // 인코딩을 UTF-8로 변환
                    string content = Encoding.UTF8.GetString(data);

                    // UTF-8로 파일 저장
                    File.WriteAllText(filePath, content, Encoding.UTF8);

                    Console.WriteLine("다운로드 및 UTF-8 변환 완료! 파일: " + filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("오류 발생: " + ex.Message);
                }
            }
        }
        public async Task downloadData(List<string> gameSymbols)
        {
            int i = 0;
            foreach (string gameSymbol in gameSymbols) {
                //List<string> langs = new List<string>() { "de", "es", "fr", "id", "it", "ko", "pt", "tr", "zh", "uk", "ru", "th" };
                List<string> langs = new List<string>() { "th" };
                if (!downloadLangFiles(gameSymbol, "desktop", langs))
                    Console.WriteLine("error has been occured in desktop {0}", gameSymbol);

                if (!downloadLangFiles(gameSymbol, "mobile", langs))
                    Console.WriteLine("error has been occured in mobile {0}", gameSymbol);
                i++;
            }

            Console.WriteLine("{0} : language donw finished!", i);
            Console.ReadLine();
        }

        private bool downloadLangFiles(string strSymbol, string strPlatform, List<string> langs)
        {
            List<string> defaultFilePaths = new List<string>();
            foreach (string lang in langs)
            {
                string fileName = string.Format("packages/{0}_{1}.json", lang, strPlatform);
                defaultFilePaths.Add(fileName);
                fileName = string.Format("packages/{0}_GUI_{1}.json", lang, strPlatform);
                defaultFilePaths.Add(fileName);
                //string fileName = string.Format("packages/{0}_client.json", lang);
                //defaultFilePaths.Add(fileName);
            }

            for (int i = 0; i < defaultFilePaths.Count; i++)
            {
                downloadFile(strSymbol, strPlatform, defaultFilePaths[i]);
                //if (!downloadFile(strSymbol, strPlatform, defaultFilePaths[i]))
                //    return false;
            }

            Console.WriteLine("{0}-{1} language file download done!", strSymbol, strPlatform);
            return true;
        }

        private bool downloadFile(string strSymbol, string strPlatform, string strPath)
        {
            try
            {
                string strURL = string.Format("https://common-static.pragmaticplay.net/gs2c/common/games-html5/games/vs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                if (strSymbol.StartsWith("c"))
                    strURL = string.Format("https://common-static.pragmaticplay.net/gs2c/common/games-html5/games/cs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                else if (strSymbol == "vs50jfmulthold" || strSymbol == "vs10ddcbells" || strSymbol == "vs20cjcluster" || strSymbol == "vswaysfltdrgny" || strSymbol == "vswaysstampede" || strSymbol == "vswaysexpandng"
                    || strSymbol == "vs20bigdawgs" || strSymbol == "vs20mergedwndw" || strSymbol == "vs20yotdk" || strSymbol == "vswaysfirewmw" || strSymbol == "vs10luckfort" ||
                    strSymbol == "vs20loksriches" || strSymbol == "vs20powerpays" || strSymbol == "vs20treesot" || strSymbol == "vswaysalterego" || strSymbol == "vs10bbfloats" ||
                    strSymbol == "vswaysmegareel" || strSymbol == "vs10strawberry" || strSymbol == "vs20multiup" || strSymbol == "vs20clustext" || strSymbol == "vs20midas2" ||
                    strSymbol == "vs20clustcol" || strSymbol == "vs20crankit" || strSymbol == "vs12scode" || strSymbol == "vs5jjwild" || strSymbol == "vs10dyndigd" || strSymbol == "vs20jhunter" ||
                    strSymbol == "vs20gembondx" || strSymbol == "vs20sulcon" || strSymbol == "vs20dede" || strSymbol == "vs20bblitz" || strSymbol == "vs10dgold88" || strSymbol == "vswayscashconv" ||
                    strSymbol == "vs15godsofwar")
                    strURL = string.Format("https://common-static.pragmaticplay.net/gs2c/common/v3/games-html5/games/vs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                else if (strSymbol == "vs20olympx" || strSymbol == "vs20mmmelon" || strSymbol == "vs10bbbnz" || strSymbol == "vs20stckwldsc" || strSymbol == "vs20sugarrushx"
                    || strSymbol == "vs20portals" || strSymbol == "vs10bburger" || strSymbol == "vs20doghouse2" || strSymbol == "vs40stckwldlvl" || strSymbol == "vs20caramsort" ||
                    strSymbol == "vs20sbpnudge" || strSymbol == "vs10bblotgl" || strSymbol == "vs20stickypos" || strSymbol == "vswaysmegahays" || strSymbol == "vs20lobseafd" ||
                    strSymbol == "vs20bison" || strSymbol == "vs20candybltz2" || strSymbol == "vs20fortbon" || strSymbol == "vs20heartcleo" || strSymbol == "vs10frontrun" ||
                    strSymbol == "vswaysspltsym" || strSymbol == "vswayshexhaus" || strSymbol == "vs20shootstars" || strSymbol == "vswaysjapan" || strSymbol == "vs10bbbrlact" || strSymbol == "vs20fruitswx"
                    || strSymbol == "vs25wildies" || strSymbol == "vs20devilic" || strSymbol == "vs20medusast" || strSymbol == "vs10bbfmission" || strSymbol == "vswaysbkingasc" || strSymbol == "vs5hotbmult"
                    || strSymbol == "vs25badge" || strSymbol == "vs20fourmc" || strSymbol == "vs20clreacts" || strSymbol == "vs5himalaw" || strSymbol == "vs10jokerhot"
                    || strSymbol == "vs10noodles" || strSymbol == "vs40wildrun" || strSymbol == "vs25ultwolgol" || strSymbol == "vs25checaishen" || strSymbol == "vs10fangfree"
                    || strSymbol == "vs10bhallbnza2" || strSymbol == "vs20procountx" || strSymbol == "vswayssevenc" || strSymbol == "vs20dhcluster2" || strSymbol == "vswayskrakenmw"
                    || strSymbol == "vs5luckytig"
                    || strSymbol == "vswayswildbrst" || strSymbol == "vs5wfrog" || strSymbol == "vs5tucanito" || strSymbol == "vs20scbparty" || strSymbol == "vs20swdicex"
                    || strSymbol == "vs25sleepdrag" || strSymbol == "vs20rizkbnz" || strSymbol == "vs9ridelightng" || strSymbol == "vswaysresurich" || strSymbol == "vs1dragon888"
                    || strSymbol == "vs20perusw" || strSymbol == "vswaysmwss" || strSymbol == "vs20mmuertx" || strSymbol == "vs5luckyt1kly" || strSymbol == "vs5luckyphnly"
                    || strSymbol == "vs5luckymly" || strSymbol == "vs5luckydogly" || strSymbol == "vs5jokerjc" || strSymbol == "vs5jellyc" || strSymbol == "vs20hotsake"
                    || strSymbol == "vs20hadex" || strSymbol == "vs25goldrexp" || strSymbol == "vs20olympgold" || strSymbol == "vs20olgamesx" || strSymbol == "vs20fpartydice"
                    || strSymbol == "vs10gbseries" || strSymbol == "vs10emotiwins" || strSymbol == "vs10cndstbnnz" || strSymbol == "vs20fatbook" || strSymbol == "vs4096bufkingh"
                    || strSymbol == "vs10bbbnz1000" || strSymbol == "vs20jjjack" || strSymbol == "vs15eyeofspart" || strSymbol == "vs20wwgcluster")

                    strURL = string.Format("https://common-static.pragmaticplay.net/gs2c/common/v1/games-html5/games/vs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);

                string strFileName = string.Format("client/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                if (File.Exists(strFileName))
                    return true;

                HttpClient client = new HttpClient();
                var response = client.GetAsync(strURL).Result;
                response.EnsureSuccessStatusCode();

                string strFolder = strFileName.Substring(0, strFileName.LastIndexOf("/"));
                Directory.CreateDirectory(strFolder);

                using (var fs = new FileStream(strFileName, FileMode.Create))
                {
                    response.Content.CopyToAsync(fs).Wait();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    class GameData
    {
        public string GameName      { get; set; }
        public string GameSymbol    { get; set; }
        public string GameTitle     { get; set; }
    }
}
