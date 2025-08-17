using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class DBProxyWriter : ReceiveActor
    {

        private string                      _strConnString      = "";
        private int                         _dbType             = -1;
        private readonly  ILoggingAdapter   _logger             = Logging.GetLogger(Context);
        private IActorRef                   _workerChild        = null;
        private bool                        _isShuttingDown     = false;

        public DBProxyWriter(string strConnStriing, int dbType)
        {
            _strConnString  = strConnStriing;
            _dbType         = dbType;

            Receive<string>(mesasge => processCommand(mesasge));            
            Receive<List<gameconfig>>(updateItem =>
            {
                WriterSnapshot.Instance.PushGameConfigUpdateItems(updateItem);
            });
            Receive<List<agentgameconfig>>(updateItem =>
            {
                WriterSnapshot.Instance.PushAgentGameConfigUpdateItems(updateItem);
            });
            Receive<List<agent>>(updateItem =>
            {
                WriterSnapshot.Instance.PushAgentUpdateItems(updateItem);
            });
            Receive<List<agentreport>>(updateItem =>
            {
                WriterSnapshot.Instance.PushAgentReportUpdateItems(updateItem);
            });
            Receive<List<gamereport>>(updateItem =>
            {
                WriterSnapshot.Instance.PushGameReportUpdateItems(updateItem);
            });
            Receive<Terminated>(_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if(_isShuttingDown)
                {
                    if (_workerChild != null &&  _.ActorRef.Equals(_workerChild))
                        _workerChild = null;

                    if (_workerChild == null)
                    {
                        _logger.Info("DB Proxy Terminated : Ended");
                        Self.Tell(PoisonPill.Instance);
                    }
                }
            });
        }

        public static Props Props(string strConnString, int dbType)
        {
            return Akka.Actor.Props.Create(() => new DBProxyWriter(strConnString, dbType));
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "PopGameConfigUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopGameConfigUpdates());
            }
            else if (strCommand == "PopAgentGameConfigUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopAgentGameConfigUpdates());
            }
            if (strCommand == "PopAgentUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopAgentUpdates());
            }
            if (strCommand == "PopAgentReportUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopAgentReportUpdates());
            }
            if (strCommand == "PopGameReportUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopGameReportUpdates());
            }
            else if (strCommand == "terminate")
            {
                _isShuttingDown = true;
                _workerChild.Tell("flush");
                _workerChild.Tell(PoisonPill.Instance);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            _workerChild        = Context.ActorOf(DBWriteWorker.Props(_strConnString, _dbType), "writeWorker");
            Context.Watch(_workerChild);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
            base.PreRestart(reason, message);
        }
    }
}
