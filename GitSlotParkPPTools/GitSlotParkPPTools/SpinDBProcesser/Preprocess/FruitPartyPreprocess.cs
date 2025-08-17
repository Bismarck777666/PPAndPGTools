using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SpinDBProcesser 
{
    public class FruitPartyPreprocess : SpinDBPreprocess
    {
        public FruitPartyPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        public virtual async Task setupPurEnabled()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET purenabled = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<int> purEnabledIds = new List<int>();
                strCommand = "SELECT * FROM spins WHERE spintype=1";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)(long)reader["id"];
                        string strData = (string)reader["data"];
                        strData = strData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        Dictionary<string, string> dicParams = splitResponseToParams(strData);

                        double totalWin = double.Parse(dicParams["tw"]);
                        if (totalWin == 0.0)
                            purEnabledIds.Add(id);
                    }
                }

                var transaction = connection.BeginTransaction();
                foreach (int id in purEnabledIds)
                {
                    strCommand = "UPDATE spins SET purenabled=1 WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }
        }
    }
}
