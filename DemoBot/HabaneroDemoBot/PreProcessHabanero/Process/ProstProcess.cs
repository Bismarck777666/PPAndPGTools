using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    public class ProstProcess : SpinDBPreprocess
    {
        public ProstProcess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }

        public override async Task preprocessDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<int> notSuitableIds = new List<int>();
                string strCommand = "SELECT id, data FROM spins";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id      = (int)(long)reader["id"];
                        string data = (string)reader["data"];

                        List<string> spinResponses = new List<string>(data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(spinResponses[0]);
                        if(!object.ReferenceEquals(resultContext["prost_mapActionList"],null) && resultContext["prost_mapActionList"].Count > 0)
                        {
                            for(int i = 0; i < resultContext["prost_mapActionList"].Count; i++)
                            {
                                int addedCnt    = resultContext["prost_mapActionList"][i]["addedNodeList"].Count;
                                int removeCnt   = resultContext["prost_mapActionList"][i]["removeNodeList"].Count;
                                if (addedCnt > 1 || removeCnt > 1)
                                    notSuitableIds.Add(id);
                            }
                        }
                    }
                }
            }
        }
    }
}
