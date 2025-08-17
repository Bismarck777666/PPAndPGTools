using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPReplayLinkFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> gameDatas = readGameData();

            //ReplayLinkUpdate.Instance.updateBuildJsFile(gameDatas);
            RSAPublicKeyUpdate.Instance.updateBuildJsFile(gameDatas);
        }

        static List<string> readGameData()
        {
            List<string> gameDataList = new List<string>();
            string strFileData = File.ReadAllText("gamesymbols.txt");
            string[] strLines = strFileData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strLines.Length; i++)
                gameDataList.Add(strLines[i].Trim());

            return gameDataList;
        }
    }
}
