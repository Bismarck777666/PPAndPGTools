using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SpinDBProcesser
{
    class BroncoSpiritPreprocess : SpinDBPreprocess
    {
        public BroncoSpiritPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override int FreeSpinOptions
        {
            get { return 2; }
        }

        public async Task updateSequences()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                List<string> sequenceList = new List<string>();
                string strCommand = "SELECT * FROM sequences";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sequenceList.Add((string)reader["data"]);
                    }
                }
                Dictionary<string, int> dicSeqProbs = new Dictionary<string, int>();
                for(int i = 0; i < sequenceList.Count; i++)
                {
                    string[] strParts = sequenceList[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (strParts.Length != 10)
                        throw new Exception();

                    List<int> wildCounts = new List<int>();
                    for(int j = 0; j < strParts.Length; j++)
                    {
                        string[] strTempParts = strParts[j].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (strTempParts.Length != 2)
                            throw new Exception();

                        wildCounts.Add(int.Parse(strTempParts[1]));
                    }
                    string strSequence = string.Join(",", wildCounts.OrderBy(x => x));
                    if (dicSeqProbs.ContainsKey(strSequence))
                        dicSeqProbs[strSequence]++;
                    else
                        dicSeqProbs[strSequence] = 1;
                }
                strCommand = "DELETE FROM sequence";
                command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                using (var transaction = connection.BeginTransaction())
                {

                    foreach(KeyValuePair<string, int> pair in dicSeqProbs)
                    {
                        strCommand = "INSERT INTO sequence(sequence, count) VALUES (@sequence, @count)";
                        command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@sequence", pair.Key);
                        command.Parameters.AddWithValue("@count", pair.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();
                }

            }

        }
        public async Task updateGroupForLastSpins()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                List<SpinData> spinDatas = new List<SpinData>();
                string strCommand       = "SELECT id, data FROM spins WHERE islast=1";
                SQLiteCommand command   = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        SpinData spinData = new SpinData((int)(long)reader["id"], 0.0, 0);
                        spinData.Data = (string)reader["data"];
                        spinDatas.Add(spinData);
                    }
                }
                using(var transaction = connection.BeginTransaction())
                {
                    for (int i = 0; i < spinDatas.Count; i++)
                    {
                        string strData = spinDatas[i].Data;
                        Dictionary<string, string> dicParams = splitResponseToParams(strData);
                        string strAccv = dicParams["accv"];
                        string[] strParts = strAccv.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (strParts.Length != 5)
                            throw new Exception();

                        int wildCount = int.Parse(strParts[3]);

                        strCommand = "UPDATE spins SET groupid=@groupid WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@id", spinDatas[i].ID);
                        command.Parameters.AddWithValue("@groupid", wildCount);

                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();
                }

            }
        }

        public override async Task calculateNormalFreeSpinRate()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET normalfreewinrate = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<SpinData> spinDatas = new List<SpinData>();
                strCommand = "SELECT id, realodd, freespintype FROM spins WHERE spintype=100 and id <= @maxid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        spinDatas.Add(new SpinData((int)(long)reader["id"], (double)reader["realodd"], (int)(long)reader["freespintype"]));
                    }
                }
                Dictionary<int, double> averageWinRates = new Dictionary<int, double>();
                for (int i = 0; i < spinDatas.Count; i++)
                {
                    int freeSpinType = spinDatas[i].FreeSpinType;
                    int[] spinTypes = AvailableSpinTypes(freeSpinType);

                    strCommand = string.Format("SELECT odd,  spintype FROM spins WHERE (spintype in ({0})) and id <= @maxid", string.Join(",", spinTypes));
                    command = new SQLiteCommand(strCommand, connection);
                    command.Parameters.AddWithValue("@maxid", _normalMaxID);
                    Dictionary<int, List<double>> dicSpinOdds = new Dictionary<int, List<double>>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int spinType = (int)(long)reader["spintype"];
                            if (!dicSpinOdds.ContainsKey(spinType))
                                dicSpinOdds.Add(spinType, new List<double>());
                            dicSpinOdds[spinType].Add((double)reader["odd"]);
                        }
                    }
                    if (spinTypes.Length != dicSpinOdds.Count)
                        continue;

                    double sumRate = 0.0;
                    foreach (KeyValuePair<int, List<double>> pair in dicSpinOdds)
                    {
                        double sum = 0.0;
                        for (int k = 0; k < pair.Value.Count; k++)
                            sum += pair.Value[k];

                        sumRate += (sum / pair.Value.Count);
                    }
                    sumRate = sumRate / dicSpinOdds.Count;
                    averageWinRates.Add(spinDatas[i].ID, sumRate + spinDatas[i].RealOdd);
                }

                var transaction = connection.BeginTransaction();
                foreach (KeyValuePair<int, double> pair in averageWinRates)
                {
                    strCommand = "UPDATE spins SET normalfreewinrate=@normalfreewinrate WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", pair.Key);
                    command.Parameters.AddWithValue("@normalfreewinrate", pair.Value);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();

                Dictionary<int, double> normalWildSumOdds = new Dictionary<int, double>();
                Dictionary<int, double> normalWildCounts = new Dictionary<int, double>();

                strCommand = "SELECT groupid, sum(odd), count(*) FROM spins WHERE spintype=0 and islast = 0 and id <= @maxid GROUP BY groupid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int groupID = (int)(long)reader[0];
                        double sumOdd = (double)reader[1];
                        int count = (int)(long)reader[2];

                        normalWildSumOdds[groupID] = sumOdd;
                        normalWildCounts[groupID] = count;
                    }
                }
                Dictionary<int, double> freeWildSumOdds = new Dictionary<int, double>();
                Dictionary<int, double> freeWildCounts = new Dictionary<int, double>();

                strCommand = "SELECT groupid, sum(normalfreewinrate), count(*) FROM spins WHERE spintype=100 and islast = 0 and id <= @maxid GROUP BY groupid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int groupID = (int)(long)reader[0];
                        double sumOdd = (double)reader[1];
                        int count = (int)(long)reader[2];

                        freeWildSumOdds[groupID] = sumOdd;
                        freeWildCounts[groupID] = count;
                    }
                }
                Dictionary<int, double> normalWildMeanOdds = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in normalWildSumOdds)
                {
                    if (freeWildSumOdds.ContainsKey(pair.Key))
                        normalWildMeanOdds[pair.Key] = (normalWildSumOdds[pair.Key] + freeWildSumOdds[pair.Key]) / (normalWildCounts[pair.Key] + freeWildCounts[pair.Key]);
                    else
                        normalWildMeanOdds[pair.Key] = (normalWildSumOdds[pair.Key]) / (normalWildCounts[pair.Key]);
                }
                foreach (KeyValuePair<int, double> pair in freeWildSumOdds)
                {
                    if (!normalWildMeanOdds.ContainsKey(pair.Key))
                        normalWildMeanOdds[pair.Key] = (freeWildSumOdds[pair.Key]) / (freeWildCounts[pair.Key]);
                }

                Dictionary<int, double> lastNormalWildSumOdds = new Dictionary<int, double>();
                Dictionary<int, double> lastNormalWildCounts = new Dictionary<int, double>();

                strCommand = "SELECT groupid, sum(odd), count(*) FROM spins WHERE spintype=0 and islast = 1 and id <= @maxid GROUP BY groupid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int groupID = (int)(long)reader[0];
                        double sumOdd = (double)reader[1];
                        int count = (int)(long)reader[2];

                        lastNormalWildSumOdds[groupID] = sumOdd;
                        lastNormalWildCounts[groupID] = count;
                    }
                }
                Dictionary<int, double> lastFreeWildSumOdds = new Dictionary<int, double>();
                Dictionary<int, double> lastFreeWildCounts = new Dictionary<int, double>();

                strCommand = "SELECT groupid, sum(normalfreewinrate), count(*) FROM spins WHERE spintype=100 and islast = 1 and id <= @maxid GROUP BY groupid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int groupID = (int)(long)reader[0];
                        double sumOdd = (double)reader[1];
                        int count = (int)(long)reader[2];

                        lastFreeWildSumOdds[groupID] = sumOdd;
                        lastFreeWildCounts[groupID] = count;
                    }
                }

                Dictionary<int, double> lastWildMeanOdds = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in lastNormalWildSumOdds)
                {
                    if (lastFreeWildSumOdds.ContainsKey(pair.Key))
                        lastWildMeanOdds[pair.Key] = (lastNormalWildSumOdds[pair.Key] + lastFreeWildSumOdds[pair.Key]) / (lastNormalWildCounts[pair.Key] + lastFreeWildCounts[pair.Key]);
                    else
                        lastWildMeanOdds[pair.Key] = (lastNormalWildSumOdds[pair.Key]) / (lastNormalWildCounts[pair.Key]);
                }
                foreach (KeyValuePair<int, double> pair in lastFreeWildSumOdds)
                {
                    if (!lastWildMeanOdds.ContainsKey(pair.Key))
                        lastWildMeanOdds[pair.Key] = (lastFreeWildSumOdds[pair.Key]) / (lastFreeWildCounts[pair.Key]);
                }

                Dictionary<string, int> sequences = new Dictionary<string, int>();
                strCommand = "SELECT * FROM sequence";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sequences.Add((string)reader["sequence"], (int)(long)reader["count"]);
                    }
                }
                double totalOdd = 0.0;
                int totalCount = 0;
                foreach (KeyValuePair<string, int> pair in sequences)
                {
                    string[] strParts = pair.Key.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < strParts.Length; i++)
                    {
                        int groupID = int.Parse(strParts[i]);
                        if (i == 9)
                            totalOdd += (lastWildMeanOdds[groupID] * pair.Value);
                        else
                            totalOdd += (normalWildMeanOdds[groupID] * pair.Value);
                    }
                    totalCount += (pair.Value * 10);
                }
                double payoutRate = totalOdd / totalCount;
            }
        }

    }
}
