using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class DBProxy : ReceiveActor
    {
        private string          _strBackConnectionString    = "";
        private string          _strSourceConnectionString  = "";
        private int             _dbType                     = -1;
        private IActorRef       _monitorActor;
        private IActorRef       _writeActor;

        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        public DBProxy(Config backConfig, Config sourceConfig)
        {
            //ConnectionString 을 빌드한다.
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource        = backConfig.GetString("ip",        "127.0.0.1");
            connStringBuilder.InitialCatalog    = backConfig.GetString("dbname",    "amatic");
            connStringBuilder.UserID            = backConfig.GetString("user",      "sa");
            connStringBuilder.Password          = backConfig.GetString("pass",      "bismarck");
            _strBackConnectionString            = connStringBuilder.ConnectionString;

            connStringBuilder.DataSource        = sourceConfig.GetString("ip",      "127.0.0.1");
            connStringBuilder.InitialCatalog    = sourceConfig.GetString("dbname",  "amatic");
            connStringBuilder.UserID            = sourceConfig.GetString("user",    "sa");
            connStringBuilder.Password          = sourceConfig.GetString("pass",    "bismarck");
            _strSourceConnectionString          = connStringBuilder.ConnectionString;

            _dbType = sourceConfig.GetInt("dbtype", 0);

            ReceiveAsync<string>(processCommand);
        }

        public static Props Props(Config backConfig, Config sourceConfig)
        {
            return Akka.Actor.Props.Create(() => new DBProxy(backConfig, sourceConfig));
        }
        private async Task processCommand(string command)
        {
            if (command == "initialize")
            {
                //자료기지서버의 련결가능성을 검사한다.
                if (!await checkDBConnection())
                {
                    _logger.Error("Can not connect to database. Please check if database is correctly configured.");
                    return;
                }

                //모니터액터를 초기화한다.
                await _monitorActor.Ask("initialize");
                Sender.Tell(new ReadyDBProxy(_writeActor));
            }
            else if (command == "terminate")
            {
                _monitorActor.Tell(PoisonPill.Instance);
                _writeActor.Tell("terminate");
                Become(ShuttingDown);
            }
        }
        private void ShuttingDown()
        {
            Receive<Terminated>(terminated =>
            {
                if (_monitorActor != null && terminated.ActorRef.Equals(_monitorActor))
                    _monitorActor = null;
                else if (_writeActor != null && terminated.ActorRef.Equals(_writeActor))
                    _writeActor = null;

                if (_monitorActor == null && _writeActor == null)
                    Self.Tell(PoisonPill.Instance);
            });
        }
        private async Task<bool> checkDBConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strBackConnectionString))
                {
                    await connection.OpenAsync();
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        protected override void PreStart()
        {
            _monitorActor = Context.ActorOf(DBProxyMonitor.Props(_strBackConnectionString, _strSourceConnectionString),"monitor");
            _writeActor   = Context.ActorOf(DBProxyWriter.Props(_strBackConnectionString, _dbType),"writer");

            Context.Watch(_monitorActor);
            Context.Watch(_writeActor);
            base.PreStart();
        }
        
        #region Messages
        public class ReadyDBProxy
        {
            public IActorRef Writer { get; private set; }

            public ReadyDBProxy(IActorRef writer)
            {
                this.Writer         = writer;
            }
        }
        #endregion
    }
}
