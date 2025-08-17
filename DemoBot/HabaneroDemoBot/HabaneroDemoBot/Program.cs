using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace HabaneroDemoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<HabaneroBotService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new HabaneroBotService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("HabaneroBotPool for Bot Solution");
                x.SetServiceName("HabaneroBotPool");
                x.SetDisplayName("HabaneroBotPool");
                x.StartAutomatically();
            });
        }
    }
}
