using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace SpinDBProcesser
{
    public class SpinDBPreprocess
    {
#if INCHON
        protected double[] _minRanges = new double[] { 20.0, 10.0, 50.0,  100.0, 300.0, 500.0,  1000.0, 1000.0, 1500.0 };
        protected double[] _maxRanges = new double[] { 50.0, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0, 1500.0, 3000.0};
#else
        protected double[] _minRanges = new double[] { 20.0, 10.0, 50.0,  100.0, 300.0, 500.0,  1000.0 };
        protected double[] _maxRanges = new double[] { 50.0, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0 };
#endif

        protected string _strGameName    = "";
        protected string _strFolderName  = "";
        protected int    _normalMaxID    = 0;
        protected string _strConnString = "";
        public SpinDBPreprocess(string strFolderName, string strGameName)
        {
            _strFolderName          = strFolderName;
            _strGameName            = strGameName;
            //string appPath          = Path.GetDirectoryName(_strFolderName);
            //string strFilePath      = Path.Combine(appPath, string.Format("{0}.db", _strGameName));
            string strFilePath      = string.Format("{0}.db", _strGameName);
            _strConnString          = @"Data Source=" + strFilePath;

        }
        protected virtual bool SupportPurchaseFree
        {
            get
            {
                return false;
            }
        }
        protected virtual int FreeSpinOptions
        {
            get
            {
                return 0;
            }
        }
        protected virtual int[] AvailableSpinTypes(int freeSpinType)
        {
            List<int> spinTypes = new List<int>();
            for (int i = 0; i < FreeSpinOptions; i++)
                spinTypes.Add(200 + freeSpinType * FreeSpinOptions + i);

            return spinTypes.ToArray();
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
        protected string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }


        public async Task readInfo()
        {
            using(SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                string strCommand = "SELECT * FROM info";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        _normalMaxID = (int)(long) reader["normalmaxid"];
                    }
                }
            }
        }

        public virtual async Task calculateNormalFreeSpinRate()
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


                int     normalSpinCount = 0;
                double  normalSpinOddSum = 0.0;
                strCommand = "SELECT sum(odd), count(*) FROM spins WHERE spintype=0 and id <= @maxid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        normalSpinOddSum = (double)reader[0];
                        normalSpinCount  = (int)(long)reader[1];
                    }
                }
                strCommand = "SELECT sum(normalfreewinrate), count(*) FROM spins WHERE spintype=100 and id <= @maxid";
                command = new SQLiteCommand(strCommand, connection);
                command.Parameters.AddWithValue("@maxid", _normalMaxID);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        normalSpinOddSum += (double)reader[0];
                        normalSpinCount  += (int)(long)reader[1];
                    }
                }

                double payoutRate = normalSpinOddSum / normalSpinCount;
            }
        }

        public async Task calculateAllFreeSpinRate()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();

                string strCommand = "UPDATE spins SET allfreewinrate = NULL";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                List<SpinData>  spinDatas   = new List<SpinData>();
                strCommand  = "SELECT id, realodd, freespintype FROM spins WHERE spintype=100";
                command     = new SQLiteCommand(strCommand, connection);
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

                    strCommand = string.Format("SELECT odd, spintype FROM spins WHERE (spintype in ({0}))", string.Join(",", spinTypes));
                    command = new SQLiteCommand(strCommand, connection);
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
                    strCommand = "UPDATE spins SET allfreewinrate=@allfreewinrate WHERE id=@id";
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", pair.Key);
                    command.Parameters.AddWithValue("@allfreewinrate", pair.Value);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
            }
        }

        public virtual async Task preprocessDB()
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
                strCommand = "UPDATE spins SET ranges = NULL, minrate = NULL";
                command = new SQLiteCommand(strCommand, connection);
                await command.ExecuteNonQueryAsync();

                Dictionary<int, List<int>> rangeUpdates = new Dictionary<int, List<int>>();
                Dictionary<int, double> minRateUpdates = new Dictionary<int, double>();
                for (int i = 0; i < spinDatas.Count; i++)
                {
                    int freeSpinType = spinDatas[i].FreeSpinType;
                    int[] spinTypes = AvailableSpinTypes(freeSpinType);

                    List<int> ranges = new List<int>();
                    for (int j = 0; j < _minRanges.Length; j++)
                    {
                        if (spinDatas[i].RealOdd > _maxRanges[j])
                            continue;

                        double minOdd = _minRanges[j] - spinDatas[i].RealOdd;
                        if (minOdd < 0.0)
                            minOdd = 0.0;
                        double maxOdd = _maxRanges[j] - spinDatas[i].RealOdd;

                        strCommand = string.Format("SELECT odd,  spintype FROM spins WHERE (spintype in ({0})) and (odd >= @minodd) and (odd <= @maxodd)", string.Join(",", spinTypes));
                        command = new SQLiteCommand(strCommand, connection);
                        command.Parameters.AddWithValue("@minodd", minOdd);
                        command.Parameters.AddWithValue("@maxodd", maxOdd);
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

                        ranges.Add(j);
                        if (SupportPurchaseFree && j == 0)
                        {
                            double sumRate = 0.0;
                            foreach (KeyValuePair<int, List<double>> pair in dicSpinOdds)
                            {
                                double sum = 0.0;
                                for (int k = 0; k < pair.Value.Count; k++)
                                    sum += pair.Value[k];

                                sumRate += (sum / pair.Value.Count);
                            }
                            sumRate = sumRate / dicSpinOdds.Count;
                            minRateUpdates.Add(spinDatas[i].ID, sumRate + spinDatas[i].RealOdd);
                        }
                    }
                    rangeUpdates.Add(spinDatas[i].ID, ranges);
                }
                using(var transaction = connection.BeginTransaction())
                {
                    foreach (KeyValuePair<int, double> pair in minRateUpdates)
                    {
                        strCommand = "UPDATE spins SET minrate=@minrate WHERE id=@id";
                        command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@id", pair.Key);
                        command.Parameters.AddWithValue("@minrate", pair.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();
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

    public class SpinData
    {
        public int      ID              { get; set; }
        public double   RealOdd         { get; set; }
        public int      FreeSpinType    { get; set; }
        public string   Data            { get; set; }
        public SpinData(int id, double realOdd, int freeSpinType)
        {
            this.ID             = id;
            this.RealOdd        = realOdd;
            this.FreeSpinType   = freeSpinType;
        }
    }
}
