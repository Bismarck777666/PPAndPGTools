using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Hocon;
using System.IO;

namespace PreProcessReelData
{
    class Program
    {
        static void Main(string[] args)
        {


            string strConfigText = File.ReadAllText("config.hocon");
            var config = HoconParser.Parse(strConfigText);

            string  strCompnay          = config.GetString("company");
            string  strGameName         = config.GetString("gameName");
            string  strGameSymbol       = config.GetString("gameSymbol");
            int     cols                = config.GetInt("Cols");
            int     freecols            = config.GetInt("FreeCols");

            Task processTask = null;
            SpinDataPreProcess preProcessor = null;
            SqliteDatabaseWork dbWorker = new SqliteDatabaseWork();
            switch (strGameName)
            {
                case "LuckyZodiac":
                    preProcessor = new LuckyZodiacPreProcess();
                    processTask = preProcessor.startPreProcess(dbWorker, strGameName, cols, freecols);
                    break;
                case "BigPanda":
                    preProcessor = new BigPandaPreProcess();
                    processTask = preProcessor.startPreProcess(dbWorker, strGameName, cols, freecols);
                    break;
                case "HotChoice":
                case "LaGranAventura":
                case "HotChoiceDeluxe":
                case "GoldenQuest":
                case "HotChoiceDice":
                case "CasanovasLadies":
                    preProcessor = new HotChoicePreProcess();
                    processTask = preProcessor.startPreProcess(dbWorker, strGameName, cols, freecols);
                    break;
            }
            //preProcessor.startPreProcess(strGameName);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                Console.WriteLine("Stopping.. Please wait..");
                e.Cancel = true;
                preProcessor.doStop();
            };
        }
    }
}
