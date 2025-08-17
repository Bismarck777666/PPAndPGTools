using Hocon;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace DBCleanupTool
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string strConfigText = File.ReadAllText("config.hocon");
            
            var         config          = HoconParser.Parse(strConfigText);
            int         maxUserCount    = config.GetInt("maxusercount", 50);
            string      strStartTime    = config.GetString("starttime");
            string      strEndTime      = config.GetString("endtime");
            DateTime    startTime       = DateTime.ParseExact(strStartTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime    endTime         = DateTime.ParseExact(strEndTime, "HH:mm:ss", CultureInfo.InvariantCulture);
            
            TimeSpan timeOfDay  = startTime.TimeOfDay;
            double startTotalSeconds = timeOfDay.TotalSeconds;

            timeOfDay = endTime.TimeOfDay;
            double endTotalSeconds = timeOfDay.TotalSeconds;
            
            DBCleaner.Instance.setDBConfig(config);
            while (true)
            {
                timeOfDay = DateTime.UtcNow.TimeOfDay;
                double nowTotalSeconds = timeOfDay.TotalSeconds;
                if (nowTotalSeconds < startTotalSeconds || nowTotalSeconds > endTotalSeconds)
                {
                    Console.WriteLine("{0}: Fetch performed but time is not in date rage", DateTime.Now);
                    await Task.Delay(60000);
                    continue;
                }

                bool hasTask = await DBCleaner.Instance.fetchTasks();
                if (!hasTask)
                {
                    Console.WriteLine("{0}: Fetch performed but didn't find anything to delete", DateTime.Now);
                    await Task.Delay(300000);
                    continue;
                }

                Console.WriteLine("{0}: Fetch performed, found something to delete", DateTime.Now);
                int onlineUserCount;
                while (true)
                {
                    onlineUserCount = await DBCleaner.Instance.getOnlineUserCount();
                    if (onlineUserCount > maxUserCount)
                    {
                        Console.WriteLine("{0}: Online User Count: {1} wait for next time", DateTime.Now, onlineUserCount);
                        await Task.Delay(60000);
                    }
                    else
                        break;
                }
                
                Console.WriteLine("{0}: Online User Count: {1}", DateTime.Now, onlineUserCount);
                await DBCleaner.Instance.removeTask();
                
                Console.WriteLine("{0}: Remove Task Completed", DateTime.Now);
                await Task.Delay(60000);
            }
        }
    }
}
