using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace RealTimeDBBackUp
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<DBBackService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new DBBackService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("DBBackup for GIT IGaming Solution");
                x.SetServiceName("DBBackup");
                x.SetDisplayName("DBBackup");
                x.StartAutomatically();
            });
        }
    }
}
