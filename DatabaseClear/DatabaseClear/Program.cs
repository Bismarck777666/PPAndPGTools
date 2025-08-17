using System;
using System.Threading.Tasks;

namespace DatabaseClear
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Database dbWork = new Database();
            bool dbinitSuccessed = await dbWork.initConfig();
            if (dbinitSuccessed)
            {
                Console.WriteLine("DB Clear Started");

                //await dbWork.clear();
                await dbWork.openCustomGames();
                //await dbWork.removeAllUserLogs();
                //await dbWork.removeAllAgentLogs();
                //await dbWork.removeAllGameLogTable();
                //await dbWork.removeAllAgents();
                //await dbWork.removeAllUsers();

                Console.WriteLine("DB Clear Finished");
            }
            else
            {
                Console.WriteLine("DB Clear Can't Start");
            }
        }
    }
}
