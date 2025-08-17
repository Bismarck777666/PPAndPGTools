using Hocon;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace DBCleanupTool
{
    public class DBCleaner
    {
        private static DBCleaner _sInstance = new DBCleaner();
        
        protected string    _connectionString   = "";
        protected int       _gameLogMaxDays     = 30;
        protected int       _transactionMaxDays = 30;
        protected int       _topMaxDays         = 30;
        protected int       _recentMaxDays      = 20;
        protected int       _pgBetMaxDays       = 90;
        protected int       _topRemainCount     = 0;
        protected int       _recentRemainCount  = 0;
        protected long      _pgBetRemainCount   = 0;

        protected TransactionRemoveTask                 _transactionTask    = null;
        protected Dictionary<int, GameLogRemoveTask>    _gameLogTasks       = null;

        public static DBCleaner Instance => _sInstance;

        public void setDBConfig(HoconRoot config)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource      = config.GetString("ip", "127.0.0.1");
            connectionStringBuilder.InitialCatalog  = config.GetString("dbname", "gitigaming");
            connectionStringBuilder.UserID          = config.GetString("user", "root");
            connectionStringBuilder.Password        = config.GetString("pass", "akduifwkro");

            _gameLogMaxDays         = config.GetInt("gamelogmaxdays", 30);
            _transactionMaxDays     = config.GetInt("transactionmaxdays", 30);
            _topMaxDays             = config.GetInt("topmaxdays", 30);
            _recentMaxDays          = config.GetInt("recentmaxdays", 20);
            _pgBetMaxDays           = config.GetInt("pgbetmaxdays", 90);
            _connectionString       = connectionStringBuilder.ConnectionString;
        }

        public async Task<bool> fetchTasks()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    _transactionTask    = await fetchTransactionTask(connection);
                    Console.WriteLine("Transaction Remain Count {0}", _transactionTask);
                    _gameLogTasks       = await fetchGameLogTasks(connection);
                    Console.WriteLine("GameLog Remain Count {0}", _gameLogTasks);
                    _topRemainCount     = await fetchTopGameLogTask(connection);
                    Console.WriteLine("Top Remain Count {0}", _topRemainCount);

                    _recentRemainCount = await fetchRecentGameLogTask(connection);
                    Console.WriteLine("Recent Remain Count {0}", _recentRemainCount);

                    _pgBetRemainCount = await fetchPGBetHistoryTask(connection);
                    Console.WriteLine("PG BetHistory Remain Count {0}", _pgBetRemainCount);

                    return _transactionTask != null || _gameLogTasks != null && _gameLogTasks.Count > 0 || _topRemainCount > 0 || _recentRemainCount > 0 || _pgBetRemainCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::fetchTasks {0}", ex);
                return false;
            }
        }

        public async Task<int> getOnlineUserCount()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string strQuery     = "SELECT COUNT(*) FROM users WHERE isonline <> 0";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    int onlineUserCount = 0;

                    using (var reader = await command.ExecuteReaderAsync()) 
                    {
                        if (await reader.ReadAsync())
                            onlineUserCount = (int)reader[0];
                    }

                    return onlineUserCount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::getOnlineUserCount {0}", ex);
                return 0;
            }
        }

        public async Task removeTask()
        {
            while (true)
            {
                if (_transactionTask != null)
                {
                    await doRemoveTransaction();
                    await Task.Delay(500);
                }

                List<int> completedAgentIds = new List<int>();
                foreach (KeyValuePair<int, GameLogRemoveTask> pair in _gameLogTasks)
                {
                    await doRemoveGameLog(pair.Value);
                    if (pair.Value.StartId == pair.Value.EndId)
                        completedAgentIds.Add(pair.Value.AgentID);
                    
                    await Task.Delay(500);
                }

                for (int i = 0; i < completedAgentIds.Count; i++)
                    _gameLogTasks.Remove(completedAgentIds[i]);

                if (_topRemainCount > 0)
                {
                    await doRemoveTopGameLog();
                    await Task.Delay(500);
                }
                
                if (_recentRemainCount > 0)
                {
                    await doRemoveRecentGameLog();
                    await Task.Delay(500);
                }

                if (_pgBetRemainCount > 0) 
                {
                    await doRemovePGBetHistory();
                    await Task.Delay(500);
                }

                if (_transactionTask != null || _gameLogTasks.Count != 0 || _topRemainCount != 0 || _recentRemainCount != 0 || _pgBetRemainCount != 0)
                {
                    await Task.Delay(500);
                }
                else
                    break;
            }
        }

        protected async Task doRemoveGameLog(GameLogRemoveTask task)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    long startId    = task.StartId;
                    long endId      = startId + 10000L;
                    if (endId > task.EndId)
                        endId = task.EndId;

                    string strQuery = string.Format("DELETE FROM gamelog_{0} WHERE id <= @id", task.AgentID);
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", endId);

                    int deletedCount = await command.ExecuteNonQueryAsync();
                    Console.WriteLine("{0}: Deleted {1} rows from gamelog_{2}", DateTime.Now, deletedCount, task.AgentID);
                    task.StartId = endId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::doRemoveGameLog {0}", ex);
            }
        }

        protected async Task doRemoveTransaction()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    long startId    = _transactionTask.StartId;
                    long endId      = startId + 10000L;

                    if (endId > _transactionTask.EndId)
                        endId = _transactionTask.EndId;
                    
                    string strQuery = "DELETE FROM transactions WHERE id < @id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", endId);
                    
                    int removeCount = await command.ExecuteNonQueryAsync();
                    Console.WriteLine("{0}: Deleted {1} rows from transactions", DateTime.Now, removeCount);
                    if (endId == _transactionTask.EndId)
                        _transactionTask = null;
                    else
                        _transactionTask.StartId = endId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::doRemoveTransaction {0}", ex);
            }
        }

        protected async Task<Dictionary<int, GameLogRemoveTask>> fetchGameLogTasks(SqlConnection connection)
        {
            string strErrTable = string.Empty;
            try
            {
                string strQuery = "SELECT id FROM agents";
                SqlCommand command = new SqlCommand(strQuery, connection);
                List<int> agentIds = new List<int>();

                using(var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int agentId = (int)reader[0];
                        agentIds.Add(agentId);
                    }
                }

                agentIds.Sort();
                //foreach(int agentId in agentIds)
                //{
                //    Console.WriteLine(agentId);
                //}


                Dictionary<int, GameLogRemoveTask> gameLogTasks = new Dictionary<int, GameLogRemoveTask>();
                for (int i = 0; i < agentIds.Count; i++)
                {
                    string strGameLogTableName = string.Format("gamelog_{0}", agentIds[i]);
                    strErrTable = strGameLogTableName;
                    bool tableExists = false;

                    strQuery    = string.Format("SELECT case WHEN exists((select * from information_schema.tables where table_name = '{0}')) then 1 else 0 end", strGameLogTableName);
                    command     = new SqlCommand(strQuery, connection);
                    
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if ((int)reader[0] == 1)
                                tableExists = true;
                        }
                    }

                    if (tableExists)
                    {
                        Console.WriteLine(string.Format("Getting start id from table {0}", strGameLogTableName));
                        
                        strQuery    = string.Format("SELECT TOP (1) logtime, id FROM {0} order by logtime", strGameLogTableName);
                        command     = new SqlCommand(strQuery, connection);

                        long gameLogStartId = 0;
                        
                        using(var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                gameLogStartId      = (long)reader[1];
                                DateTime startTime  = (DateTime)reader[0];
                                if (startTime >= DateTime.UtcNow.Subtract(TimeSpan.FromDays(_gameLogMaxDays)))
                                    gameLogStartId = 0L;
                            }
                        }

                        Console.WriteLine(string.Format("gamelog start id {1} from table {0}", strGameLogTableName, gameLogStartId));

                        if (gameLogStartId != 0L)
                        {
                            Console.WriteLine(string.Format("Getting end id from table {0}", strGameLogTableName));
                            
                            strQuery    = string.Format("SELECT TOP 1 id FROM {0} WHERE logtime < @logtime ORDER BY logtime DESC", strGameLogTableName);
                            command     = new SqlCommand(strQuery, connection);
                            command.Parameters.AddWithValue("@logtime", DateTime.UtcNow.Subtract(TimeSpan.FromDays(_gameLogMaxDays)));
                            long gameLogEndId = 0;

                            using(var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                    gameLogEndId = (long)reader[0];

                                if (gameLogEndId != 0)
                                {
                                    //Console.WriteLine(string.Format("GameLog Remove {0}-{1}from table {2}", gameLogStartId, gameLogEndId, strGameLogTableName));
                                    gameLogTasks.Add(agentIds[i], new GameLogRemoveTask(agentIds[i], gameLogStartId, gameLogEndId));
                                }
                            }
                            
                        }
                    }
                }
                return gameLogTasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}-{1}", strErrTable, ex.ToString());
                return new Dictionary<int, GameLogRemoveTask>();
            }
        }

        protected async Task<TransactionRemoveTask> fetchTransactionTask(SqlConnection connection)
        {
            try
            {
                string strQuery     = "SELECT TOP (1) id FROM transactions WHERE timestamp < @timestamp ORDER BY id";
                SqlCommand command  = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(_transactionMaxDays)));
                long transactionStartId = 0;

                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        transactionStartId = (long)reader[0];
                }
                
                if (transactionStartId == 0)
                    return null;

                strQuery    = "SELECT TOP (1) id FROM transactions WHERE timestamp >= @timestamp ORDER BY id";
                command     = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@timestamp", DateTime.UtcNow.Subtract(TimeSpan.FromDays(_transactionMaxDays)));
                long transactionEndId = 0;
                
                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        transactionEndId = (long)reader[0];
                }

                if (transactionEndId == 0)
                    return null;

                return new TransactionRemoveTask(transactionStartId, transactionEndId);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::fetchTransactionTask {0}", ex);
                return null;
            }
        }

        protected async Task<int> fetchTopGameLogTask(SqlConnection connection)
        {
            try
            {
                string strQuery     = "SELECT COUNT(*) FROM ppusertopgamelog WHERE playedDate < @playedDate";
                SqlCommand command  = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@playedDate", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays((double)_topMaxDays))));

                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return (int)reader[0];
                }
            }
            catch
            {
                Console.WriteLine("aaa");
            }
            return 0;
        }

        protected async Task<int> fetchRecentGameLogTask(SqlConnection connection)
        {
            try
            {
                string strQuery     = "SELECT COUNT(*) FROM ppuserrecentgamelog WHERE timestamp < @timestamp";
                SqlCommand command  = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@timestamp", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays(_recentMaxDays))));

                using(var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return (int)reader[0];
                }
            }
            catch
            {
            }
            return 0;
        }

        protected async Task<long> fetchPGBetHistoryTask(SqlConnection connection)
        {
            try
            {
                string strQuery = "SELECT COUNT(*) FROM pgbethistory WHERE timestamp < @timestamp";
                SqlCommand command = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@timestamp", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays(_pgBetMaxDays))));

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader[0]);
                        return (int)reader[0];
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }

        public async Task doRemoveTopGameLog()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string strQuery     = string.Format("DELETE TOP (10000) FROM ppusertopgamelog WHERE playedDate < {0}", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays(_topMaxDays))));
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    
                    int deletedCount = await command.ExecuteNonQueryAsync();
                    _topRemainCount -= deletedCount;
                    if (_topRemainCount < 0 || deletedCount == 0)
                        _topRemainCount = 0;

                    Console.WriteLine("{0}: Deleted {1} rows from ppusertopgamelog", DateTime.Now, deletedCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::doRemoveTopGameLog {0}", ex);
            }
        }

        public async Task doRemoveRecentGameLog()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string strQuery     = string.Format("DELETE TOP (10000) FROM ppuserrecentgamelog WHERE timestamp < {0}", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays(_recentMaxDays))));
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    
                    int deletedCount = await command.ExecuteNonQueryAsync();
                    _recentRemainCount -= deletedCount;
                    if (_recentRemainCount < 0 || deletedCount == 0)
                        _recentRemainCount = 0;
                    
                    Console.WriteLine("{0}: Deleted {1} rows from ppuserrecentgamelog", DateTime.Now, deletedCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::doRemoveRecentGameLog {0}", ex);
            }
        }

        public async Task doRemovePGBetHistory()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string strQuery     = string.Format("DELETE TOP (10000) FROM pgbethistory WHERE timestamp < {0}", unixTimestampMilli(DateTime.UtcNow.Subtract(TimeSpan.FromDays(_pgBetMaxDays))));
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    
                    int deletedCount = await command.ExecuteNonQueryAsync();
                    _pgBetRemainCount -= deletedCount;
                    if (_pgBetRemainCount < 0 || deletedCount == 0)
                        _pgBetRemainCount = 0;
                    
                    Console.WriteLine("{0}: Deleted {1} rows from pgbethistory", DateTime.Now, deletedCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured in DBCleaner::doRemovePGBetHistory {0}", ex);
            }
        }

        protected long unixTimestampMilli(DateTime time)
        {
            return (long)time.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        //protected long unixTimestampSec(DateTime time)
        //{
        //    return (long)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        //}
    }

    public class GameLogRemoveTask
    {
        public int  AgentID { get; set; }
        public long StartId { get; set; }
        public long EndId   { get; set; }

        public GameLogRemoveTask(int agentID, long startId, long endId)
        {
            AgentID     = agentID;
            StartId     = startId;
            EndId       = endId;
        }
    }

    public class TransactionRemoveTask
    {
        public long StartId { get; set; }
        public long EndId   { get; set; }

        public TransactionRemoveTask(long startId, long endId)
        {
            StartId = startId;
            EndId   = endId;
        }
    }
}
