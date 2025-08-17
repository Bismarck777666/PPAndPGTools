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

namespace AmaticDemoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<AmaticBotService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new AmaticBotService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("AmaticDemoBotPool for Bot Solution");
                x.SetServiceName("AmaticDemoBotPool");
                x.SetDisplayName("AmaticDemoBotPool");
                x.StartAutomatically();
            });
        }
    }
}
