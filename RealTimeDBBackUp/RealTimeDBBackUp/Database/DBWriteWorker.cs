using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class DBWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private int                         _dbType                 = -1;
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        public DBWriteWorker(string strConnectionString, int dbType)
        {
            _strConnectionString    = strConnectionString;
            _dbType                 = dbType;
            ReceiveAsync<string>(processCommand);
        }
        protected override void PreStart()
        {
            if (_schedulerCancel != null)
                _schedulerCancel.Cancel();

            _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(60000, Self, "write", ActorRefs.NoSender);
        }
        protected override void PostStop()
        {
            base.PostStop();
        }
        public static Props Props(string strConnString, int dbType)
        {
            return Akka.Actor.Props.Create(() => new DBWriteWorker(strConnString, dbType));
        }       
        private async Task processCommand(string strCommand)
        {
            if (strCommand == "write")
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();

                        await updateGameConfigs(connection);
                        await updateAgentGameConfigs(connection);
                        await updateAgents(connection);
                        await updateAgentReports(connection);
                        await updateGameReports(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                }
                _logger.Info("\n {0} --------- Database backuped!!!", DateTime.UtcNow);

                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(600000, Self, "write", ActorRefs.NoSender);
            }
            else if(strCommand == "flush")
            {
                if (_schedulerCancel != null)
                    _schedulerCancel.Cancel();

                _logger.Info("Flush");

                do
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_strConnectionString))
                        {
                            await connection.OpenAsync();
                            int gameConfigCount         = await updateGameConfigs(connection);
                            int agnetGameConfigCount    = await updateAgentGameConfigs(connection);
                            int agentCount              = await updateAgents(connection);
                            int agentReportCount        = await updateAgentReports(connection);
                            int gameReportCount         = await updateGameReports(connection);

                            if (gameConfigCount + agnetGameConfigCount + agentCount + agentReportCount + gameReportCount == 0)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                    }
                } while (true);

                _logger.Info("Flush Ended");
            }
        }
        private async Task<int> updateGameConfigs(SqlConnection connection)
        {
            List<gameconfig> gameConfigUpdates = null;
            try
            {
                gameConfigUpdates = await Context.Parent.Ask<List<gameconfig>>("PopGameConfigUpdates", TimeSpan.FromSeconds(5));
                if (gameConfigUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("gametype",   typeof(int));
                dataTable.Columns.Add("gamename",   typeof(string));
                dataTable.Columns.Add("gamesymbol", typeof(string));
                dataTable.Columns.Add("payoutrate", typeof(decimal));
                dataTable.Columns.Add("openclose",  typeof(bool));
                if(_dbType == 0)
                    dataTable.Columns.Add("gamedata",   typeof(string));
                dataTable.Columns.Add("updatetime", typeof(DateTime));
                dataTable.Columns.Add("releasedate",typeof(DateTime));

                foreach (gameconfig updateItem in gameConfigUpdates)
                {
                    if(_dbType == 0)
                        dataTable.Rows.Add(updateItem.gameid,updateItem.gametype, updateItem.gamename, updateItem.gamesymbol, updateItem.payoutrate, 
                            updateItem.openclose, updateItem.gamedata, updateItem.updatetime, updateItem.releasedate);
                    else
                        dataTable.Rows.Add(updateItem.gameid, updateItem.gametype, updateItem.gamename, updateItem.gamesymbol, updateItem.payoutrate,
                            updateItem.openclose, updateItem.updatetime, updateItem.releasedate);
                }

                SqlCommand command = new SqlCommand("BackupGameConfigs", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameConfigs", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updateGameConfigs : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(gameConfigUpdates);
                return -1;
            }
        }
        private async Task<int> updateAgentGameConfigs(SqlConnection connection)
        {
            List<agentgameconfig> agentGameConfigUpdates = null;
            try
            {
                agentGameConfigUpdates = await Context.Parent.Ask<List<agentgameconfig>>("PopAgentGameConfigUpdates", TimeSpan.FromSeconds(5));
                if (agentGameConfigUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("payoutrate", typeof(decimal));
                dataTable.Columns.Add("updatetime", typeof(DateTime));

                foreach (agentgameconfig updateItem in agentGameConfigUpdates)
                {
                    dataTable.Rows.Add(updateItem.agentid, updateItem.gameid,updateItem.payoutrate, updateItem.updatetime);
                }

                SqlCommand command = new SqlCommand("BackupAgentGameConfigs", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameConfigs", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updateAgentGameConfigs : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(agentGameConfigUpdates);
                return -1;
            }
        }
        private async Task<int> updateAgents(SqlConnection connection)
        {
            List<agent> agentUpdates = null;
            try
            {
                agentUpdates = await Context.Parent.Ask<List<agent>>("PopAgentUpdates", TimeSpan.FromSeconds(5));
                if (agentUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("dbid",       typeof(int));
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("password",   typeof(string));
                dataTable.Columns.Add("secretkey",  typeof(string));
                dataTable.Columns.Add("callbackurl",typeof(string));
                dataTable.Columns.Add("apitoken",   typeof(string));
                dataTable.Columns.Add("nickname",   typeof(string));
                dataTable.Columns.Add("state",      typeof(int));
                dataTable.Columns.Add("currency",   typeof(int));
                dataTable.Columns.Add("language",   typeof(int));
                dataTable.Columns.Add("updatetime", typeof(DateTime));

                foreach (agent updateItem in agentUpdates)
                {
                    dataTable.Rows.Add(updateItem.id, updateItem.username, updateItem.password,updateItem.secretkey, updateItem.callbackurl, updateItem.apitoken,
                        updateItem.nickname, updateItem.state, updateItem.currency, updateItem.language, updateItem.updatetime);
                }

                SqlCommand command = new SqlCommand("BackupAgents", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblAgents", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updateAgents : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(agentUpdates);
                return -1;
            }
        }
        private async Task<int> updateAgentReports(SqlConnection connection)
        {
            List<agentreport> agentReportUpdates = null;
            try
            {
                agentReportUpdates = await Context.Parent.Ask<List<agentreport>>("PopAgentReportUpdates", TimeSpan.FromSeconds(5));
                if (agentReportUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("reporttime", typeof(DateTime));

                foreach (agentreport updateItem in agentReportUpdates)
                {
                    dataTable.Rows.Add(updateItem.agentid, updateItem.bet,updateItem.win, updateItem.reporttime);
                }

                SqlCommand command = new SqlCommand("BackupAgentReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblReports", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updateAgentReports : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(agentReportUpdates);
                return -1;
            }
        }
        private async Task<int> updateGameReports(SqlConnection connection)
        {
            List<gamereport> agentReportUpdates = null;
            try
            {
                agentReportUpdates = await Context.Parent.Ask<List<gamereport>>("PopGameReportUpdates", TimeSpan.FromSeconds(5));
                if (agentReportUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("reportdate", typeof(DateTime));

                foreach (gamereport updateItem in agentReportUpdates)
                {
                    dataTable.Rows.Add(updateItem.gameid, updateItem.agentid, updateItem.bet,updateItem.win, updateItem.reportdate);
                }

                SqlCommand command = new SqlCommand("BackupGameReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblReports", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updateGameReports : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(agentReportUpdates);
                return -1;
            }
        }
    }
}
