using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PPLangDown
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> gameDatas = readGameData();
            
            LangDownTools.Instance.downloadData(gameDatas).Wait();
            //LangDownTools.Instance.downloadLogoInfo().Wait();
            
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
