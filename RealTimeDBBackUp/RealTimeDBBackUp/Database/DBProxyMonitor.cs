using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class DBProxyMonitor : ReceiveActor
    {
        private string                      _backConnString         = null;
        private string                      _sourceConnString       = null;
        private ICancelable                 _monitorCancelable      = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        public DBProxyMonitor(string backConnString, string sourceConnString)
        {
            _backConnString     = backConnString;
            _sourceConnString   = sourceConnString;

            ReceiveAsync<string>(processCommand);
        }
        public static Props Props(string backConnString, string sourceConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyMonitor(backConnString, sourceConnString));
        }
        protected override void PreStart()
        {
            base.PreStart();
        }
        protected override void PostStop()
        {
            if (_monitorCancelable != null)
                _monitorCancelable.Cancel();

            base.PostStop();
        }
        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            Self.Tell("initialize");
        }

        private async Task processCommand(string strCommand)
        {
            if(strCommand == "initialize")
            {
                await initializeMonitor();
                _monitorCancelable      = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "monitor", ActorRefs.NoSender);

                Sender.Tell("dbInitialized");
            }
            else if(strCommand == "monitor")
            {
                _logger.Info("\n {0} DB Monitoring Started-------!!!", DateTime.UtcNow);
                await monitorTables();
                _logger.Info("\n {0} DB Monitoring Finished------!!!", DateTime.UtcNow);
            }
        }
        private async Task initializeMonitor()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_backConnString))
                {
                    await connection.OpenAsync();
                    
                    //게임설정을 감시한다.
                    string strQuery = "SELECT updatetime  FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTimes);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime    updateTime  = (DateTime)reader["updatetime"];

                            if(DBMonitorSnapshot.Instance.GameConfigUpdateTimes < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTimes = updateTime;
                        }
                    }

                    //에이전트표를 감시한다
                    strQuery = "SELECT updatetime FROM agents WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime updatetime = (DateTime)reader["updatetime"];

                            if(DBMonitorSnapshot.Instance.AgentUpdateTime < updatetime)
                                DBMonitorSnapshot.Instance.AgentUpdateTime = updatetime;

                        }
                    }

                    //에이전트별게임설정을 감시한다.
                    strQuery = "SELECT updatetime FROM agentgameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime        updateTime      = (DateTime)reader["updatetime"];

                            if(DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes < updateTime)
                                DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes = updateTime;
                        }
                    }

                    //에이전리포트표를 감시한다
                    strQuery = "SELECT reporttime FROM agentreports WHERE reporttime >= @reporttime ORDER BY reporttime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@reporttime", DBMonitorSnapshot.Instance.AgentReportUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime reporttime = (DateTime)reader["reporttime"];

                            if(DBMonitorSnapshot.Instance.AgentReportUpdateTime < reporttime)
                                DBMonitorSnapshot.Instance.AgentReportUpdateTime = reporttime;
                        }
                    }

                    //게임리포트표를 감시한다
                    strQuery = "SELECT reportdate FROM gamereports WHERE reportdate >= @reportdate ORDER BY reportdate";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@reportdate", DBMonitorSnapshot.Instance.GameReprotUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime reportdate = (DateTime)reader["reportdate"];

                            if (DBMonitorSnapshot.Instance.GameReprotUpdateTime < reportdate)
                                DBMonitorSnapshot.Instance.GameReprotUpdateTime = reportdate;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor :: initializeMonitor {1}", ex);
            }
        }
        private async Task monitorTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_sourceConnString))
                {
                    List<gameconfig>        gameConfigList  = new List<gameconfig>();
                    List<agent>             agentList       = new List<agent>();
                    List<agentgameconfig>   agentConfigList = new List<agentgameconfig>();
                    List<agentreport>       agentReportList = new List<agentreport>();
                    List<gamereport>        gameReportList  = new List<gamereport>();

                    await connection.OpenAsync();

                    //게임설정을 감시한다.
                    string strQuery = "SELECT * FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTimes);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int             gameID          = (int)reader["gameid"];
                            int             gameType        = (int)reader["gametype"];
                            string          gameName        = (string)reader["gamename"];
                            string          gameSymbol      = (string)reader["gamesymbol"];
                            decimal         payoutrate      = (decimal)reader["payoutrate"];
                            bool            openClose       = (bool)reader["openclose"];
                            DateTime        updateTime      = (DateTime)reader["updatetime"];
                            DateTime        releaseDate     = (DateTime)reader["releasedate"];
                            string          gamedata        = "";

                            for(int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.GetName(i).Equals("gamedata", StringComparison.InvariantCultureIgnoreCase))
                                    gamedata = (string)reader["gamedata"];
                            }
                            
                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTimes < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTimes = updateTime;

                            gameconfig config = new gameconfig();
                            config.gameid       = gameID;
                            config.gametype     = gameType;
                            config.gamename     = gameName;
                            config.gamesymbol   = gameSymbol;
                            config.payoutrate   = payoutrate;
                            config.openclose    = openClose;
                            config.gamedata     = gamedata;
                            config.updatetime   = updateTime;
                            config.releasedate  = releaseDate;
                            gameConfigList.Add(config);
                        }
                    }

                    //에이전트표를 감시한다
                    strQuery = "SELECT * FROM agents WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id              = (int)reader["id"];
                            string username     = (string)reader["username"];
                            string password     = (string)reader["password"];
                            string secretkey    = (string)reader["secretkey"];
                            string callbackurl  = (string)reader["callbackurl"];
                            string apitoken     = (string)reader["apitoken"];
                            string nickname     = (string)reader["nickname"];
                            int state           = (int)reader["state"];
                            int currency        = (int)reader["currency"];
                            int language        = (int)reader["language"];
                            DateTime updatetime = (DateTime)reader["updatetime"];

                            if(DBMonitorSnapshot.Instance.AgentUpdateTime < updatetime)
                                DBMonitorSnapshot.Instance.AgentUpdateTime = updatetime;

                            agent agentInfo = new agent();
                            agentInfo.id            = id;
                            agentInfo.username      = username;
                            agentInfo.password      = password;
                            agentInfo.secretkey     = secretkey;
                            agentInfo.callbackurl   = callbackurl;
                            agentInfo.apitoken      = apitoken;
                            agentInfo.nickname      = nickname;
                            agentInfo.state         = state;
                            agentInfo.currency      = currency;
                            agentInfo.language      = language;
                            agentInfo.updatetime    = updatetime;

                            agentList.Add(agentInfo);
                        }
                    }

                    //에이전트별게임설정을 감시한다.
                    strQuery = "SELECT * FROM agentgameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int             gameID          = (int)reader["gameid"];
                            int             agentID         = (int)reader["agentid"];
                            decimal         payoutrate      = (decimal)reader["payoutrate"];
                            DateTime        updateTime      = (DateTime)reader["updatetime"];

                            if(DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes < updateTime)
                                DBMonitorSnapshot.Instance.AgentGameConfigUpdateTimes = updateTime;

                            agentgameconfig agentGameConfig = new agentgameconfig();
                            agentGameConfig.agentid     = agentID;
                            agentGameConfig.gameid      = gameID;
                            agentGameConfig.payoutrate  = payoutrate;
                            agentGameConfig.updatetime  = updateTime;
                            agentConfigList.Add(agentGameConfig);
                        }
                    }

                    //에이전리포트표를 감시한다
                    strQuery = "SELECT * FROM agentreports WHERE reporttime >= @reporttime ORDER BY reporttime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@reporttime", DBMonitorSnapshot.Instance.AgentReportUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int agentid         = (int)reader["agentid"];
                            decimal bet         = (decimal)reader["bet"];
                            decimal win         = (decimal)reader["win"];
                            DateTime reporttime = (DateTime)reader["reporttime"];

                            if(DBMonitorSnapshot.Instance.AgentReportUpdateTime < reporttime)
                                DBMonitorSnapshot.Instance.AgentReportUpdateTime = reporttime;

                            agentreport agentReport = new agentreport();
                            agentReport.agentid     = agentid;
                            agentReport.bet         = bet;
                            agentReport.win         = win;
                            agentReport.reporttime  = reporttime;

                            agentReportList.Add(agentReport);
                        }
                    }

                    //게임리포트표를 감시한다
                    strQuery = "SELECT * FROM gamereports WHERE reportdate >= @reportdate ORDER BY reportdate";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@reportdate", DBMonitorSnapshot.Instance.GameReprotUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int gameid  = (int)reader["gameid"];
                            int agentid = (int)reader["agentid"];
                            decimal bet = (decimal)reader["bet"];
                            decimal win = (decimal)reader["win"];
                            DateTime reportdate = (DateTime)reader["reportdate"];

                            if (DBMonitorSnapshot.Instance.GameReprotUpdateTime < reportdate)
                                DBMonitorSnapshot.Instance.GameReprotUpdateTime = reportdate;

                            gamereport gameReport = new gamereport();
                            gameReport.gameid   = gameid;
                            gameReport.agentid  = agentid;
                            gameReport.bet      = bet;
                            gameReport.win      = win;
                            gameReport.reportdate = reportdate;

                            gameReportList.Add(gameReport);
                        }
                    }

                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(gameConfigList);
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(agentList);
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(agentConfigList);
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(agentReportList);
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(gameReportList);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor:: monitorOneServerTables {1}", ex);
            }

            _monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(600000, Self, "monitor", ActorRefs.NoSender);
        }
    }
}
