using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;

namespace PreProcessReelData
{
    public class SqliteDatabaseWork
    {
        private string _strConnectionString = null;
        //private SQLiteConnection _Connection = null;
        public async Task initialize(string strGameName)
        {
            try
            {
                string appPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath = Path.Combine(appPath, string.Format("slotdata\\{0}.db", strGameName));

                _strConnectionString = @"Data Source=" + strFilePath;
                SQLiteConnection connection = new SQLiteConnection(_strConnectionString);
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        
        public async Task<List<FreeOptionSpinData>> readSpinData()
        {
            try
            {
                List<FreeOptionSpinData> spinDataLists = new List<FreeOptionSpinData>();
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    string strCommand = "SELECT id, spintype, extra, odd, data, realodd, freespintype, ranges, minrate FROM spins WHERE spintype<>@spintype";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@spintype", 0);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long id             = (long)reader["id"];
                            int spingroup       = reader["freespintype"] != DBNull.Value ? (int)(long)reader["freespintype"] : -1;
                            long spintype       = (long)reader["spintype"];
                            long extra          = (long)reader["extra"];
                            double spinodd      = (double)reader["odd"];
                            string data         = (string)reader["data"];
                            double realodd      = (double)reader["realodd"];
                            string ranges       = reader["ranges"] != DBNull.Value ? (string)reader["ranges"] : "";

                            FreeOptionSpinData fsOptSpinData = new FreeOptionSpinData(id, (int)spintype, (int)extra, spinodd, data, realodd, spingroup, ranges, 0);
                            spinDataLists.Add(fsOptSpinData);
                        }
                    }
                    transaction.Commit();
                    connection.Close();
                }
                return spinDataLists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new List<FreeOptionSpinData>();
            }
        }

        public async Task updateSpinData(List<FreeOptionSpinData> request)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    for (int i = 0; i < request.Count; i++)
                    {
                        string strCommand = "UPDATE spins SET spintype=@spintype,freespintype=@freespintype,ranges=@ranges,minrate=@minrate WHERE id=@id";
                        SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                        command.Parameters.AddWithValue("@spintype", request[i].SpinType);
                        if(request[i].FreeSpinType != -1)
                        {
                            command.Parameters.AddWithValue("@freespintype",    request[i].FreeSpinType);
                            command.Parameters.AddWithValue("@ranges",          request[i].Ranges);
                            command.Parameters.AddWithValue("@minrate",         request[i].MinRate);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@freespintype",    null);
                            command.Parameters.AddWithValue("@ranges",          null);
                            command.Parameters.AddWithValue("@minrate",         null);
                        }
                        command.Parameters.AddWithValue("@id", request[i].Id);
                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();
                    connection.Close();
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
        public long     Id          { get; set; }
        public int      SpinType    { get; set; }
        public int      Extra       { get; set; }
        public double   SpinOdd     { get; set; }
        public string   Data        { get; set; }
    }
    public class FreeOptionSpinData: SpinData
    {
        public double   RealOdd         { get; set; }
        public int      FreeSpinType    { get; set; }
        public string   Ranges          { get; set; }
        public double   MinRate         { get; set; }

        public FreeOptionSpinData(long id,int spintype, int extra,double spinodd,string data, double realodd, int freespintype, string ranges, double meanrate)
        {
            this.Id             = id;
            this.SpinType       = spintype;
            this.Extra          = extra;
            this.SpinOdd        = spinodd;
            this.Data           = data;
            this.RealOdd        = realodd;
            this.FreeSpinType   = freespintype;
            this.Ranges         = ranges;
            this.MinRate        = meanrate;
        }
    }
}
