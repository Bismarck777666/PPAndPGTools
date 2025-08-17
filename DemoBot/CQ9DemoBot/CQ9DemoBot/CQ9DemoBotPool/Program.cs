using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Hocon;
using System.IO;
using Topshelf;

namespace CQ9DemoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<CQ9BotService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new CQ9BotService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("CQ9BotPool for Bot Solution");
                x.SetServiceName("CQ9BotPool");
                x.SetDisplayName("CQ9BotPool");
                x.StartAutomatically();
            });
        }
    }
}
