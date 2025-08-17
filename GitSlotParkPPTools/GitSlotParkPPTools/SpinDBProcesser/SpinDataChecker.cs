using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace SpinDBProcesser
{
    class SpinDataChecker
    {
        protected static Dictionary<string, string> splitResponseToParams(string strResponse)
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

        public static async Task doCheck()
        {
            string strFolder = "D:\\Workshop\\PragmaticPlayGames\\Server\\GITGameServerSolution2021(Sqlite)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\";
            string[] strFileNames = Directory.GetFiles(strFolder, "*.db");

            for(int i = 0; i < strFileNames.Length; i++)
            {
                string strFilePath = Path.Combine(strFolder, strFileNames[i]);
                string strConnString = @"Data Source=" + strFilePath;
                using (SQLiteConnection connection = new SQLiteConnection(strConnString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM spins";
                    List<int> invalidIDs = new List<int>();
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strData = (string)reader["data"];
                            string[] strLines = strData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                            bool isValid = true;
                            for(int j = 0; j < strLines.Length; j++)
                            {
                                Dictionary<string, string> dicParams = splitResponseToParams(strLines[j]);
                                if(!dicParams.ContainsKey("na"))
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                            if (!isValid)
                                invalidIDs.Add((int)(long)reader["id"]);

                            
                        }
                    }
                    if(invalidIDs.Count > 0)
                    {

                    }
                }
            }
            
        }
    }
}
