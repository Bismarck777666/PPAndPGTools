using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConvert
{
    public class ReelSetAdd
    {
        private static ReelSetAdd _sInstance = new ReelSetAdd();
        public static ReelSetAdd Instance => _sInstance;
        public async Task doAddReelSet()
        {
            GameInfo info = new GameInfo("MysteryMice", true, true, false, 100);
            string strFileName          = string.Format("./slotdata/{0}.db", info.Name);
            string strConnectionString  = @"Data Source=" + strFileName;
            
            if (!File.Exists(strFileName))
                return;

            using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
            {
                await connection.OpenAsync();
                
                string strCommand       = "SELECT * FROM spins";
                SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int     id      = (int)(long)reader["id"];
                        string  data    = (string)reader["data"];

                        await updateData(id, data, connection);
                    }
                }
            }

            Console.WriteLine("ReelSetAdd has been finished!!!");
        }

        public async Task updateData(int id, string data, SQLiteConnection connection)
        {
            try
            {
                string[] strLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<string> newStrLines = new List<string>();
                
                int oldReelSet = 0;
                foreach (string strLine in strLines)
                {
                    var dicParams = splitResponseToParams(strLine);
                
                    if (dicParams.ContainsKey("reel_set"))
                        oldReelSet = int.Parse(dicParams["reel_set"]);
                    else
                        dicParams["reel_set"] = oldReelSet.ToString();
            
                    newStrLines.Add(joinParams(dicParams));
                }
                data = string.Join("\n", newStrLines.ToArray());

                string strCommand       = "UPDATE spins SET data=@data WHERE id=@id";
                SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@data",    data);
                command.Parameters.AddWithValue("@id",      id);

                await command.ExecuteNonQueryAsync();
            }
            catch(Exception ex)
            {

            }
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

        protected string joinParams(Dictionary<string, string> dicParamValues) 
        {
            string strData = string.Empty;
            foreach (KeyValuePair<string, string> pair in dicParamValues) 
            {
                strData += pair.Key + "=" + pair.Value + "&";
            }

            return strData.Remove(strData.Length - 1); ;
        }
    }
}
