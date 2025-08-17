using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SpinDBProcesser
{
    class OlympusGatePreprocess : SpinDBPreprocess
    {
        public OlympusGatePreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        public async Task check()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<SpinData> startSpinDatas = new List<SpinData>();
                string strCommand = "SELECT * FROM spins";
                var command = new SQLiteCommand(strCommand, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)(long)reader["id"];
                        string strData = (string)reader["data"];

                        string[] strDatas = strData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for(int  i = 0;  i < strDatas.Length; i++)
                        {
                            Dictionary<string, string> dicParams = splitResponseToParams(strDatas[i]);
                            if(dicParams.ContainsKey("apwa"))
                            {
                                string[] strParts = dicParams["apwa"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if(strParts.Length > 1)
                                {

                                }
                            }
                        }

                    }
                }

                
            }

        }
        public async Task setupPurEnabled()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET purenabled = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<SpinData> startSpinDatas = new List<SpinData>();
                strCommand = "SELECT * FROM spins WHERE spintype=1";
                command = new SQLiteCommand(strCommand, connection);
                List<int> purEnabledIDs = new List<int>();
                List<string> purSamples = new List<string>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)(long)reader["id"];
                        string strData = (string)reader["data"];

                        strData = strData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)[0];

                        Dictionary<string, string> dicParams = splitResponseToParams(strData);
                        if (dicParams.ContainsKey("tmb_win"))
                            continue;

                        if (dicParams.ContainsKey("psym"))
                        {
                            string [] strParts = dicParams["psym"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                            if (double.Parse(strParts[1]) != 0.6)
                                continue;


                            if (double.Parse(dicParams["tw"]) != 0.6)
                                continue;

                            string[] strPositions = strParts[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            bool isGood = true;
                            for (int i = 0; i < strPositions.Length; i++)
                            {
                                int position = int.Parse(strPositions[i]);
                                int col = position % 6;
                                if (col == 0 || col == 5)
                                {
                                    isGood = false;
                                    break;
                                }
                            }
                            if (isGood)
                                purSamples.Add(strData);

                            purEnabledIDs.Add(id);

                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                
                var transaction = connection.BeginTransaction();
                foreach (int id in purEnabledIDs)
                {
                    strCommand = "UPDATE spins SET purenabled=1 WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();

                transaction = connection.BeginTransaction();
                foreach (string  strData in purSamples)
                {
                    strCommand = "INSERT INTO pursamples (data) VALUES (@data)";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@data", strData);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }

        }
    }
}
