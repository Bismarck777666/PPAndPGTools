using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPReplayLinkFix
{
    class ReplayLinkUpdate
    {
        private static ReplayLinkUpdate _instance   = new ReplayLinkUpdate();
        public static ReplayLinkUpdate  Instance    => _instance;

        public void updateBuildJsFile(List<string> gameSymbols)
        {
            foreach (var gameSymbol in gameSymbols) 
            {
                string[] platForms = new string[2] { "desktop", "mobile" };
                for (int i = 0; i < platForms.Length; i++)
                {
                    string strFilePath      = string.Format("vs/{0}/{1}/build.js", gameSymbol, platForms[i]);
                    string strFileContent   = File.ReadAllText(strFilePath);

                    if (strFileContent.Contains("var query = \"?\" + [GameProtocolDictionary.mgckey + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol].join(\"&\");var watchQuery = \"?\" + [ReplayAPI.Keys.token + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol].join(\"&\");ReplayConnection.watchURL = url + \"/replayGame\" + watchQuery;"))
                    {
                        strFileContent = strFileContent.Replace("var query = \"?\" + [GameProtocolDictionary.mgckey + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol].join(\"&\");var watchQuery = \"?\" + [ReplayAPI.Keys.token + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol].join(\"&\");ReplayConnection.watchURL = url + \"/replayGame\" + watchQuery;",
                            "var query = \"?\" + [GameProtocolDictionary.mgckey + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol, \"lang=\" + config[\"lang\"], \"currency=\" + config[\"currency\"]].join(\"&\");var watchQuery = \"?\" + [ReplayAPI.Keys.token + \"=\" + ServerOptions.mgckey, ReplayAPI.Keys.envID + \"=\" + config[ReplayAPI.Keys.environmentId], \"symbol=\" + ServerOptions.gameSymbol, \"lang=\" + config[\"lang\"], \"currency=\" + config[\"currency\"]].join(\"&\");ReplayConnection.watchURL = url + \"/replayGame\" + watchQuery;");
                    }

                    File.WriteAllText(strFilePath, strFileContent);
                }

                Console.WriteLine("{0} ------ build.js file update finished!", gameSymbol);
            }
        }
    }
}
