using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    public class LanternLuckProcess : SpinDBPreprocess
    {
        public LanternLuckProcess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }

        public override async Task preprocessDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<int> notSuitableIds = new List<int>();
                string strCommand = "SELECT id, data FROM spins WHERE spintype=1";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id      = (int)(long)reader["id"];
                        string data = (string)reader["data"];

                        List<string> spinResponses = new List<string>(data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        if (spinResponses.Count > 4)
                            notSuitableIds.Add(id);
                    }
                }
            }
        }
    }
}
