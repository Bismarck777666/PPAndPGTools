using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;

namespace BNGSpinFetcher
{
    class SqliteWriter
    {
        private string _strConnectionString = null;
        private string _strGameName = "";
        public async Task initialize(string strGameName)
        {
            try
            {
                string appPath      = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath  = Path.Combine(appPath, string.Format("{0}.db", strGameName));

                _strGameName = strGameName;
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
        public async Task writeGroupedSpinDataResponse(List<GroupedSpinDataResponse> responseList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    for (int i = 0; i < responseList.Count; i++)
                    {
                        string strCommand = "INSERT INTO spins (spintype, odd, data, count, odds) VALUES (@spintype, @odd, @data, @count, @odds)";
                        SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@spintype", 0);
                        command.Parameters.AddWithValue("@odd", responseList[i].TotalOdd);
                        command.Parameters.AddWithValue("@data", string.Join("###", responseList[i].Responses.ToArray()));
                        command.Parameters.AddWithValue("@count", string.Join(",", responseList[i].SpinCount));
                        command.Parameters.AddWithValue("@odds", string.Join(",", responseList[i].SpinOdds));
                        await command.ExecuteNonQueryAsync();
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
        public async Task writeGirlsResponse(List<PinupGirlsResponse> responseList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    for (int i = 0; i < responseList.Count; i++)
                    {
                        string strCommand = "INSERT INTO spins (spintype, odd, data, spintypes, odds) VALUES (@spintype, @odd, @data, @spintypes, @odds)";
                        SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@spintype",    0);
                        command.Parameters.AddWithValue("@odd",         responseList[i].TotalOdd);
                        command.Parameters.AddWithValue("@data",        string.Join("###", responseList[i].Responses.ToArray()));
                        command.Parameters.AddWithValue("@spintypes",   string.Join(",", responseList[i].SpinTypes));
                        command.Parameters.AddWithValue("@odds",        string.Join(",", responseList[i].SpinOdds));
                        await command.ExecuteNonQueryAsync();
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
        public async Task writeSequenceData(List<BroncoSequence> sequenceList)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    for (int i = 0; i < sequenceList.Count; i++)
                    {
                        string strCommand = "INSERT INTO sequences (odd, data) VALUES (@odd, @data)";
                        SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@odd",     Math.Round(sequenceList[i].getTotalOdd(), 2));
                        command.Parameters.AddWithValue("@data",    sequenceList[i].toString());
                        await command.ExecuteNonQueryAsync();
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

                        if(request.SpinDatas[i] is BroncoSpinData)
                        {
                            if (request.SpinDatas[i].SpinType == 100)
                            {
                                string strCommand = "INSERT INTO spins (spintype, odd, realodd, wilds, islast, data, freespintype) VALUES (@spintype, @odd, @realodd, @wilds, @islast, @data, @freespintype)";
                                SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                                command.Parameters.AddWithValue("@spintype",        request.SpinDatas[i].SpinType);
                                command.Parameters.AddWithValue("@odd",             Math.Round(request.SpinDatas[i].SpinOdd, 2));
                                command.Parameters.AddWithValue("@realodd",         Math.Round(request.SpinDatas[i].RealOdd, 2));
                                command.Parameters.AddWithValue("@data",            request.SpinDatas[i].Response);
                                command.Parameters.AddWithValue("@freespintype",    request.SpinDatas[i].FreeSpinType);
                                command.Parameters.AddWithValue("@wilds",           (request.SpinDatas[i] as BroncoSpinData).Wilds);
                                command.Parameters.AddWithValue("@islast",          (request.SpinDatas[i] as BroncoSpinData).IsLast);
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                string strCommand = "INSERT INTO spins (spintype, odd,  wilds, islast, data) VALUES (@spintype, @odd,@wilds,@islast,@data)";
                                SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                                command.Parameters.AddWithValue("@spintype",    request.SpinDatas[i].SpinType);
                                command.Parameters.AddWithValue("@odd",         Math.Round(request.SpinDatas[i].SpinOdd, 2));
                                command.Parameters.AddWithValue("@data",        request.SpinDatas[i].Response);
                                command.Parameters.AddWithValue("@wilds",       (request.SpinDatas[i] as BroncoSpinData).Wilds);
                                command.Parameters.AddWithValue("@islast",      (request.SpinDatas[i] as BroncoSpinData).IsLast);

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        else if(request.SpinDatas[i] is CashElevatorSpinData)
                        {
                            string strCommand = "INSERT INTO spins (spintype, beginfloor, endfloor, odd,  data) VALUES (@spintype,@beginfloor,@endfloor, @odd, @data)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype", request.SpinDatas[i].SpinType);
                            command.Parameters.AddWithValue("@odd", Math.Round(request.SpinDatas[i].SpinOdd, 2));
                            command.Parameters.AddWithValue("@data", request.SpinDatas[i].Response);
                            command.Parameters.AddWithValue("@beginfloor", (request.SpinDatas[i] as CashElevatorSpinData).BeginFloor);
                            command.Parameters.AddWithValue("@endfloor", (request.SpinDatas[i] as CashElevatorSpinData).EndFloor);
                            await command.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            if (request.SpinDatas[i].SpinType == 100)
                            {
                                string strCommand = "INSERT INTO spins (spintype, odd, realodd, data, freespintype) VALUES (@spintype, @odd, @realodd, @data, @freespintype)";
                                if(_strGameName == "ReleaseTheKraken2")
                                    strCommand = "INSERT INTO spins (spintype, odd, realodd, data, freespintypes) VALUES (@spintype, @odd, @realodd, @data, @freespintypes)";

                                SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                                command.Parameters.AddWithValue("@spintype", request.SpinDatas[i].SpinType);
                                command.Parameters.AddWithValue("@odd", Math.Round(request.SpinDatas[i].SpinOdd, 2));
                                command.Parameters.AddWithValue("@realodd", Math.Round(request.SpinDatas[i].RealOdd, 2));
                                command.Parameters.AddWithValue("@data", request.SpinDatas[i].Response);
                                if (_strGameName == "ReleaseTheKraken2")
                                    command.Parameters.AddWithValue("@freespintypes", request.SpinDatas[i].FreeSpinTypes);
                                else
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
        public async Task createCashElevatorGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'beginfloor' INTEGER NOT NULL, 'endfloor' INTEGER NOT NULL,'odd' real NOT NULL, 'data' text NOT NULL)");

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
        public async Task createBroncoGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'wilds' INTEGER NOT NULL, 'islast' INTEGER NOT NULL, 'odd' real NOT NULL,  'realodd' real NOT NULL DEFAULT 0.0, 'data' text NOT NULL, 'freespintype' INTEGER, 'ranges' text, minrate real)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'sequences' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'odd' real NOT NULL, 'data' text NOT NULL)");
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index1' ON 'spins' ('id' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index2' ON 'spins' ('spintype' ASC, 'wilds' ASC, 'islast' ASC, 'odd' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = "CREATE INDEX IF NOT EXISTS 'spin_index3' ON 'spins' ('spintype' ASC, 'wilds' ASC, 'islast' ASC, 'ranges' ASC)";
                    sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                }
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

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'realodd' real NOT NULL DEFAULT 0.0, 'data' text NOT NULL, 'freespintype' INTEGER, 'ranges' text, minrate real, allfreewinrate real)");
                    if(_strGameName == "ReleaseTheKraken2")
                        strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'realodd' real NOT NULL DEFAULT 0.0, 'data' text NOT NULL, 'freespintypes' text, 'ranges' text, minrate real, allfreewinrate real)");
                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'info' ('normalmaxid' INTEGER NOT NULL,'defaultbet' real NOT NULL)");
                    sqlCommand = new SQLiteCommand(strCommand, connection);
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

                    strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'info' ('normalmaxid' INTEGER NOT NULL,'defaultbet' real NOT NULL)");
                    sqlCommand = new SQLiteCommand(strCommand, connection);
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
        public async Task createSequenceGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'data' text NOT NULL, 'spintypes' text, 'odds' text)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'info' ('normalmaxid' INTEGER NOT NULL,'defaultbet' real NOT NULL)");
                    sqlCommand = new SQLiteCommand(strCommand, connection);
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
        public async Task createGroupedSpinDataGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'data' text NOT NULL, 'count' INTEGER NOT NULL, 'odds' text NOT NULL)");

                    SQLiteCommand sqlCommand = new SQLiteCommand(strCommand, connection);
                    await sqlCommand.ExecuteNonQueryAsync();

                    strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'info' ('normalmaxid' INTEGER NOT NULL,'defaultbet' real NOT NULL)");
                    sqlCommand = new SQLiteCommand(strCommand, connection);
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

    }

    public class SpinData
    {
        public int      SpinType        { get; set; }        
        public int      FreeSpinType    { get; set; }
        public string   FreeSpinTypes   { get; set; }
        public double   SpinOdd         { get; set; }
        public double   RealOdd         { get; set; }
        public string   Response        { get; set; }
        public List<SpinData> ChildSpins { get; set; }
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
}
