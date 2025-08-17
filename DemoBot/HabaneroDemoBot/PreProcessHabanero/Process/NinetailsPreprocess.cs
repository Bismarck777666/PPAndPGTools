using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    public class NinetailsPreprocess : SpinDBPreprocess
    {
        public NinetailsPreprocess(string strFolderName, string strGameName):
            base(strFolderName, strGameName)
        {

        }
        public override async Task preprocessDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<SpinData> spinDatas = new List<SpinData>();
                string strCommand = "SELECT id,odd, data FROM spins WHERE id > 228871";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        SpinData data = new SpinData((int)(long)reader["id"], (double)reader["odd"], 0);
                        data.Data = (string)reader["data"];
                        spinDatas.Add(data);
                    }
                }

                foreach (SpinData data in spinDatas)
                {
                    List<string> spinStrings = new List<string>(data.Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

                    for (int i = 0; i < spinStrings.Count; i++)
                    {
                        dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(spinStrings[i]);
                        spinStrings[i]          = JsonConvert.SerializeObject(resultContext["portmessage"]);
                    }
                    strCommand = string.Format("UPDATE spins SET data=@data WHERE id=@id");
                    command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@data", string.Join("\n", spinStrings));
                    command.Parameters.AddWithValue("@id", data.ID);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
