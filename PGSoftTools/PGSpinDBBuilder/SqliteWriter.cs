using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using System.Data;

namespace PGSpinDBBuilder
{
    class SqliteWriter
    {
        private string _strConnectionString = null;
        public async Task initialize(string strGameName)
        {
            try
            {
                string appPath      = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath  = Path.Combine(appPath, string.Format("{0}.db", strGameName));

                if (!File.Exists(strFilePath))
                {
                    SQLiteConnection.CreateFile(strFilePath);
                }
                _strConnectionString = @"Data Source=" + strFilePath;
                SQLiteConnection connection = new SQLiteConnection(_strConnectionString);
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task createMultiFreeGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'data' text NOT NULL, 'freespintype' INTEGER, 'ranges' text, minrate real)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index1' ON 'spins' ('id' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index2' ON 'spins' ('spintype' ASC, 'odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index3' ON 'spins' ('spintype' ASC, 'ranges' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public async Task writeSpinData(WriteSpinDataRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    int count = 0;
                    SQLiteTransaction transaction = null;
                    for (int i = 0; i < request.SpinDatas.Count; i++)
                    {
                        if (transaction == null)
                            transaction = connection.BeginTransaction();

                        if (request.SpinDatas[i].SpinType == 100)
                        {
                            string strCommand = "INSERT INTO spins (spintype, odd, data, freespintype) VALUES (@spintype, @odd, @data, @freespintype)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype", request.SpinDatas[i].SpinType);
                            command.Parameters.AddWithValue("@odd", Math.Round(request.SpinDatas[i].SpinOdd, 2));
                            command.Parameters.AddWithValue("@data", request.SpinDatas[i].Response);
                            command.Parameters.AddWithValue("@freespintype", request.SpinDatas[i].FreeSpinType);
                            await command.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            string strCommand = "INSERT INTO spins (spintype, odd,  data) VALUES (@spintype, @odd, @data)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype", request.SpinDatas[i].SpinType);
                            command.Parameters.AddWithValue("@odd", Math.Round(request.SpinDatas[i].SpinOdd, 2));
                            command.Parameters.AddWithValue("@data", request.SpinDatas[i].Response);
                            await command.ExecuteNonQueryAsync();

                        }

                        count++;

                        if (count >= 1000)
                        {
                            transaction.Commit();
                            count = 0;
                            transaction = null;
                        }
                    }
                    if (transaction != null)
                        transaction.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task writeGroupSpinData(WriteGroupSpinDataRequest request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    int count = 0;
                    SQLiteTransaction transaction = null;
                    for (int i = 0; i < request.SpinDatas.Count; i++)
                    {
                        if (transaction == null)
                            transaction = connection.BeginTransaction();

                        string          strCommand  = "INSERT INTO spins (spincount, odd,  data) VALUES (@spincount, @odd, @data)";
                        SQLiteCommand   command     = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@spincount", request.SpinDatas[i].SpinCount);
                        command.Parameters.AddWithValue("@odd", Math.Round(request.SpinDatas[i].SpinOdd, 2));
                        command.Parameters.AddWithValue("@data", request.SpinDatas[i].Response);
                        await command.ExecuteNonQueryAsync();

                        count++;

                        if (count >= 1000)
                        {
                            transaction.Commit();
                            count = 0;
                            transaction = null;
                        }
                    }
                    if (transaction != null)
                        transaction.Commit();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task createGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'data' text NOT NULL)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index1' ON 'spins' ('id' ASC, 'odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index2' ON 'spins' ('odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public async Task createGroupGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spincount' INTEGER NOT NULL, 'odd' real NOT NULL, 'data' text NOT NULL)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index1' ON 'spins' ('id' ASC, 'odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index2' ON 'spins' ('odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public async Task<int> getSpinCount()
        {
            try
            {
                int totalCount = 0;
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = string.Format("SELECT COUNT(id) as count FROM spins WHERE spintype <= 100");
                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);

                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            totalCount = (int)(long)reader["count"];
                    }
                }
                return totalCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }
        public async Task<int[]> countPerRanges(double[] minOdds, double[] maxOdds)
        {
            try
            {
                int[] counts = new int[minOdds.Length];
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    for (int i = 0; i < minOdds.Length; i++)
                    {
                        string strCommand = string.Format("SELECT COUNT(id) as count FROM spins WHERE odd >= @minOdd and odd < @maxOdd and spintype <= 100");
                        SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                        sqlCommand.Parameters.AddWithValue("@minOdd", minOdds[i]);
                        sqlCommand.Parameters.AddWithValue("@maxOdd", maxOdds[i]);

                        using (var reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                                counts[i] = (int)(long)reader["count"];
                        }

                    }
                }
                return counts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        public async Task<List<double>> getFreeSpinOdds()
        {
            try
            {
                List<double> freeSpinOdds = new List<double>();
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string          strCommand = string.Format("SELECT odd FROM spins WHERE spintype=1");
                    SQLiteCommand   sqlCommand = new SQLiteCommand(strCommand, connection);
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            freeSpinOdds.Add((double)reader["odd"]);
                        }
                    }
                }
                return freeSpinOdds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return new List<double>();
        }


    }

    public class SpinData
    {
        public int      SpinType        { get; set; }        
        public int      FreeSpinType    { get; set; }
        public double   SpinOdd         { get; set; }
        public double   RealOdd         { get; set; }
        public string   Response        { get; set; }
        public List<SpinData> ChildSpins { get; set; }
    }
    public class GroupSpinData
    {
        public int      SpinCount   { get; set; }
        public double   SpinOdd     { get; set; }
        public string   Response    { get; set; }
    }
    public class BroncoSpinData : SpinData
    {
        public int Wilds    { get; set; }
        public int IsLast   { get; set; }
    }

    public class CashElevatorSpinData : SpinData
    {
        public int BeginFloor   { get; set; }
        public int EndFloor     { get; set; }
    }
    public class WriteSpinDataRequest
    {
        public List<SpinData> SpinDatas { get; private set; }

        public WriteSpinDataRequest(List<SpinData> spinDatas)
        {
            this.SpinDatas = spinDatas;
        }
    }
    public class WriteGroupSpinDataRequest
    {
        public List<GroupSpinData> SpinDatas { get; private set; }

        public WriteGroupSpinDataRequest(List<GroupSpinData> spinDatas)
        {
            this.SpinDatas = spinDatas;
        }
    }
}
