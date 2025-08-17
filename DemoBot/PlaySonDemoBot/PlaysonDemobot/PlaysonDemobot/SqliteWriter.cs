using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaysonDemobot
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
                            string strCommand = "INSERT INTO spins (spintype, odd, realodd, data, freespintype) VALUES (@spintype, @odd, @realodd, @data, @freespintype)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype",        request.SpinDatas[i].SpinType);
                            command.Parameters.AddWithValue("@odd",             Math.Round(request.SpinDatas[i].SpinOdd, 2));
                            command.Parameters.AddWithValue("@realodd",         Math.Round(request.SpinDatas[i].RealOdd, 2));
                            command.Parameters.AddWithValue("@data",            request.SpinDatas[i].Response);
                            command.Parameters.AddWithValue("@freespintype",    request.SpinDatas[i].FreeSpinType);
                            await command.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            string strCommand = "INSERT INTO spins (spintype, odd,  data) VALUES (@spintype, @odd, @data)";
                            if(request.SpinDatas[i] is PurSpinData)
                                strCommand = "INSERT INTO spins (spintype, odd,  data, puri) VALUES (@spintype, @odd, @data, @puri)";

                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype",    request.SpinDatas[i].SpinType);
                            command.Parameters.AddWithValue("@odd",         Math.Round(request.SpinDatas[i].SpinOdd, 2));
                            command.Parameters.AddWithValue("@data",        request.SpinDatas[i].Response);
                            if(request.SpinDatas[i] is PurSpinData)
                                command.Parameters.AddWithValue("@puri", (request.SpinDatas[i] as PurSpinData).Puri);
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

        public async Task createMultiFreeGameTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();

                    string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'realodd' real NOT NULL DEFAULT 0.0, 'data' text NOT NULL, 'freespintype' INTEGER, 'ranges' text, minrate real)");

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
    
    public class PurSpinData : SpinData
    {
        public int Puri { get; set; }
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
