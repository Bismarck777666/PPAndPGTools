using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using Akka.Actor;
using Akka.Event;
using Akka.Configuration;

namespace AmaticDemoBot.Database
{
    public class SqliteWriter : ReceiveActor
    {
        private Config                      _config                 = null;
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);
        private IActorRef                   _workerChild            = null;
        private bool                        _isShuttingDown         = false;


        public SqliteWriter(Config config)
        {
            _config = config;
            Receive<string>                 (processCommand);
            Receive<SpinResponse>           (_ => { SpinDataQueue.Instance.insertSpinDataToQueue(_); });
            Receive<List<SpinResponse>>     (_ => { SpinDataQueue.Instance.PushSpinDataItems(_); });

            Receive<Terminated>(_ =>
            {
                _logger.Info("SqliteWriter::Terminated");
                if (_isShuttingDown)
                {
                    if (_workerChild != null && _.ActorRef.Equals(_workerChild))
                        _workerChild = null;

                    if (_workerChild == null)
                    {
                        _logger.Info("SqliteWriter Terminated : Ended");
                        Self.Tell(PoisonPill.Instance);
                    }
                }
            });

        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new SqliteWriter(config));
        }

        protected override void PreStart()
        {
            base.PreStart();
            _workerChild = Context.ActorOf(SqliteWriteWorker.Props(_config), "SqliteWriteWorker");
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "PopSpinDataItems")
            {
                Sender.Tell(SpinDataQueue.Instance.PopSpinDataItems());
            }
            else if (strCommand == "terminate")
            {
                _isShuttingDown = true;
                _workerChild.Tell("flush");
                _workerChild.Tell(PoisonPill.Instance);
            }
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
            base.PreRestart(reason, message);
        }

        protected override void PostStop()
        {
            if (_schedulerCancel != null)
            {
                _schedulerCancel.Cancel();
                _schedulerCancel = null;
            }
            base.PostStop();
        }
    }
    public class SpinData
    {
        public int      SpinType    { get; set; }        
        public double   SpinOdd     { get; set; }
        public string   Response    { get; set; }
    }
}
