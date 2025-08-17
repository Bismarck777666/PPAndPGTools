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
                    string strCommand = "SELECT id,spintype,odd, data, realodd,freespintype,ranges,minrate FROM spins WHERE spintype<>@spintype";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@spintype", 0);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long id             = (long)reader["id"];
                            int spingroup       = reader["freespintype"] != DBNull.Value ? (int)(long)reader["freespintype"] : -1;
                            long spintype       = (long)reader["spintype"];
                            double spinodd      = (double)reader["odd"];
                            string data         = (string)reader["data"];
                            double realodd      = (double)reader["realodd"];
                            string ranges       = reader["ranges"] != DBNull.Value ? (string)reader["ranges"] : "";

                            FreeOptionSpinData fsOptSpinData = new FreeOptionSpinData(id, (int)spintype,spinodd,data, realodd, spingroup, ranges, 0);
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

        public async Task<List<SpinData>> readNormalFreeSpinData(int spintypenum)
        {
            try
            {
                List<SpinData> spinDataLists = new List<SpinData>();
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    //string strCommand = "SELECT id,spintype,odd, data FROM spins WHERE spintype=@spintype AND id <= (SELECT normalmaxid FROM info)";
                    string strCommand = "SELECT id,spintype,odd, data FROM spins WHERE spintype=@spintype";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@spintype", spintypenum);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long id             = (long)reader["id"];
                            long spintype       = (long)reader["spintype"];
                            double spinodd      = (double)reader["odd"];
                            string data         = (string)reader["data"];

                            SpinData spinData = new SpinData() { Id = id, Data = data, SpinOdd = spinodd, SpinType = (int)spintype };
                            spinDataLists.Add(spinData);
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
                return new List<SpinData>();
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
                            command.Parameters.AddWithValue("@freespintype", request[i].FreeSpinType);
                            command.Parameters.AddWithValue("@ranges", request[i].Ranges);
                            command.Parameters.AddWithValue("@minrate", request[i].MinRate);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@freespintype", null);
                            command.Parameters.AddWithValue("@ranges", null);
                            command.Parameters.AddWithValue("@minrate", null);
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

        public async Task deleteSpinData(string ids)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    string strCommand       = string.Format("DELETE FROM spins WHERE id IN ({0})",ids);
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection, transaction);
                    
                    await command.ExecuteNonQueryAsync();
                    transaction.Commit();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task updateSpinType(string ids, int spintype)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    string strCommand = string.Format("UPDATE spins SET spintype={0} WHERE id IN ({1})", spintype, ids);
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);

                    await command.ExecuteNonQueryAsync();
                    transaction.Commit();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task updateFreeSpinType(string ids,int freespintype)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    string strCommand       = string.Format("UPDATE spins SET freespintype={0} WHERE id IN ({1})", freespintype, ids);
                    SQLiteCommand command   = new SQLiteCommand(strCommand, connection, transaction);
                    
                    await command.ExecuteNonQueryAsync();
                    transaction.Commit();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //훗볼베이비
        public async Task insertGeneratedSpinData(SpinData spinData) 
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    string strCommand = string.Format("INSERT INTO spins (spintype,odd,data) VALUES (@spintype,@odd,@data)");
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@spintype",    spinData.SpinType);
                    command.Parameters.AddWithValue("@odd",         spinData.SpinOdd);
                    command.Parameters.AddWithValue("@data",        spinData.Data);

                    await command.ExecuteNonQueryAsync();
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
        public double   SpinOdd     { get; set; }
        public string   Data        { get; set; }
    }
    public class FreeOptionSpinData: SpinData
    {
        public double   RealOdd         { get; set; }
        public int      FreeSpinType    { get; set; }
        public string   Ranges          { get; set; }
        public double   MinRate        { get; set; }

        public FreeOptionSpinData(long id,int spintype,double spinodd,string data, double realodd, int freespintype, string ranges, double meanrate)
        {
            this.Id             = id;
            this.SpinType       = spintype;
            this.SpinOdd        = spinodd;
            this.Data           = data;
            this.RealOdd        = realodd;
            this.FreeSpinType   = freespintype;
            this.Ranges         = ranges;
            this.MinRate        = meanrate;
        }
    }
    public class UpdateSpinDataRequest
    {
        public List<UpdateSpinDataItem> Lists { get; set; }
        public UpdateSpinDataRequest(List<UpdateSpinDataItem> lists)
        {
            this.Lists = lists;
        }
    }
    public class UpdateSpinDataItem
    {
        public long     Id              { get; set; }
        public int      FreeSpinType    { get; set; }
        public string   Ranges          { get; set; }
        public double   MeanRate        { get; set; }

        public UpdateSpinDataItem(long id, int freespintype,string ranges,double meanrate)
        {
            this.Id             = id;
            this.FreeSpinType   = freespintype;
            this.Ranges         = ranges;
            this.MeanRate       = meanrate;
        }
    }
}
