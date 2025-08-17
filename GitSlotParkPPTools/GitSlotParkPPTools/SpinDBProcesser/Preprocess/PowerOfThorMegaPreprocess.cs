using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SpinDBProcesser
{
    public class PowerOfThorMegaPreprocess : SpinDBPreprocess
    {
        public PowerOfThorMegaPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }

        public async Task calculateFreeWinRate()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string          strCommand  = "UPDATE spins SET freewinrate = NULL";
                SQLiteCommand   command     = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<SpinData> startSpinDatas = new List<SpinData>();
                strCommand = "SELECT * FROM spins WHERE spintype=100";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int     id              = (int) (long) reader["id"];
                        double  realOdd         = (double)reader["realodd"];
                        int     freeSpinType    = (int)(long)reader["freespintype"];

                        startSpinDatas.Add(new SpinData(id, realOdd, freeSpinType));
                    }
                }
                double[] moveProbs = new double[] { 0.6281, 0.7039, 0.7502 };
                double[] meanRates = new double[4];
                for(int i = 3; i >= 0; i--)
                {
                    double sumRate = 0.0;
                    int count = 0;
                    strCommand  = string.Format("SELECT * FROM spins WHERE spintype={0}", 200 + i);
                    command     = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            double odd = (double)reader["odd"];
                            sumRate += odd;
                            count++;
                        }
                    }
                    double meanRate = sumRate / count;
                    if(i == 3)
                    {
                        meanRates[i] = meanRate;
                        continue;
                    }
                    meanRates[i] = 0.5 * meanRate + 0.5 * moveProbs[i] * meanRates[i + 1];
                }

                
                var transaction = connection.BeginTransaction();
                for(int i = 0; i < startSpinDatas.Count; i++)
                {
                    strCommand = "UPDATE spins SET freewinrate=@freewinrate WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", startSpinDatas[i].ID);
                    double freeWinRate = startSpinDatas[i].RealOdd + meanRates[startSpinDatas[i].FreeSpinType];

                    command.Parameters.AddWithValue("@freewinrate", freeWinRate);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }

        }
        public async Task calculateMinFreeWinRate()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET minrate = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<SpinData> startSpinDatas = new List<SpinData>();
                strCommand = "SELECT * FROM spins WHERE spintype=100";
                command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)(long)reader["id"];
                        double realOdd = (double)reader["realodd"];
                        int freeSpinType = (int)(long)reader["freespintype"];

                        startSpinDatas.Add(new SpinData(id, realOdd, freeSpinType));
                    }
                }
                Dictionary<int, double> minRates = new Dictionary<int, double>();
                for(int k = 0; k < startSpinDatas.Count; k++)
                {
                    if (startSpinDatas[k].RealOdd > 50.0)
                        continue;

                    double[] moveProbs = new double[] { 0.6281, 0.7039, 0.7502 };
                    double[] meanRates = new double[4];
                    bool isNotCompleted = false;
                    for (int i = 3; i >= startSpinDatas[k].FreeSpinType; i--)
                    {
                        double sumRate = 0.0;
                        int count = 0;

                        double minOdd = 20.0 - startSpinDatas[k].RealOdd;
                        if (minOdd < 0.0)
                            minOdd = 0.0;
                        double maxOdd = 50.0 - startSpinDatas[k].RealOdd;
                        strCommand = string.Format("SELECT * FROM spins WHERE spintype={0} and odd >= @minodd and odd <= @maxodd", 200 + i);
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("@minodd", minOdd);
                        command.Parameters.AddWithValue("@maxodd", maxOdd);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                double odd = (double)reader["odd"];
                                sumRate += odd;
                                count++;
                            }
                        }
                        if (count == 0)
                        {
                            isNotCompleted = true;
                            break;
                        }

                        double meanRate = sumRate / count;
                        if (i == 3)
                        {
                            meanRates[i] = meanRate;
                            continue;
                        }
                        meanRates[i] = 0.5 * meanRate + 0.5 * moveProbs[i] * meanRates[i + 1];
                    }
                    if (isNotCompleted)
                        continue;

                    minRates.Add(startSpinDatas[k].ID, startSpinDatas[k].RealOdd + meanRates[startSpinDatas[k].FreeSpinType]);
                }
                


                var transaction = connection.BeginTransaction();
                foreach(KeyValuePair<int, double> pair in minRates)
                {
                    strCommand = "UPDATE spins SET minrate=@minrate WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", pair.Key);
                    command.Parameters.AddWithValue("@minrate", pair.Value);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }

        }

        public async Task setRanges()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<SpinData> spinDatas = new List<SpinData>();
                string strCommand = "SELECT id, realodd, freespintype FROM spins WHERE spintype=100";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        spinDatas.Add(new SpinData((int)(long)reader["id"], (double)reader["realodd"], (int)(long)reader["freespintype"]));
                    }
                }
                strCommand = "UPDATE spins SET ranges = NULL";
                command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                Dictionary<int, List<int>> rangeUpdates = new Dictionary<int, List<int>>();
                for (int i = 0; i < spinDatas.Count; i++)
                {
                    int freeSpinType = spinDatas[i].FreeSpinType;
                    List<int> ranges = new List<int>();
                    for (int j = 0; j < _minRanges.Length; j++)
                    {
                        if (spinDatas[i].RealOdd > _maxRanges[j])
                            continue;

                        double minOdd = _minRanges[j] - spinDatas[i].RealOdd;
                        if (minOdd < 0.0)
                            minOdd = 0.0;
                        double maxOdd = _maxRanges[j] - spinDatas[i].RealOdd;

                        bool isNotCompleted = false;

                        for (int k = 3; k >= spinDatas[i].FreeSpinType; k--)
                        {
                            int count = 0;
                            strCommand = string.Format("SELECT COUNT(*) FROM spins WHERE spintype={0} and odd >= @minodd and odd <= @maxodd", 200 + k);
                            command = new SQLiteCommand(strCommand, connection);
                            command.Parameters.AddWithValue("@minodd", minOdd);
                            command.Parameters.AddWithValue("@maxodd", maxOdd);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                    count = (int)(long)reader[0];
                            }
                            if (count == 0)
                            {
                                isNotCompleted = true;
                                break;
                            }                            
                        }
                        if(isNotCompleted)                        
                            continue;
                        ranges.Add(j);
                    }
                    rangeUpdates.Add(spinDatas[i].ID, ranges);
                }                
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (KeyValuePair<int, List<int>> pair in rangeUpdates)
                    {
                        strCommand = "UPDATE spins SET ranges=@ranges WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@id", pair.Key);
                        command.Parameters.AddWithValue("@ranges", string.Join(",", pair.Value.ToArray()));
                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();
                }
            }


        }


    }
}
