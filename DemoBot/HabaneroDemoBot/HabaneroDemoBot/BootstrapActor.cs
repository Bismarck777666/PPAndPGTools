using Akka.Actor;
using Akka.Configuration;
using HabaneroDemoBot.Database;
using HabaneroDemoBot.HabaneroFetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HabaneroDemoBot
{
    public class BootstrapActor: ReceiveActor
    {
        private Config      _configuration  = null;
        private IActorRef   _fetchManager   = null;
        private IActorRef   _sqliteWriter   = null;

        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;
            Receive<string>(command =>
            {
                processCommand(command);
            });

            ReceiveAsync<ShutdownSystemMessage>(onShutdownSystem);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "startService")
            {
                //sqlWirter 액터를 창조한다
                _sqliteWriter = Context.System.ActorOf(SqliteWriter.Props(_configuration), "SqliteWriter");
                //FetchManager 액터를 창조한다
                _fetchManager = Context.System.ActorOf(FetchManager.Props(_configuration), "FetchManager");
            }
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                //스핀데이터 훼쳐들을 중지한다
                await _fetchManager.GracefulStop(TimeSpan.FromSeconds(60), "terminate");
            }
            catch (Exception)
            {
            }
            Sender.Tell(true);
        }
    }
    public class ShutdownSystemMessage
    {

    }
}
