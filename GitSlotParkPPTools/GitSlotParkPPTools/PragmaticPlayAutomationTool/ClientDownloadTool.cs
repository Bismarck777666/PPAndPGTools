using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.ComponentModel;

namespace PragmaticPlayAutomationTool
{
    class ClientDownloadTool
    {
        private static ClientDownloadTool   _sInstance  = new ClientDownloadTool();
        public static ClientDownloadTool    Instance    => _sInstance;

        public async Task downloadData(List<GameData> gameDatas)
        {
            //Dictionary<string, string> gameIds = await SlotDiamondAPI.Instance.getGameList();
            for (int i = 0; i < gameDatas.Count; i++)
            {
                bool result1 = await procNonAsiaGame(gameDatas[i]);
                if (!result1)
                    //i--;
                    Console.WriteLine("Exception has been occured in gamesymbol : {0}", gameDatas[i].GameSymbol);
                continue;

                //if (gameIds.ContainsKey(gameDatas[i].GameSymbol))
                //{
                //    var gameId = gameIds[gameDatas[i].GameSymbol];
                //    bool result = await procAsiaGame(gameDatas[i], gameId);
                //    if (!result)
                //        i--;
                //}
                //else
                //{
                //    bool result = await procNonAsiaGame(gameDatas[i]);
                //    if (!result)
                //        i--;
                //}
            }
         }
        private async Task<string> getNonAsiaInitString(string strSymbol)
        {
            var         httpClient      = new HttpClient();
            //string      strLaunchURL    = string.Format("https://qtechgames-dk2.pragmaticplay.net/gs2c/playGame.do?key=token%3D2763-15cbfb0f-9daf-4aa2-9d24-9ec124204af5%26symbol%3D{0}%26technology%3DH5%26platform%3DWEB%26language%3Den%26currency%3DTND%26lobbyUrl%3Dhttps%3A%2F%2Fkoora365.org&stylename=qg_qtechgamesrow&isGameUrlApiCalled=true", strSymbol);
            string strLaunchURL = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?gameSymbol={0}&websiteUrl=https%3A%2F%2Fdemogamesfree.pragmaticplay.net&jurisdiction=99&lobby_url=https%3A%2F%2Fwww.pragmaticplay.com%2Fen%2F&lang=EN&cur=USD", strSymbol);
            PPGameInfo  gameInfo        = await getPPGameInfo(httpClient, strLaunchURL, strSymbol);
            string      strInitString   = await getInitString(httpClient, gameInfo, strSymbol);
            return strInitString;
        }
        private async Task<bool> procAsiaGame(GameData gameData, string gameId)
        {
            try
            {
                Console.WriteLine("{0} --------- started!", gameData.GameSymbol);
                string strLaunchURL = await launchGame(gameId);
                if (strLaunchURL == null)
                    return false;

                var httpClient = new HttpClient();
                PPGameInfo gameInfo = await getPPGameInfo(httpClient, strLaunchURL, gameData.GameSymbol);
                if (gameInfo == null)
                    return false;

                string strInitString = await getInitString(httpClient, gameInfo, gameData.GameSymbol);
                procInitString(gameInfo, strInitString);
                generateGameLogic(gameData, gameInfo);
                if (!downloadClients(gameData.GameSymbol, "desktop"))
                    return false;

                if (!downloadClients(gameData.GameSymbol, "mobile"))
                    return false;

                List<string> langs = new List<string>() { "de", "es", "fr", "id", "it", "ko", "pt", "tr", "zh", "uk", "ru" };
                if (!downloadLangFiles(gameData.GameSymbol, "desktop", langs))
                    return false;

                if (!downloadLangFiles(gameData.GameSymbol, "mobile", langs))
                    return false;

                Console.WriteLine("{0} --------- finished!", gameData.GameSymbol);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<string> getDemoKRWInitString(string strGameSymbol)
        {
            var httpClient              = new HttpClient();
            string strLaunchURL         = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?lang=en&cur=KRW&gameSymbol={0}", strGameSymbol);
            PPGameInfo  gameInfo        = await getPPGameInfo(httpClient, strLaunchURL, strGameSymbol);
            if (gameInfo == null)
                return null;

            string strInitString = await getInitString(httpClient, gameInfo, strGameSymbol);
            return strInitString;
        }
        private async Task<bool> procNonAsiaGame(GameData gameData)
        {
            try
            {
                Console.WriteLine("{0} --------- started!", gameData.GameSymbol);
                var httpClient      = new HttpClient();
                //string strLaunchURL = string.Format("https://mlsn5tup3f.mcnwfkxdbd.net/gs2c/playGame.do?key=token%3D2763-8cab2f157ac44f39b6141034c0ffaea7-CIS%60%7C%60symbol%3D{0}%60%7C%60technology%3DH5%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DTND%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Fkoora365.org&ppkv=2&stylename=qg_qtechgamesrow&isGameUrlApiCalled=true", gameData.GameSymbol);
                string strLaunchURL = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?gameSymbol={0}&websiteUrl=https%3A%2F%2Fdemogamesfree.pragmaticplay.net&jurisdiction=99&lobby_url=https%3A%2F%2Fwww.pragmaticplay.com%2Fen%2F&lang=EN&cur=USD",gameData.GameSymbol);
                PPGameInfo gameInfo = await getPPGameInfo(httpClient, strLaunchURL, gameData.GameSymbol);

                if (gameInfo == null)
                    return false;

                string strInitString    = await getInitString(httpClient, gameInfo, gameData.GameSymbol);
                string strKRWInitString = await getDemoKRWInitString(gameData.GameSymbol);

                procInitString(gameInfo, strInitString, strKRWInitString);
                generateGameLogic(gameData, gameInfo);
                if (!downloadClients(gameData.GameSymbol, "desktop"))
                    return false;

                if (!downloadClients(gameData.GameSymbol, "mobile"))
                    return false;

                List<string> langs = new List<string>() { "de", "es", "fr", "id", "it", "ko", "pt", "tr", "zh", "uk", "ru","th" };
                if (!downloadLangFiles(gameData.GameSymbol, "desktop", langs))
                    return false;

                if (!downloadLangFiles(gameData.GameSymbol, "mobile", langs))
                    return false;

                Console.WriteLine("{0} --------- finished!", gameData.GameSymbol);
                return true;

            }
            catch(Exception ex)
            {
                return false;
            }
        }
        private void generateGameLogic(GameData gameData, PPGameInfo gameInfo)
        {
            string strClassTemplate = File.ReadAllText("classtemplate.txt");
            string strClassName     = string.Format("{0}GameLogic", gameData.GameName);
            strClassTemplate        = strClassTemplate.Replace("***ClassName***",  strClassName);
            strClassTemplate        = strClassTemplate.Replace("***GameID***", gameData.GameName);

            strClassTemplate = strClassTemplate.Replace("***SymbolName***", gameData.GameSymbol);
            if(gameInfo.IsReplaySupported)
                strClassTemplate = strClassTemplate.Replace("***SupportReplay***", "true");
            else
                strClassTemplate = strClassTemplate.Replace("***SupportReplay***", "false");

            strClassTemplate = strClassTemplate.Replace("***ClientLineCount***", gameInfo.ServerLineCount.ToString());
            strClassTemplate = strClassTemplate.Replace("***ServerLineCount***", gameInfo.ServerLineCount.ToString());
            strClassTemplate = strClassTemplate.Replace("***ROWS***",            gameInfo.Rows.ToString());
            strClassTemplate = strClassTemplate.Replace("***InitString***",      gameInfo.InitString.Replace("\"", "\\\""));
            
            if(gameInfo.PurchaseMultiple > 0)
            {
                strClassTemplate = strClassTemplate.Replace("***PurchaseMultiple***", gameInfo.PurchaseMultiple.ToString());
                strClassTemplate = strClassTemplate.Replace("***PurchaseStart***", "");
                strClassTemplate = strClassTemplate.Replace("***PurchaseEnd***", "");
            }
            else
            {
                strClassTemplate = removeBetween(strClassTemplate, "***PurchaseStart***", "***PurchaseEnd***");
            }
            if (gameInfo.AnteBetMultiple > 0.0)
            {
                strClassTemplate = strClassTemplate.Replace("***AnteMultiple***", gameInfo.AnteBetMultiple.ToString());
                strClassTemplate = strClassTemplate.Replace("***AnteStart***", "");
                strClassTemplate = strClassTemplate.Replace("***AnteEnd***", "");
            }
            else
            {
                strClassTemplate = removeBetween(strClassTemplate, "***AnteStart***", "***AnteEnd***");
            }
            List<string> strLines = new List<string>();
            foreach(KeyValuePair<string,string> pair in gameInfo.AdditionalInitParams)
            {
                string strLine = string.Format("\tdicParams[\"{0}\"] = \"{1}\";", pair.Key, pair.Value.Replace("\"", "\\\""));
                strLines.Add(strLine);
            }
            strClassTemplate = strClassTemplate.Replace("***DefaultParams***", string.Join("\r\n", strLines.ToArray()));
            if(gameInfo.PurchaseMultiple == 0 && gameInfo.AnteBetMultiple == 0.0)
            {
                strClassTemplate = removeBetween(strClassTemplate, "***ReadBetInfoStart***", "***ReadBetInfoEnd***");
            }
            else if(gameInfo.PurchaseMultiple == 0)
            {
                strClassTemplate = removeBetween(strClassTemplate, "***ReadPurchaseStart***", "***ReadPurchaseEnd***");
                strClassTemplate = removeBetween(strClassTemplate, "***ReadAntePurchaseStart***", "***ReadAntePurchaseEnd***");
            }
            else if(gameInfo.AnteBetMultiple == 0.0)
            {
                strClassTemplate = removeBetween(strClassTemplate, "***ReadAnteInfoStart***", "***ReadAnteInfoEnd***");
                strClassTemplate = removeBetween(strClassTemplate, "***ReadAntePurchaseStart***", "***ReadAntePurchaseEnd***");
            }
            strClassTemplate = strClassTemplate.Replace("***ReadAnteInfoStart***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadAnteInfoEnd***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadPurchaseStart***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadPurchaseEnd***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadAntePurchaseStart***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadAntePurchaseEnd***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadBetInfoStart***", "");
            strClassTemplate = strClassTemplate.Replace("***ReadBetInfoEnd***", "");

            string strFileName = string.Format("server/{0}.cs", strClassName);
            File.WriteAllText(strFileName, strClassTemplate);
        }
        private bool downloadClients(string strSymbol, string strPlatform)
        {
            string[] defaultFilePaths = new string[]
            {
                "bootstrap.js",
                string.Format("packages/ko_{0}.json",strPlatform),
                string.Format("packages/ko_GUI_{0}.json", strPlatform),
                "customizations.info",
                "build.js",
                "style.css",
                "client/game.json",
                "client/resources.json",
            };
            int maxCount = defaultFilePaths.Length;
            if (strSymbol.StartsWith("c"))
                maxCount -= 1;

            for (int i = 0; i < maxCount; i++)
            {
                if (!downloadFile(strSymbol, strPlatform, defaultFilePaths[i]))
                    return false;
            }
            List<string> strFileNames = parseGameResources(strSymbol, strPlatform);
            for (int i = 0; i < strFileNames.Count; i++)
            {
                if (!downloadFile(strSymbol, strPlatform, string.Format("game/{0}.json", strFileNames[i])))
                    return false;
            }

            string strMainResources     = "";
            string strGUIResources      = "";
            string strOtherResources    = "";
            string strCSResources       = "";
            for (int i = 0; i < strFileNames.Count; i++)
            {
                if (strFileNames[i].Contains("main_resources"))
                {
                    string strFileName = string.Format("client/{0}/{1}/game/{2}.json", strSymbol, strPlatform, strFileNames[i]);
                    string strContent = File.ReadAllText(strFileName);
                    strMainResources += strContent;
                }
                else if (strFileNames[i].Contains("GUI_resources"))
                {
                    string strFileName = string.Format("client/{0}/{1}/game/{2}.json", strSymbol, strPlatform, strFileNames[i]);
                    string strContent = File.ReadAllText(strFileName);
                    strGUIResources += strContent;
                }
                else if(strFileNames[i].Contains("other_resources"))
                {
                    string strFileName = string.Format("client/{0}/{1}/game/{2}.json", strSymbol, strPlatform, strFileNames[i]);
                    string strContent = File.ReadAllText(strFileName);
                    strOtherResources += strContent;
                }
                
            }
            if (strMainResources != "" && !downloadNotInLineResource(strSymbol, strPlatform, JToken.Parse(strMainResources)))
                return false;

            if (strGUIResources != "" && !downloadNotInLineResource(strSymbol, strPlatform, JToken.Parse(strGUIResources)))
                return false;

            if (strOtherResources != "" && !downloadNotInLineResource(strSymbol, strPlatform, JToken.Parse(strOtherResources)))
                return false;


            string strFilePath      = string.Format("client/{0}/{1}/bootstrap.js", strSymbol, strPlatform);
            string strFileContent   = File.ReadAllText(strFilePath);
            if (strFileContent.Contains("var url=config[\"replaySystemUrl\"]+contextPath+\"/api/replay/data?token=\"+config[\"mgckey\"]+\"&roundID=\"+config[\"replayRoundId\"]+\"&envID=\"+config[\"environmentId\"];"))
            {
                strFileContent = strFileContent.Replace("var url=config[\"replaySystemUrl\"]+contextPath+\"/api/replay/data?token=\"+config[\"mgckey\"]+\"&roundID=\"+config[\"replayRoundId\"]+\"&envID=\"+config[\"environmentId\"];",
                    "var url=config[\"replayAPIURL\"] + \"/gitapi/pp/replay/data?token=\" + config[\"mgckey\"] + \"&roundID=\" + config[\"replayRoundId\"] + \"&envID=\" + config[\"environmentId\"] + \"&symbol=\" + config[\"symbol\"];");

                File.WriteAllText(strFilePath, strFileContent);
            }
            else
            {

            }
            strFilePath = string.Format("client/{0}/{1}/build.js", strSymbol, strPlatform);
            strFileContent = File.ReadAllText(strFilePath);
            if (strFileContent.Contains("ServerOptions.serverUrl=(ServerOptions.isSecure?\"https://\":\"http://\")+ServerInterface.GetDomain();"))
            {
                strFileContent = strFileContent.Replace("ServerOptions.serverUrl=(ServerOptions.isSecure?\"https://\":\"http://\")+ServerInterface.GetDomain();",
                    "ServerOptions.serverUrl = UHT_GAME_CONFIG_SRC[\"apiuri\"];");
            }
            else
            {

            }
            if (strFileContent.Contains("TournamentURLs:{tournaments:\"/gs2c/promo/active/\",details:\"/gs2c/promo/tournament/details/\",leaderboards:\"/gs2c/promo/tournament/v3/leaderboard/\",raceDetails:\"/gs2c/promo/race/details/\",racePrizes:\"/gs2c/promo/race/prizes/\",raceOptIn:\"/gs2c/promo/race/player/choice/OPTIN/\",raceOptOut:\"/gs2c/promo/race/player/choice/OPTOUT/\",tournamentOptIn:\"/gs2c/promo/tournament/player/choice/OPTIN/\",tournamentOptOut:\"/gs2c/promo/tournament/player/choice/OPTOUT/\",tournamentScores:\"/gs2c/promo/tournament/scores/\"}"))
            {
                strFileContent = strFileContent.Replace("TournamentURLs:{tournaments:\"/gs2c/promo/active/\",details:\"/gs2c/promo/tournament/details/\",leaderboards:\"/gs2c/promo/tournament/v3/leaderboard/\",raceDetails:\"/gs2c/promo/race/details/\",racePrizes:\"/gs2c/promo/race/prizes/\",raceOptIn:\"/gs2c/promo/race/player/choice/OPTIN/\",raceOptOut:\"/gs2c/promo/race/player/choice/OPTOUT/\",tournamentOptIn:\"/gs2c/promo/tournament/player/choice/OPTIN/\",tournamentOptOut:\"/gs2c/promo/tournament/player/choice/OPTOUT/\",tournamentScores:\"/gs2c/promo/tournament/scores/\"}",
                    "TournamentURLs:{tournaments:\"/gitapi/pp/promo/active/\",details:\"/gitapi/pp/promo/tournament/details/\",leaderboards:\"/gitapi/pp/promo/tournament/v3/leaderboard/\",raceDetails:\"/gitapi/pp/promo/race/details/\",racePrizes:\"/gitapi/pp/promo/race/prizes/\",raceOptIn:\"/gitapi/pp/promo/race/player/choice/OPTIN/\",raceOptOut:\"/gitapi/pp/promo/race/player/choice/OPTOUT/\",tournamentOptIn:\"/gitapi/pp/promo/tournament/player/choice/OPTIN/\",tournamentOptOut:\"/gitapi/pp/promo/tournament/player/choice/OPTOUT/\",tournamentScores:\"/gitapi/pp/promo/tournament/scores/\"}");
            }
            else
            {

            }
            if (strFileContent.Contains("var contextPath=\"/ReplayService\";if(config[ReplayAPI.Keys.replaySystemContextPath]!=undefined)contextPath=config[ReplayAPI.Keys.replaySystemContextPath];var query=\"?\"+[GameProtocolDictionary.mgckey+\"=\"+ServerOptions.mgckey,ReplayAPI.Keys.envID+\"=\"+config[ReplayAPI.Keys.environmentId]].join(\"&\");var watchQuery=\"?\"+[ReplayAPI.Keys.token+\"=\"+ServerOptions.mgckey,ReplayAPI.Keys.envID+\"=\"+config[ReplayAPI.Keys.environmentId]].join(\"&\");ReplayConnection.watchURL=url+contextPath+\"/replayGame.do\"+watchQuery;"))
            {
                strFileContent = strFileContent.Replace("var contextPath=\"/ReplayService\";if(config[ReplayAPI.Keys.replaySystemContextPath]!=undefined)contextPath=config[ReplayAPI.Keys.replaySystemContextPath];var query=\"?\"+[GameProtocolDictionary.mgckey+\"=\"+ServerOptions.mgckey,ReplayAPI.Keys.envID+\"=\"+config[ReplayAPI.Keys.environmentId]].join(\"&\");var watchQuery=\"?\"+[ReplayAPI.Keys.token+\"=\"+ServerOptions.mgckey,ReplayAPI.Keys.envID+\"=\"+config[ReplayAPI.Keys.environmentId]].join(\"&\");ReplayConnection.watchURL=url+contextPath+\"/replayGame.do\"+watchQuery;",
                    "var query = \"?\" + [GameProtocolDictionary.mgckey + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol, \"lang=\" + config[\"lang\"], \"currency=\" + config[\"currency\"]].join(\"&\");var watchQuery = \"?\" + [ReplayAPI.Keys.token + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol, \"lang=\" + config[\"lang\"], \"currency=\" + config[\"currency\"]].join(\"&\");ReplayConnection.watchURL = url + \"/replayGame\" + watchQuery;");
            }
            else
            {

            }
            if (strFileContent.Contains("this.winningsURL=url+contextPath+\"/api/top/winnings/list\"+query;this.sharedLinkURL=url+contextPath+\"/api/top/share/link\"+query;XT.SetBool(ReplayVars.Replay_WinningsEnabled,true)"))
            {
                strFileContent = strFileContent.Replace("this.winningsURL=url+contextPath+\"/api/top/winnings/list\"+query;this.sharedLinkURL=url+contextPath+\"/api/top/share/link\"+query;XT.SetBool(ReplayVars.Replay_WinningsEnabled,true)",
                    "var replayAPIURL=config[\"replayAPIURL\"];this.winningsURL=replayAPIURL + \"/gitapi/pp/replay/list\" + query;this.sharedLinkURL = replayAPIURL + \"/gitapi/pp/replay/link\" + query;XT.SetBool(ReplayVars.Replay_WinningsEnabled, true)");
            }
            else
            {

            }
            if (strFileContent.Contains("var url=UHT_GAME_CONFIG_SRC[\"replaySystemUrl\"]+\"/ReplayService/api/top/share/link?mgckey=\"+UHT_GAME_CONFIG_SRC[\"mgckey\"]+\"&roundID=\"+UHT_GAME_CONFIG_SRC[\"replayRoundId\"]+\"&envID=\"+UHT_GAME_CONFIG_SRC[\"environmentId\"];"))
            {
                strFileContent = strFileContent.Replace("var url=UHT_GAME_CONFIG_SRC[\"replaySystemUrl\"]+\"/ReplayService/api/top/share/link?mgckey=\"+UHT_GAME_CONFIG_SRC[\"mgckey\"]+\"&roundID=\"+UHT_GAME_CONFIG_SRC[\"replayRoundId\"]+\"&envID=\"+UHT_GAME_CONFIG_SRC[\"environmentId\"];",
                    "var url=UHT_GAME_CONFIG_SRC[\"replayAPIURL\"] + \"/gitapi/pp/replay/link?mgckey=\" + UHT_GAME_CONFIG_SRC[\"mgckey\"] + \"&roundID=\" + UHT_GAME_CONFIG_SRC[\"replayRoundId\"] + \"&envID=\" + UHT_GAME_CONFIG_SRC[\"environmentId\"] + \"&symbol=\" + ServerOptions.gameSymbol;");
            }
            else
            {

            }
            if(strFileContent.Contains("if(location.href.indexOf(\"envID\")!=-1||location.href.indexOf(\"roundID\")!=-1)"))
            {
                strFileContent = strFileContent.Replace("if(location.href.indexOf(\"envID\")!=-1||location.href.indexOf(\"roundID\")!=-1)",
                    "if(ServerOptions.mgckey.indexOf(\"replay\") == -1)");
            }
            else
            {

            }
            if(strFileContent.Contains("this.raceWinnersURL=ServerOptions.serverUrl+\"/gs2c/promo/race/v2/winners/\";"))
            {
                strFileContent = strFileContent.Replace("this.raceWinnersURL=ServerOptions.serverUrl+\"/gs2c/promo/race/v2/winners/\";",
                    "this.raceWinnersURL=ServerOptions.serverUrl+\"/gitapi/pp/promo/race/v2/winners/\";");
            }
            else
            {

            }
            if (strFileContent.Contains("this.endpointUrl=\"/gs2c/promo/frb/available/\";"))
            {
                strFileContent = strFileContent.Replace("this.endpointUrl=\"/gs2c/promo/frb/available/\";",
                    "this.endpointUrl=\"/gitapi/pp/promo/frb/available/\";");
            }
            else
            {

            }
            if (strFileContent.Contains("gvuArgs[1]=GameProtocolDictionary.mgckey+\"=\"+ServerOptions.mgckey;"))
            {
                strFileContent = strFileContent.Replace("gvuArgs[1]=GameProtocolDictionary.mgckey+\"=\"+ServerOptions.mgckey;",
                    "gvuArgs[1]=GameProtocolDictionary.mgckey+\"=\"+ServerOptions.mgckey;gvuArgs[2]=\"company=\"+verifyCompanyParam;");
            }    
            else
            {

            }

            try
            {
                do
                {
                    long bufIndex = strFileContent.IndexOf("=!![]);try{return ");
                    if (bufIndex < 0)
                    {
                        Console.WriteLine("Set Rsa Error! - 0");
                        break;
                    }
                    else
                    {
                        int firstLastFunctionIndex = strFileContent.LastIndexOf("function(", (int)bufIndex);
                        if (firstLastFunctionIndex < 0)
                        {
                            Console.WriteLine("Set Rsa Error! - 1");
                            break;
                        }

                        int secondLastFunctionIndex = strFileContent.LastIndexOf("function(", (int)firstLastFunctionIndex - 1);
                        if (secondLastFunctionIndex < 0)
                        {
                            Console.WriteLine("Set Rsa Error! - 2");
                            break;
                        }

                        int rsaStartIndex = strFileContent.IndexOf("(", secondLastFunctionIndex + 1);
                        if (rsaStartIndex < 0)
                        {
                            Console.WriteLine("Set Rsa Error! - 3");
                            break;
                        }

                        int rsaEndIndex = strFileContent.IndexOf(")", rsaStartIndex);
                        if (rsaEndIndex < 0)
                        {
                            Console.WriteLine("Set Rsa Error! - 4");
                            break;
                        }

                        string rsaArg = strFileContent.Substring(rsaStartIndex, rsaEndIndex - rsaStartIndex + 1);

                        strFileContent = strFileContent.Replace(rsaArg + ";", "(RSAPublicKey);");
                    }
                }
                while (false);
            }
            catch(Exception e) 
            { 
                Console.WriteLine($"Error: {e.Message}");
            }
            

            File.WriteAllText(strFilePath, strFileContent);
            return true;
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
                fileName = string.Format("packages/{0}_client.json", lang);
                defaultFilePaths.Add(fileName);
            }

            for(int i = 0; i < defaultFilePaths.Count; i++)
            {
                if (!downloadFile(strSymbol, strPlatform, defaultFilePaths[i])) 
                {
                    Console.WriteLine("Language file not exist {0} {1} {2}", strSymbol, strPlatform, defaultFilePaths[i]);
                }
            }

            return true;
        }
        private bool downloadNotInLineResource(string strSymbol, string strPlatform, JToken token)
        {
            List<string> strResourcePaths = new List<string>();
            JArray resourceArray = token["resources"] as JArray;
            for(int i = 0; i < resourceArray.Count; i++)
            {
                JToken resource = resourceArray[i];
                if ((resource["isInline"] == null) || ((bool) resource["isInline"] == false))
                {
                    if (resource["type"].ToString() == "Texture")
                        strResourcePaths.Add(resource["data"].ToString());
                }
            }
            for (int i = 0; i < strResourcePaths.Count; i++)
            {
                if (!downloadFile(strSymbol, strPlatform, string.Format("game/{0}", strResourcePaths[i])))
                    return false;
            }
            return true;
        }
        private List<string> parseGameResources(string strSymbol, string strPlatform)
        {
            string strFileName      = string.Format("client/{0}/{1}/bootstrap.js", strSymbol, strPlatform);
            string strContent       = File.ReadAllText(strFileName);
            string strGameSizes     = findStringBetween(strContent, "UHT_GAME_SIZE='", "'");
            string strResourceSizes = findStringBetween(strContent, "UHT_RESOURCES_SIZE='", "'");
            string strOtherSizes    = findStringBetween(strContent, "UHT_OTHER_SIZE='", "'");
            string strGUISizes      = findStringBetween(strContent, "UHT_GUI_SIZE='", "'");
            string strGUIResSizes   = findStringBetween(strContent, "UHT_GUI_RESOURCES_SIZE='", "'");
            string strSoundSizes    = findStringBetween(strContent, "UHT_SOUNDS_SIZES='", "'");

            List<string> strNames = new List<string>();
            strNames.AddRange(parseGameSizes(strSymbol, strGameSizes));
            if(strResourceSizes != null)
                strNames.AddRange(parseGameSizes(strSymbol, strResourceSizes));
    
            if(strOtherSizes != null)
                strNames.AddRange(parseGameSizes(strSymbol, strOtherSizes));
            if(strGUISizes != null)
                strNames.AddRange(parseGameSizes(strSymbol, strGUISizes));
            if(strGUIResSizes != null)
                strNames.AddRange(parseGameSizes(strSymbol, strGUIResSizes));
            if(strSoundSizes != null)
                strNames.AddRange(parseGameSizes(strSymbol, strSoundSizes));

            return strNames;
        }
        private List<string> parseGameSizes(string strSymbol, string strGameSizes)
        {
            string[] strParts = strGameSizes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> strNames = new List<string>();
            for(int i = 0; i < strParts.Length; i++)
            {
                int index = strParts[i].IndexOf("?");
                if(strSymbol.StartsWith("c"))
                    index = strParts[i].IndexOf(":");
                strNames.Add(strParts[i].Substring(0, index));
            }
            if (strSymbol.StartsWith("c"))
            {
                for(int i = 0; i < strNames.Count - 1; i++)
                {
                    for(int j = i + 1; j < strNames.Count; j++)
                    {
                        int index1 = 0;
                        if (!int.TryParse(strNames[i].Replace("game", ""), out index1))
                            break;

                        int index2 = 0;
                        if (!int.TryParse(strNames[j].Replace("game", ""), out index2))
                            break;

                        if(index1 > index2)
                        {
                            string strName = strNames[i];
                            strNames[i] = strNames[j];
                            strNames[j] = strName;
                        }
                    }
                }
            }
            return strNames;
        }
        private bool downloadFile(string strSymbol, string strPlatform, string strPath)
        {
            try
            {
                string strURL = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/common/v5/games-html5/games/vs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                if (strSymbol.StartsWith("c"))
                    strURL = string.Format("https://common-static.pragmaticplay.net/gs2c/common/v5/games-html5/games/cs/{0}/{1}/{2}", strSymbol, strPlatform, strPath);

                string strFileName  = string.Format("client/{0}/{1}/{2}", strSymbol, strPlatform, strPath);
                if (File.Exists(strFileName))
                    return true;

                int retryCnt = 0;
                do
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        var response = client.GetAsync(strURL).Result;
                        response.EnsureSuccessStatusCode();

                        string strFolder = strFileName.Substring(0, strFileName.LastIndexOf("/"));
                        Directory.CreateDirectory(strFolder);

                        using (var fs = new FileStream(strFileName, FileMode.Create))
                        {
                            response.Content.CopyToAsync(fs).Wait();
                        }
                        if (strPath.EndsWith("customizations.info"))
                            File.WriteAllText(strFileName, "");

                        break;
                    }
                    catch (Exception ex1)
                    {
                        retryCnt++;
                        if(retryCnt == 10)
                        {
                            Console.WriteLine(strURL);
                            break;
                        }
                    }
                }
                while (true);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        private string removeBetween(string strContent, string strStart, string strEnd)
        {
            int startIndex = strContent.IndexOf(strStart);
            int endIndex   = strContent.IndexOf(strEnd, startIndex) + strEnd.Length;
            return strContent.Remove(startIndex, endIndex - startIndex);

        }
        private bool procInitString(PPGameInfo gameInfo, string strInitString, string strKRWInitString = null)
        {
            var dicInitParams = splitResponseToParams(strInitString);
            dicInitParams.Remove("balance");
            dicInitParams.Remove("index");
            dicInitParams.Remove("counter");
            dicInitParams.Remove("balance_cash");
            dicInitParams.Remove("balance_bonus");
            dicInitParams.Remove("na");
            dicInitParams.Remove("stime");
            dicInitParams.Remove("sa");
            dicInitParams.Remove("sb");
            dicInitParams.Remove("s");
            dicInitParams.Remove("c");
            dicInitParams.Remove("sver");

            if (dicInitParams.ContainsKey("reel_set"))
            {
                gameInfo.AdditionalInitParams.Add("reel_set", dicInitParams["reel_set"]);
                dicInitParams.Remove("reel_set");
            }
            else
            {
            }
            if (dicInitParams.ContainsKey("sh"))
            {
                gameInfo.Rows = int.Parse(dicInitParams["sh"]);
                dicInitParams.Remove("sh");
            }
            else
            {
                //return false;
            }
            if (dicInitParams.ContainsKey("l"))
            {
                gameInfo.ServerLineCount = int.Parse(dicInitParams["l"]);
                dicInitParams.Remove("l");
            }
            else
            {
                //return false;
            }
            if(dicInitParams.ContainsKey("g"))
            {
                gameInfo.AdditionalInitParams.Add("g", dicInitParams["g"]);
                dicInitParams.Remove("g");
            }
            if (dicInitParams.ContainsKey("st"))
            {
                gameInfo.AdditionalInitParams.Add("st", dicInitParams["st"]);
                dicInitParams.Remove("st");
            }
            if (dicInitParams.ContainsKey("sw"))
            {
                gameInfo.AdditionalInitParams.Add("sw", dicInitParams["sw"]);
                dicInitParams.Remove("sw");
            }
            if (dicInitParams.ContainsKey("bls"))
            {
                string[] parts = dicInitParams["bls"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                double bl0 = double.Parse(parts[0]);
                double bl1 = double.Parse(parts[1]);
                gameInfo.AnteBetMultiple = Math.Round(bl1 / bl0, 2);
            }
            if(dicInitParams.ContainsKey("bl"))
            {
                gameInfo.AdditionalInitParams.Add("bl", dicInitParams["bl"]);
                dicInitParams.Remove("bl");
            }
            if (dicInitParams.ContainsKey("purInit"))
            {
                JArray purInitArray         = JArray.Parse(dicInitParams["purInit"]);
                gameInfo.PurchaseMultiple   = int.Parse(purInitArray[0]["bet"].ToString()) / gameInfo.ServerLineCount;
            }            
            if(!string.IsNullOrEmpty(strKRWInitString))
            {
                var dicKRWParams = splitResponseToParams(strKRWInitString);
                if (dicInitParams.ContainsKey("sc"))
                    dicInitParams["sc"] = dicKRWParams["sc"];

                if (dicInitParams.ContainsKey("defc"))
                    dicInitParams["defc"] = dicKRWParams["defc"];

                if (dicInitParams.ContainsKey("total_bet_min"))
                    dicInitParams["total_bet_min"] = dicKRWParams["total_bet_min"];

                if (dicInitParams.ContainsKey("total_bet_max"))
                    dicInitParams["total_bet_max"] = dicKRWParams["total_bet_max"];
            }
            gameInfo.InitString = convertKeyValuesToString(dicInitParams);
            return true;
        }
        protected string serializeJsonSpecial(JToken token)
        {
            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, token);
            }
            return stringWriter.ToString();
        }
        private Dictionary<string, string> splitResponseToParams(string strResponse)
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
        private string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }

        private async Task<string> getInitString(HttpClient httpClient, PPGameInfo gameInfo, string strSymbol)
        {
            KeyValuePair<string, string>[] postParams = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("action", "doInit"),
                new KeyValuePair<string, string>("symbol", strSymbol),
                new KeyValuePair<string, string>("cver",   gameInfo.CVer),
                new KeyValuePair<string, string>("index", "1"),
                new KeyValuePair<string, string>("counter", "1"),
                new KeyValuePair<string, string>("repeat", "0"),
                new KeyValuePair<string, string>("mgckey", gameInfo.Token),
            };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://modoogames-sg13.ppgames.net");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://modoogames-sg13.ppgames.net");
            var response = await httpClient.PostAsync(gameInfo.GameServiceURL, new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        private async Task<PPGameInfo> getPPGameInfo(HttpClient httpClient, string strURL, string strGameSymbol)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();

                string strContent   = await response.Content.ReadAsStringAsync();                
                string strGameConfig = findStringBetween(strContent, "gameConfig: '", "'");

                JToken gameConfig   = JToken.Parse(strGameConfig);
                PPGameInfo info     = new PPGameInfo();
                info.GameServiceURL = gameConfig["gameService"].ToString();
                info.Token          = gameConfig["mgckey"].ToString();
                if (gameConfig["replaySystemUrl"] == null)
                    info.IsReplaySupported = false;
                else
                    info.IsReplaySupported = true;
                info.DataPath = gameConfig["datapath"].ToString();
                string strBootstrapURL = string.Format("{0}desktop/bootstrap.js", info.DataPath);
                response               = await httpClient.GetAsync(strBootstrapURL);
                response.EnsureSuccessStatusCode();

                strContent = await response.Content.ReadAsStringAsync();
                string strUHTRevision = findStringBetween(strContent, ";UHT_REVISION={", "}");
                info.CVer = findStringBetween(strUHTRevision, "desktop:'", "'");
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private string findStringBetween(string strContent, string strStart, string strEnd)
        {
            int startIndex = strContent.IndexOf(strStart);
            if (startIndex < 0)
                return null;

            startIndex += strStart.Length;
            int endIndex = strContent.IndexOf(strEnd, startIndex);
            return strContent.Substring(startIndex, endIndex - startIndex);
        }
        private async Task<string> launchGame(string strSymbol)
        {
            var loginResult = await SlotDiamondAPI.Instance.loginAccount("promotionfetcher", "Abc123");
            if (loginResult == null)
                return null;

            string strLaunchURL = await SlotDiamondAPI.Instance.getGameLaunchURL(loginResult.UserCode, loginResult.Token, strSymbol, "https://casino.com");

            HttpClient              httpClient  = new HttpClient();
            HttpResponseMessage     response    = await httpClient.GetAsync(strLaunchURL);
            response.EnsureSuccessStatusCode();
            string strContent = await response.Content.ReadAsStringAsync();
            KeyValuePair<string, string>[] strParams = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("domainname", "h" + findStringBetween(strContent, "var domain = 'h", "';")),
                    new KeyValuePair<string, string>("tokenVal", findStringBetween(strContent, "var token = '", "';")),
                    new KeyValuePair<string, string>("symbolVal", findStringBetween(strContent, "var symbol = '", "';")),
                    new KeyValuePair<string, string>("technologyVal", "H5"),
                    new KeyValuePair<string, string>("platformVal", "WEB"),
                    new KeyValuePair<string, string>("languageVal", "ko"),
                    new KeyValuePair<string, string>("cashierUrlVal", "local"),
                    new KeyValuePair<string, string>("lobbyUrlVal", "local"),
                    new KeyValuePair<string, string>("secureLoginVal", "blst_slotsmania"),
                    new KeyValuePair<string, string>("keyVal", findStringBetween(strContent, "var key = '", "';")),
                    new KeyValuePair<string, string>("userId", findStringBetween(strContent, "var userCode = '", "';")),
                    new KeyValuePair<string, string>("currency", "KRW"),

                };
            response = await httpClient.PostAsync("https://prd-sdv2-api.slotsdiamond.com/PP/ApiprodpragmaticgamesslotV2/GetCasinoGames", new FormUrlEncodedContent(strParams));
            response.EnsureSuccessStatusCode();
            strContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<SlotDiamondURLResponse>(strContent);

            return apiResponse.game_url;
        }        
    }
    public class SlotDiamondURLResponse
    {
        public string game_url { get; set; }
        public int error { get; set; }
    }

    class GameData
    {
        public string GameName      { get; set; }
        public string GameSymbol    { get; set; }
        public string GameTitle     { get; set; }
    }
    class GetGameListRequest
    {
        public string language_id { get; set; }
        public string provider_id { get; set; }
        public string limit { get; set; }
        public string offset { get; set; }
    }
    class GameListData
    {
        public List<PPGame> data { get; set; }
    }
    class PPGame
    {
        public string id            { get; set; }
        public string game_code     { get; set; }
        public string is_demo       { get; set; }
    }
    
    class CreateAccountResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public int user_id { get; set; }
            public string username { get; set; }
            public string usercode { get; set; }
            public string token { get; set; }
        }
    }
    class CreateAccountRequest
    {
        public string username { get; set; }
        public string password { get; set; }

    }
    class GetGameURLRequest
    {
        public string mode { get; set; }
        public string usercode { get; set; }
        public string game { get; set; }
        public string lang { get; set; }
        public string return_url { get; set; }
        public string token { get; set; }
    }
    class GetGameURLResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public string return_url { get; set; }
        }
    }
    class PPGameInfo
    {
        public string GameServiceURL    { get; set; }
        public string Token             { get; set; }
        public bool   IsReplaySupported { get; set; }
        public string CVer              { get; set; }
        public string DataPath          { get; set; }
        public int    ServerLineCount   { get; set; }
        public int    Rows              { get; set; }
        public int    PurchaseMultiple  { get; set; }
        public double AnteBetMultiple   { get; set; }
        public string InitString        { get; set; }
        public Dictionary<string, string> AdditionalInitParams { get; set; }

        public PPGameInfo()
        {
            this.AdditionalInitParams = new Dictionary<string, string>();
        }
    }
}
