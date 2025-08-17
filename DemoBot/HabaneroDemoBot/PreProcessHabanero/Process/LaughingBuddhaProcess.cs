using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    public class LaughingBuddhaProcess : SpinDBPreprocess
    {
        public LaughingBuddhaProcess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get
            {
                return true;
            }
        }

        protected override int FreeSpinOptions
        {
            get
            {
                return 9;
            }
        }

        protected override int[] AvailableSpinTypes(int freeSpinType)
        {
            List<int> spinTypes = new List<int>();
            for (int i = 0; i < FreeSpinOptions; i++)
                spinTypes.Add(205 + freeSpinType * FreeSpinOptions + i);

            return spinTypes.ToArray();
        }

        public override async Task decreaseFreeStartWin()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                double defaultbet = 1.0;
                int normalmaxid = 200000;
                string strCommand = "SELECT defaultbet,normalmaxid FROM info";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        defaultbet = (double)reader["defaultbet"];
                        normalmaxid = (int)(long)reader["normalmaxid"];
                    }
                }

                List<SpinData> startSpinDatas = new List<SpinData>();
                List<SpinData> childSpinDatas = new List<SpinData>();
                strCommand = "SELECT id,spintype,odd ,realodd, freespintype,data FROM spins WHERE spintype>=100 ORDER BY id DESC";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int spinType = (int)(long)reader["spintype"];
                        if (spinType == 100)
                        {
                            SpinData startData = new SpinData((int)(long)reader["id"], (double)reader["realodd"], 0);
                            startData.SpinType = spinType;
                            startData.Data = (string)reader["data"];
                            startData.Odd = (double)reader["odd"];
                            startSpinDatas.Add(startData);
                        }
                        else
                        {
                            SpinData childData = new SpinData((int)(long)reader["id"], (double)reader["realodd"], 0);
                            childData.SpinType = spinType;
                            childData.Data = (string)reader["data"];
                            childData.Odd = (double)reader["odd"];
                            childSpinDatas.Add(childData);
                        }
                    }
                }

                List<int> notSuitables = new List<int>();
                try
                {
                    for (int i = 0; i < startSpinDatas.Count; i++)
                    {
                        int id = startSpinDatas[i].ID;
                        int childIndex = childSpinDatas.FindIndex(_ => _.ID == id + 1);

                        if (childIndex == -1)
                        {
                            notSuitables.Add(id);
                            continue;
                        }
                        double originOdd = startSpinDatas[i].Odd;
                        double calcedOdd = startSpinDatas[i].RealOdd + childSpinDatas[childIndex].RealOdd;


                        dynamic startContext    = JsonConvert.DeserializeObject<dynamic>(startSpinDatas[i].Data);
                        double startPayOut      = (double)startContext["totalwincash"];

                        List<string> spinResponses  = new List<string>(childSpinDatas[childIndex].Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> newResponses   = new List<string>();

                        foreach (string childString in spinResponses)
                        {
                            dynamic childContext    = JsonConvert.DeserializeObject<dynamic>(childString);
                            if (!object.ReferenceEquals(childContext["totalwincash"], null))
                            {
                                double totalpayout  = (double)childContext["totalwincash"];
                                totalpayout         = Math.Max(0, Math.Round(totalpayout - startPayOut, 2));
                                childContext["totalwincash"] = totalpayout;
                            }
                            newResponses.Add(JsonConvert.SerializeObject(childContext));
                        }
                        childSpinDatas[childIndex].Data = string.Join("\n", newResponses);
                    }

                    for (int i = 0; i < notSuitables.Count; i++)
                    {
                        strCommand = "DELETE FROM spins WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("id", notSuitables[i]);

                        await command.ExecuteNonQueryAsync();
                    }

                    for (int i = 0; i < childSpinDatas.Count; i++)
                    {
                        strCommand = "UPDATE spins SET data = @data WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("data", childSpinDatas[i].Data);
                        command.Parameters.AddWithValue("id", childSpinDatas[i].ID);

                        await command.ExecuteNonQueryAsync();
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public override async Task increaseFreeStartOdd()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                double defaultbet = 1.0;
                int normalmaxid = 200000;
                string strCommand = "SELECT defaultbet,normalmaxid FROM info";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        defaultbet  = (double)reader["defaultbet"];
                        normalmaxid = (int)(long)reader["normalmaxid"];
                    }
                }

                List<SpinData> startSpinDatas = new List<SpinData>();
                List<SpinData> childSpinDatas = new List<SpinData>();
                strCommand = "SELECT id,spintype,odd ,realodd, freespintype,data FROM spins WHERE spintype>=100 ORDER BY id DESC";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int spinType = (int)(long)reader["spintype"];
                        if (spinType == 100)
                        {
                            SpinData startData = new SpinData((int)(long)reader["id"], (double)reader["realodd"], 0);
                            startData.SpinType = spinType;
                            startData.Data = (string)reader["data"];
                            startData.Odd = (double)reader["odd"];
                            startSpinDatas.Add(startData);
                        }
                        else
                        {
                            SpinData childData = new SpinData((int)(long)reader["id"], (double)reader["realodd"], 0);
                            childData.SpinType = spinType;
                            childData.Data = (string)reader["data"];
                            childData.Odd = (double)reader["odd"];
                            childSpinDatas.Add(childData);
                        }
                    }
                }

                List<int> notSuitables = new List<int>();
                try
                {
                    for (int i = 0; i < startSpinDatas.Count; i++)
                    {
                        int id = startSpinDatas[i].ID;
                        int childIndex = childSpinDatas.FindIndex(_ => _.ID == id + 1);

                        if (childIndex == -1)
                        {
                            notSuitables.Add(id);
                            continue;
                        }
                        double originOdd = startSpinDatas[i].Odd;
                        double calcedOdd = startSpinDatas[i].RealOdd + childSpinDatas[childIndex].RealOdd;


                        dynamic startContext    = JsonConvert.DeserializeObject<dynamic>(startSpinDatas[i].Data);
                        double startPayOut      = (double)startContext["totalwincash"];

                        List<string> spinResponses  = new List<string>(childSpinDatas[childIndex].Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        List<string> newResponses   = new List<string>();

                        dynamic childContext    = JsonConvert.DeserializeObject<dynamic>(spinResponses.Last());
                        double totalpayout      = (double)childContext["totalwincash"];
                        double startWinOdd      = Math.Round((startPayOut + totalpayout) / defaultbet,2);

                        startSpinDatas[i].Odd   = startWinOdd;
                    }

                    for (int i = 0; i < notSuitables.Count; i++)
                    {
                        strCommand = "DELETE FROM spins WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("id", notSuitables[i]);

                        await command.ExecuteNonQueryAsync();
                    }

                    for (int i = 0; i < startSpinDatas.Count; i++)
                    {
                        strCommand = "UPDATE spins SET odd = @odd WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("odd", startSpinDatas[i].Odd);
                        command.Parameters.AddWithValue("id", startSpinDatas[i].ID);

                        await command.ExecuteNonQueryAsync();
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
