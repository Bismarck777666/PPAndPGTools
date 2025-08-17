using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetMoneyFetcher
{
    internal class FetchWorker
    {
        private static FetchWorker  _sInstance  = new FetchWorker();
        public static FetchWorker   Instance    => _sInstance;
        private async Task<Dictionary<string, PPGameBetMoneyInfo>> loadMoneyInfoFile(string strCurrency)
        {
            string strFileName = string.Format("chipset({0}).info", strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> oldBetInfos = new Dictionary<string, PPGameBetMoneyInfo>();
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.OpenRead(strFileName)))
                {
                    int unavailableCount = binReader.ReadInt32();
                    for (int i = 0; i < unavailableCount; i++)
                    {
                        string strSymbol = (string)binReader.ReadString();
                        oldBetInfos[strSymbol] = null;
                    }
                    int availableCount = binReader.ReadInt32();
                    for (int i = 0; i < availableCount; i++)
                    {
                        string strSymbol = binReader.ReadString();
                        PPGameBetMoneyInfo gameMoneyInfo = new PPGameBetMoneyInfo();
                        gameMoneyInfo.sc            = binReader.ReadString();
                        gameMoneyInfo.defc          = binReader.ReadString();
                        gameMoneyInfo.totalBetMin   = binReader.ReadString();
                        gameMoneyInfo.totalBetMax   = binReader.ReadString();
                        gameMoneyInfo.lineCount     = binReader.ReadInt32();    
                        oldBetInfos[strSymbol]      = gameMoneyInfo;
                    }
                }
            }
            catch
            {

            }
            return oldBetInfos;
        }
        protected Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        public async Task fetchRTPInfo(string strCurrency)
        {
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, PPGameRTPInfo> dicRTPInfos = new Dictionary<string, PPGameRTPInfo>();
            string strFileName = "gamertp.info";
            using (BinaryReader binReader = new BinaryReader(File.Open(strFileName, FileMode.Open)))
            {

                int count = binReader.ReadInt32();
                for(int i = 0; i < count; i++)
                {
                    string strSymbol = binReader.ReadString();
                    string rtp       = binReader.ReadString();
                    string gameInfo  = binReader.ReadString();

                    var rtpInfo         = new PPGameRTPInfo();
                    rtpInfo.rtp         = rtp;
                    rtpInfo.gameInfo    = gameInfo;
                    dicRTPInfos.Add(strSymbol, rtpInfo);
                }
            }
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (dicRTPInfos.ContainsKey(strSymbols[i]))
                    continue;

                //string strLaunchURL = string.Format("https://mlsn5tup3f.mcnwfkxdbd.net/gs2c/playGame.do?key=token%3D2763-a6f8f39e13c94419a8c1ac77a7914627-CIS%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DTND%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Fkoora365.org&ppkv=2&stylename=qg_qtechgamesrow&isGameUrlApiCalled=true", strSymbols[i]);
                string strLaunchURL     = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?lang=en&cur=USD&gameSymbol={0}", strSymbols[i]);
                PPGameInfo gameInfo     = await getPPGameInfo(strLaunchURL, strSymbols[i]);
                if (gameInfo == null)
                    continue;
                try
                {
                    string strInitString = await getInitString(gameInfo, strSymbols[i], strCurrency);
                    var dicParams = splitResponseToParams(strInitString);
                    PPGameRTPInfo rtpInfo = new PPGameRTPInfo();
                    if (dicParams.ContainsKey("rtp"))
                        rtpInfo.rtp = dicParams["rtp"];
                    if (dicParams.ContainsKey("gameInfo"))
                        rtpInfo.gameInfo = dicParams["gameInfo"];

                    dicRTPInfos[strSymbols[i]] = rtpInfo;
                }
                catch (Exception ex)
                {

                }
            }
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(dicRTPInfos.Count);
                foreach (KeyValuePair<string, PPGameRTPInfo> pair in dicRTPInfos)
                {
                    binWriter.Write(pair.Key);
                    if (string.IsNullOrEmpty(pair.Value.rtp))
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.rtp);

                    if (string.IsNullOrEmpty(pair.Value.gameInfo))
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.gameInfo);
                }
            }

            Console.WriteLine("GameRTP fetcher has been finished!");
        }
        public async Task fetchBetMoneyInfo(string strCurrency, double unit)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string      strSymbolString  = File.ReadAllText("gamesymbols.txt");
            string[]    strSymbols      = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for(int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos    = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }
                        else
                        {
                            betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                            continue;
                        }
                    }
                    string strLaunchURL = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?lang=en&cur={1}&gameSymbol={0}", strSymbol, "USD");

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        Console.WriteLine("{0} $$$ {1}: $$$$$$$ unavailable!", strCurrency, strSymbol);
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "USD");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc             = findStringBetween(strInitString, "&sc=", "&");
                    info.defc           = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax    = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin    = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount      = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));
                    
                    if(info.sc == null)
                    {
                        Console.WriteLine("{0} $$$ {1}: $$$$$$$ unavailable sc!", strCurrency, strSymbol);
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    int multiplier = (int)(unit / 0.01);

                    if(!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax) * multiplier);
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin) * multiplier);
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc) * multiplier);

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]) * multiplier;
                            //if (strCurrency == "EUR" && sc * info.lineCount > 20)
                            //    break;

                            scList.Add(sc);
                        }

                        if (strCurrency == "TRY" && scList.Last() * info.lineCount < 50000)
                        {
                            if(info.lineCount > 243)
                                scList.Add(2500);
                            else
                                scList.Add((int)(50000 / info.lineCount));
                        }

                        if(strCurrency == "ARS" && info.lineCount < 20)
                        {
                            double additionalMutil = 10 / (scList.First() * info.lineCount);    
                            if(additionalMutil >= 2)
                            {
                                for (int i = 0; i < scList.Count; i++)
                                {
                                    scList[i] = Convert.ToDouble(scList[i]) * additionalMutil;
                                }

                                if (!string.IsNullOrEmpty(info.totalBetMax))
                                {
                                    info.totalBetMax = info.totalBetMax.Replace(",", "");
                                    info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax) * additionalMutil);
                                }

                                if (!string.IsNullOrEmpty(info.totalBetMin))
                                {
                                    info.totalBetMin = info.totalBetMin.Replace(",", "");
                                    info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin) * additionalMutil);
                                }

                                if (info.defc != null)
                                    info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc) * additionalMutil);
                            }
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;

                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }
            //return;

            string strFileName = string.Format("chipset({0}).info", strCurrency); 
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    try
                    {
                        binWriter.Write(pair.Key);
                        binWriter.Write(pair.Value.sc);
                        if (pair.Value.defc == null)
                            binWriter.Write("");
                        else
                            binWriter.Write(pair.Value.defc);

                        binWriter.Write(pair.Value.totalBetMin);
                        binWriter.Write(pair.Value.totalBetMax);
                        binWriter.Write(pair.Value.lineCount);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchTRYBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string      strSymbolString  = File.ReadAllText("gamesymbols.txt");
            string[]    strSymbols      = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for(int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos    = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://0f17a7d410.mzyfmuskwr.net/gs2c/playGame.do?key=token%3D52eb42be-2730-449b-bbf8-270c3a3e5199%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DTRY%60%7C%60cashierUrl%3Dhttps%3A%2F%2Fsekabet1297.com%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Fsekabet1297.com&ppkv=2&stylename=skbt_sekabet&country=TR&isGameUrlApiCalled=true", strSymbol);
                    
                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "TRY");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc             = findStringBetween(strInitString, "&sc=", "&");
                    info.defc           = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax    = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin    = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount      = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));
                    
                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if(!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency); 
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach(KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if(pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task<string> getMegafonLaunchURL(string strSymbol)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string lunchPayLoad = "{\"brandId\":\"pragmatic_PM\",\"gameId\":\"6565156d8714414bb231b01a\",\"image\":\"https://test1.prerelease-env.biz/game_pic/rec/325/vs25goldparty.png\",\"integrationProvider\":\"PM\",\"name\":\"Gold Party\",\"providerId\":\"pragmatic\",\"status\":true,\"tags\":[\"Treasures\",\"Adventure\"],\"token\":\"IPL28hs2JVzreim86194-jugar\",\"playerId\":\"277951_86194\",\"userId\":\"655d4ae63ad36797f65fabad\",\"currency\":\"ARS\",\"language\":\"es\"}";
                HttpResponseMessage response = await httpClient.PostAsync("https://casino2.jcasino.live/v1/casino/launch_game", new StringContent(lunchPayLoad, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string strContent = await response.Content.ReadAsStringAsync();
                dynamic lunchRes = JsonConvert.DeserializeObject<dynamic>(strContent);

                if ((string)lunchRes["result"] == "success")
                {
                    string strURL = lunchRes["message"];
                    return strURL.Replace("vs25goldparty", strSymbol);
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public async Task fetchARSBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    //string strLaunchURL = string.Format("https://0abe8e5ef2.dxrtmhaorj.net/gs2c/playGame.do?key=token%3D66f67b5e9bbe96d54da85735%60%7C%60symbol%3D{0}%60%7C%60language%3Des%60%7C%60currency%3DARS&ppkv=2&stylename=betsw3_betsw3&promo=y&treq=WoVRXoHmTpdlWlmpEE6PIzuF4wc4SHuY7VRfxhZ6zVvAtekf7m5cOop6jFfFyphT&isGameUrlApiCalled=true&userId=277951_86194", strSymbol);


                    string strLaunchURL = await getMegafonLaunchURL(strSymbol);
                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "ARS");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchPHPBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://1103custom.bbixootp.click/gs2c/playGame.do?key=token%3D06bab31b8d0748558295467907ce4a7f%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DPHP%60%7C%60cashierUrl%3Dhttps%3A%2F%2F1xbet.com%2Foffice%2Frecharge%2F%60%7C%60lobbyUrl%3Dhttps%3A%2F%2F1xbet.com%2Fslots%2Fthankyou&ppkv=2&stylename=1xbet_sw&rcCloseUrl=https://1xbet.com/slots/thankyou&isGameUrlApiCalled=true", strSymbol);
                    strLaunchURL = string.Format("https://1103custom.bbixootp.click/gs2c/playGame.do?key=token%3D20ac5f66d24145c28c12c17f80cfa125%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DPHP%60%7C%60cashierUrl%3Dhttps%3A%2F%2F1xsinga.com%2Foffice%2Frecharge%2F%60%7C%60lobbyUrl%3Dhttps%3A%2F%2F1xsinga.com%2Fslots%2Fthankyou&ppkv=2&stylename=1xbet_sw&rcCloseUrl=https://1xsinga.com/slots/thankyou&isGameUrlApiCalled=true&userId=1069885451_PHP", strSymbol);

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "PHP");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchDZDBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://1103custom.bbixootp.click/gs2c/playGame.do?key=token%3D20ac5f66d24145c28c12c17f80cfa125%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DPHP%60%7C%60cashierUrl%3Dhttps%3A%2F%2F1xsinga.com%2Foffice%2Frecharge%2F%60%7C%60lobbyUrl%3Dhttps%3A%2F%2F1xsinga.com%2Fslots%2Fthankyou&ppkv=2&stylename=1xbet_sw&rcCloseUrl=https://1xsinga.com/slots/thankyou&isGameUrlApiCalled=true&userId=1069885451_PHP", strSymbol);

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "DZD");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax) * 10);
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin) * 10);
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc) * 10);

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]) * 10;
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchNewEURBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://5cb650433a.icsobdjtjn.net/gs2c/playGame.do?key=token%3D31dc62b4-d1d7-4383-a08d-e762af4afcb2%60%7C%60symbol%3D{0}%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DUSD%60%7C%60cashierUrl%3Dhttps%3A%2F%2Fstake1022.com%2Fdeposit%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Fstake1022.com%2Fcasino%2Fhome&ppkv=2&stylename=rare_stake&country=RU&isGameUrlApiCalled=true&userId=32cf4903-29ce-4198-8a79-b1306fafe43a", strSymbol);

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "PHP");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchNewLBPBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://mlsn5tup3f.mcnwfktbdb.net/gs2c/playGame.do?key=token=sgh4E4ndZ7hmgdEYJUE3TFloV635seAsf5HuFwDZSuj6`|`symbol={0}`|`language=en`|`lobbyUrl=https://colorbet.xyz/en/casino?selectedProvider=pragmatic&ppkv=2&stylename=tmlss_eulink2&isGameUrlApiCalled=true&userId=8275063WT824cf0ed", strSymbol);

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "PHP");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        public async Task fetchNewMMKBetMoneyInfo(string strCurrency)
        {
            Console.WriteLine("{0} :----------started!", strCurrency);
            string strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[] strSymbols = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for (int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach (string strSymbol in strSymbols)
            {
                try
                {
                    if (oldBetMoneyInfos.ContainsKey(strSymbol))
                    {
                        if (oldBetMoneyInfos[strSymbol] == null)
                        {
                            notAvailableGames.Add(strSymbol);
                            continue;
                        }

                        betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                        continue;
                    }
                    string strLaunchURL = string.Format("https://sg67custom.klrogkok.click/gs2c/playGame.do?key=token%3Dc3b8a899-0e65-11f0-906b-13dd9e83b16d-MTM2MTcwZ2Q3N3Rlc3Q%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DMMK%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Ffsa.velkigames365.cc%2Fplayer%2Fgamehall%2F%3Fsess%3Dc3b8a899-0e65-11f0-906b-13dd9e83b16d-MTM2MTcwZ2Q3N3Rlc3Q%26origin%3D%2Fplatform%2FPP_tabType%3DSLOT&ppkv=2&stylename=nttech_aws06&country=RU&promo=n&isGameUrlApiCalled=true&userId=136170gd77test", strSymbol);

                    PPGameInfo gameInfo = await getPPGameInfo(strLaunchURL, strSymbol);
                    if (gameInfo == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }
                    string strInitString = await getInitString(gameInfo, strSymbol, "PHP");

                    PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                    info.sc = findStringBetween(strInitString, "&sc=", "&");
                    info.defc = findStringBetween(strInitString, "&defc=", "&");
                    info.totalBetMax = findStringBetween(strInitString, "&total_bet_max=", "&");
                    info.totalBetMin = findStringBetween(strInitString, "&total_bet_min=", "&");
                    info.lineCount = Convert.ToInt32(findStringBetween(strInitString, "&l=", "&"));

                    if (info.totalBetMax == null)
                        info.totalBetMax = "";
                    if (info.totalBetMin == null)
                        info.totalBetMin = "";

                    if (!string.IsNullOrEmpty(info.totalBetMax))
                    {
                        info.totalBetMax = info.totalBetMax.Replace(",", "");
                        info.totalBetMax = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMax));
                    }

                    if (!string.IsNullOrEmpty(info.totalBetMin))
                    {
                        info.totalBetMin = info.totalBetMin.Replace(",", "");
                        info.totalBetMin = string.Format("{0:0.00}", Convert.ToDouble(info.totalBetMin));
                    }

                    if (info.defc != null)
                        info.defc = string.Format("{0:0.00}", Convert.ToDouble(info.defc));

                    if (info.sc != null)
                    {
                        string[] sces = info.sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        List<double> scList = new List<double>();
                        for (int i = 0; i < sces.Length; i++)
                        {
                            double sc = Convert.ToDouble(sces[i]);
                            scList.Add(sc);
                        }

                        string[] newSces = new string[scList.Count];
                        for (int i = 0; i < scList.Count; i++)
                            newSces[i] = string.Format("{0:0.00}", scList[i]);

                        info.sc = string.Join(",", newSces);
                    }

                    betMoneyInfos[strSymbol] = info;
                    Console.WriteLine("{0} *** {1}: ***** done!", strCurrency, strSymbol);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, :{1}", ex.Message, strSymbol);
                    notAvailableGames.Add(strSymbol);
                }
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach (KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if (pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                    binWriter.Write(pair.Value.lineCount);
                }
            }

            Console.WriteLine("{0} :---------- finshed!", strCurrency);
        }
        private string findStringBetween(string strContent, string strStart, string strEnd)
        {
            int startIndex = strContent.IndexOf(strStart);
            if (startIndex < 0)
                return null;

            startIndex += strStart.Length;
            int endIndex = strContent.IndexOf(strEnd, startIndex);
            if (endIndex < 0)
                return strContent.Substring(startIndex);
            else
                return strContent.Substring(startIndex, endIndex - startIndex);
        }
        private async Task<PPGameInfo> getPPGameInfo(string strURL, string strGameSymbol)
        {
            do
            {
                string strContent = "";
                try
                {
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://arg.1x-bet.com/");
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "iframe");
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-site", "cross-site");
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
                    
                    
                    //httpClient.DefaultRequestHeaders.Remove("Cookie");

                    HttpResponseMessage response = await httpClient.GetAsync(strURL);
                    response.EnsureSuccessStatusCode();

                    strContent = await response.Content.ReadAsStringAsync();
                    string strRedirectURL = response.RequestMessage.RequestUri.ToString();
                    string strGameConfig = findStringBetween(strContent, "gameConfig: '", "'");

                    JToken gameConfig = JToken.Parse(strGameConfig);
                    PPGameInfo info = new PPGameInfo();
                    info.GameServiceURL = gameConfig["gameService"].ToString();
                    info.Token = gameConfig["mgckey"].ToString();
                    if (gameConfig["replaySystemUrl"] == null)
                        info.IsReplaySupported = false;
                    else
                        info.IsReplaySupported = true;
                    info.DataPath = gameConfig["datapath"].ToString();
                    string strBootstrapURL = string.Format("{0}desktop/bootstrap.js", info.DataPath);
                    response = await httpClient.GetAsync(strBootstrapURL);
                    response.EnsureSuccessStatusCode();

                    strContent = await response.Content.ReadAsStringAsync();
                    string strUHTRevision = findStringBetween(strContent, ";UHT_REVISION={", "}");
                    info.CVer = findStringBetween(strUHTRevision, "desktop:'", "'");
                    info.NewSymbol = findStringBetween(strContent, "UHT_CONFIG.SYMBOL='", "'");
                    return info;
                }
                catch (Exception ex)
                {
                    if (strGameSymbol == "vs75bronco")
                        return null;

                    if (!strContent.Contains("Sorry") && !strContent.Contains("Internal Service Error"))
                    {
                        await Task.Delay(500);
                        continue;
                    }
                    else
                        return null;
                }
            } while (true);

        }
        private async Task<string> getInitString(PPGameInfo gameInfo, string strSymbol, string strCurrency)
        {
            HttpClient httpClient = new HttpClient();
            if (strSymbol != gameInfo.NewSymbol)
                strSymbol = gameInfo.NewSymbol;

            KeyValuePair<string, string>[] postParams = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("action",  "doInit"),
                new KeyValuePair<string, string>("symbol",  strSymbol),
                new KeyValuePair<string, string>("cver",    gameInfo.CVer),
                new KeyValuePair<string, string>("index",   "1"),
                new KeyValuePair<string, string>("counter", "1"),
                new KeyValuePair<string, string>("repeat",  "0"),
                new KeyValuePair<string, string>("mgckey",  gameInfo.Token),
            };
            if(strCurrency == "KRW")
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin",  "https://modoogames-sg13.ppgames.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://modoogames-sg13.ppgames.net");
            }
            else if(strCurrency == "IDR")
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://bcgame-dk2.pragmaticplay.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://bcgame-dk2.pragmaticplay.net/gs2c/html5Game.do?jackpotid=0&gname=Starlight%20Christmas&extGame=1&ext=0&cb_target=exist_tab&symbol=vs20schristmas&jurisdictionID=99&lobbyUrl=https%253A%252F%252Fbc.fun%252Fcasino&cashierUrl=https%253A%252F%252Fbc.fun%252Fcasino%2523%252Fwallet%252Fdeposit%252FCHAIN&minimode=0&minilobby=true&mgckey=AUTHTOKEN@e1b757f40148103562d594a4b87c0afe0db698ce2e5081b2c22f7efe8130037b~stylename@bcgame_bcgame~SESSION@78abb0e9-51da-4061-8263-fd35debd0a79~SN@e4ca05e4&tabName=\r\n");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0");
            }
            else
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin",  "https://demogamesfree.pragmaticplay.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://demogamesfree.pragmaticplay.net");
            }
            var response = await httpClient.PostAsync(gameInfo.GameServiceURL, new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
    class PPGameInfo
    {
        public string   GameServiceURL      { get; set; }
        public string   Token               { get; set; }
        public bool     IsReplaySupported   { get; set; }
        public string   CVer                { get; set; }
        public string   DataPath            { get; set; }
        public int      ServerLineCount     { get; set; }
        public int      Rows                { get; set; }
        public int      PurchaseMultiple    { get; set; }
        public double   AnteBetMultiple     { get; set; }
        public string   InitString          { get; set; }
        public string   NewSymbol           { get; set; }
        public Dictionary<string, string> AdditionalInitParams { get; set; }

        public PPGameInfo()
        {
            this.AdditionalInitParams = new Dictionary<string, string>();
        }
    }
    class PPGameRTPInfo
    {
        public string rtp       { get; set; }
        public string gameInfo  { get; set; }
    }
    class PPGameBetMoneyInfo
    {
        public string   sc              { get; set; }
        public string   defc            { get; set; }
        public string   totalBetMax     { get; set; }
        public string   totalBetMin     { get; set; }
        public int      lineCount       { get; set; }
    }
}
