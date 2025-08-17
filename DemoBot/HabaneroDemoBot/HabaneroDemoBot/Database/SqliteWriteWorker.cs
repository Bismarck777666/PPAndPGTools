using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace HabaneroDemoBot.Database
{
    public class SqliteWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        private Config                      _config                 = null;
        private string                      _gameName               = "";
        private int                         _gameCategory           = -1;
        private int                         _playbet                = 0;
        private int                         _playmini               = 0;
        private double                      _minOdd                 = 0;

        private int                         _totalCount             = 0;
        private double                      _totalBet               = 0.0;
        private double                      _totalWin               = 0.0;
        private int                         _freeSpinCount          = 0;
        private double                      _maxOdd                 = 0;

        private Dictionary<double, bool>    _spinOdds               = new Dictionary<double, bool>();

        public SqliteWriteWorker(Config config)
        {
            _config         = config;
            _gameName       = _config.GetString("gameName");
            _playbet        = _config.GetInt("playbet");
            _playmini       = _config.GetInt("playmini");
            _minOdd         = _config.GetDouble("minOdd");
            _gameCategory   = _config.GetInt("gameCategory");

            ReceiveAsync<string>(processCommand);
        }
        protected override void PreStart()
        {
            if (_schedulerCancel != null)
                _schedulerCancel.Cancel();

            _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "initialize", ActorRefs.NoSender);
        }
        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new SqliteWriteWorker(config));
        }
        private async Task processCommand(string strCommand)
        {
            if(strCommand == "initialize")
            {
                await initialize();
                await createGameTable();
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(10000, Self, "write", ActorRefs.NoSender);
            }
            else if (strCommand == "write")
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await writeSpinData();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in Sqlite WriteWorker::processCommand {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
            }
            else if (strCommand == "flush")
            {
                if (_schedulerCancel != null)
                    _schedulerCancel.Cancel();

                _logger.Info("Flush Started");

                do
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();
                        int insertCount = await writeSpinData();

                        _logger.Info("report updated at {0}, count: {1}", DateTime.Now, insertCount);
                        if (insertCount == 0)
                            break;
                    }
                } while (true);
                _logger.Info("Flush Ended");
            }
        }
        public async Task initialize()
        {
            try
            {
                string appPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string strFilePath = Path.Combine(appPath, string.Format("{0}.db", _gameName));

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
        public async Task<int> writeSpinData()
        {
            List<SpinResponse> spinResponseItems = null;
            try
            {
                spinResponseItems = await Context.Parent.Ask<List<SpinResponse>>("PopSpinDataItems", TimeSpan.FromSeconds(5));
                if (spinResponseItems == null)
                    return 0;
                using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                    int count = 0;
                    SQLiteTransaction transaction = null;
                    for (int i = 0; i < spinResponseItems.Count; i++)
                    {
                        if (spinResponseItems[i].SpinType == 1 || spinResponseItems[i].SpinType == 100)
                            _freeSpinCount++;
                        double odd = spinResponseItems[i].TotalWin / (_playbet * _playmini);
                        if (_gameCategory == (int)GameCategory.OptionGame)     //프리스핀옵션인경우
                        {
                            double realodd = 0;
                            if (spinResponseItems[i] is OptionGameResponse)
                            {
                                OptionGameResponse optionGameResponse = spinResponseItems[i] as OptionGameResponse;
                                realodd = optionGameResponse.RealWin / (_playbet * _playmini);
                            }
                            if (odd > _maxOdd)
                                _maxOdd = odd;

                            if (odd < _minOdd)
                                continue;
                            _totalCount++;
                            _totalBet += (_playbet * _playmini);
                            _totalWin += spinResponseItems[i].TotalWin;

                            if (!_spinOdds.ContainsKey(odd))
                                _spinOdds.Add(odd, true);

                            if (transaction == null)
                                transaction = connection.BeginTransaction();

                            string strCommand = "INSERT INTO spins (spintype, odd, realodd ,data) VALUES (@spintype, @odd,@realodd, @data)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype",    spinResponseItems[i].SpinType);
                            command.Parameters.AddWithValue("@odd",         Math.Round(odd, 2));
                            command.Parameters.AddWithValue("@realodd",     Math.Round(realodd, 2));
                            command.Parameters.AddWithValue("@data",        spinResponseItems[i].Response);
                            await command.ExecuteNonQueryAsync();
                        }
                        else//일반 경우
                        {
                            if (odd > _maxOdd)
                                _maxOdd = odd;

                            if (odd < _minOdd)
                                continue;
                            _totalCount++;
                            _totalBet += (_playbet * _playmini);
                            _totalWin += spinResponseItems[i].TotalWin;

                            if (!_spinOdds.ContainsKey(odd))
                                _spinOdds.Add(odd, true);

                            if (transaction == null)
                                transaction = connection.BeginTransaction();

                            string strCommand = "INSERT INTO spins (spintype, odd,  data) VALUES (@spintype, @odd, @data)";
                            SQLiteCommand command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@spintype",    spinResponseItems[i].SpinType);
                            command.Parameters.AddWithValue("@odd",         Math.Round(odd, 2));
                            command.Parameters.AddWithValue("@data",        spinResponseItems[i].Response);
                            await command.ExecuteNonQueryAsync();
                        }
                        count++;
                    }
                    if (transaction != null)
                    {
                        transaction.Commit();
                        Console.WriteLine(string.Format("{0}: totalCount: {1} totalBet:{2}, totalWin: {3}, maxOdd: {4}, freeCount: {5}, total Odds Count: {6}", 
                            DateTime.Now, _totalCount, Math.Round(_totalBet, 2),Math.Round(_totalWin, 2), Math.Round(_maxOdd, 2), _freeSpinCount, _spinOdds.Count));
                    }
                    return count;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning("exception has occured in write to database {0} :: spinItemCnt = {1}", ex.ToString(),spinResponseItems.Count);
                if (spinResponseItems != null && spinResponseItems.Count > 0)
                    Context.Parent.Tell(spinResponseItems);

                return -1;
            }
        }
        public async Task createGameTable()
        {
            if (_gameCategory == (int)GameCategory.OptionGame)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();

                        string strCommand = string.Format("CREATE TABLE IF NOT EXISTS 'spins' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,'spintype' INTEGER NOT NULL, 'odd' real NOT NULL, 'realodd' real NOT NULL, 'data' text NOT NULL)");

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
            else
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
    }
}
