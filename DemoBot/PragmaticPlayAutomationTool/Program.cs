using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace PragmaticPlayAutomationTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            SlotDiamondAPI.Instance.ApiURL    = "https://prd-sdv2-api.slotsdiamond.com/apiprod-slotsdiamond";
            SlotDiamondAPI.Instance.ClientID  = "1290";
            SlotDiamondAPI.Instance.SecretKey = "96016048287144200";

            /*
            var loginResult = SlotDiamondAPI.Instance.loginAccount("promotionfetcher", "Abc123").Result;
            string strLaunchURL = SlotDiamondAPI.Instance.getGameLaunchURL(loginResult.UserCode, loginResult.Token, "11150", "https://casino.com").Result;
            */

            List<GameData> gameDatas            = readGameData();
            List<string> strLines               = new List<string>();
            int          gameId                 = 2388;
            for(int i = 6; i < gameDatas.Count;i++)
            {
                string strLine = string.Format("INSERT INTO gameconfigs(gameid,gametype,gamename,gamesymbol,payoutrate,poolredundency,eventrate,openclose,updatetime)" + 
                    " VALUES({0},1,'{1}','{2}',95.0,1000.0,0.0,1,'2023-10-02')", gameId, gameDatas[i].GameName,
                    gameDatas[i].GameSymbol);

                //string strLine = string.Format("{0} = {1},", gameDatas[i].GameName, gameId);
                //string strLine = string.Format("_dicGameLogicActors.Add(GAMEID.{0},             Context.ActorOf(Props.Create(() => new {0}GameLogic()),              \"{0}\"));", gameDatas[i].GameName);
                strLines.Add(strLine);
                gameId++;
            }
            string strText = string.Join("\r\n", strLines.ToArray());
            ClientDownloadTool.Instance.downloadData(gameDatas).Wait();
        }

        static List<GameData> readGameData()
        {
            List<GameData> gameDataList = new List<GameData>();
            string strFileData = File.ReadAllText("gamedata.txt");
            string[] strLines = strFileData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strLines.Length; i++)
            {
                string[] strParts = strLines[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);

                GameData gameData   = new GameData();
                gameData.GameSymbol = strParts[1].Trim();
                gameData.GameName   = strParts[0].Trim();
                gameDataList.Add(gameData);
            }
            return gameDataList;
        }
    }
}
