using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SpinDBProcesser
{
    public class HandOfMidasPreprocess : SpinDBPreprocess
    {
        public HandOfMidasPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        public virtual async Task setupFreeSpinType()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET freespintype = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                Dictionary<int, int> freeSpinTypes = new Dictionary<int, int>();
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

                        string[] strParts = dicParams["is"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        int scatterCount = 0;
                        for (int i = 0; i < strParts.Length; i++)
                        {
                            if (int.Parse(strParts[i]) == 1)
                                scatterCount++;
                        }
                        if (scatterCount == 3)
                            freeSpinTypes[id] = 0;
                        else if (scatterCount == 4)
                            freeSpinTypes[id] = 1;
                        else if (scatterCount == 5)
                            freeSpinTypes[id] = 2;
                        else
                            break;
                    }
                }

                var transaction = connection.BeginTransaction();
                foreach (KeyValuePair<int, int> pair in freeSpinTypes)
                {
                    strCommand = "UPDATE spins SET freespintype=@freespintype WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", pair.Key);
                    command.Parameters.AddWithValue("@freespintype", pair.Value);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }
        }

    }
}
