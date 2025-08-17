using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class DBBackService
    {
        private ActorSystem _actorSystem;
        private IActorRef   _bootstrapActor;

        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting DBBackup Service...");

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

            var backupConfig = clusterConfig.GetConfig("dbbackup");
            if (backupConfig == null)
            {
                logger.Error("config.hocon doesn't contain connector server configuration");
                return false;
            }

            string systemName = backupConfig.GetString("actorsystem", "gitigaming");
            _actorSystem    = ActorSystem.Create(systemName, clusterConfig);
            _bootstrapActor = _actorSystem.ActorOf(BootstrapActor.Props(backupConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        public Task TerminationHandle => _actorSystem.WhenTerminated;

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
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
