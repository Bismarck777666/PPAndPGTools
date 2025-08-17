using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace HabaneroDemoBot
{
    public class HabaneroBotService
    {
        private ActorSystem     _connectSystem;
        private IActorRef       _bootstrapActor;

        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting Connect Service...");

            //먼저 설정정보를 검사한다.
            Config clusterConfig = null;
            try
            {
                clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            }
            catch
            {
                logger.Error("Please make sure if config.hocon file is correctly formatted");
                return false;
            }

            var connectorConfig = clusterConfig.GetConfig("habanerobotpool");
            if (connectorConfig == null)
            {
                logger.Error("config.hocon doesn't contain configuration");
                return false;
            }

            //액터시스템을 창조한다.
            _connectSystem = HabaneroBotHostFactory.LauchCommNode(clusterConfig);

            //부트스트랩액터를 창조한다.
            _bootstrapActor = _connectSystem.ActorOf(BootstrapActor.Props(connectorConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        public Task TerminationHandle => _connectSystem.WhenTerminated;

        public async Task StopAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Stop Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new ShutdownSystemMessage(), TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }
            await CoordinatedShutdown.Get(_connectSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
