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

            Task processTask = null;
            SpinDataPreProcess preProcessor = null;
            SqliteDatabaseWork dbWorker = new SqliteDatabaseWork();
            if(strCompnay == "PP")
            {
                switch (strGameName)
                {
                    case "GreatRhinoMega":
                        preProcessor = new GreatRhinoMegaPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "FiveLionsMega":
                        preProcessor = new FiveLionsGoldMegaPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "TheDogHouseMega":
                        preProcessor = new TheDogHouseMegaPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "WildBooster":
                        preProcessor = new WildBoosterPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "FiveLionsGold":
                        preProcessor = new FiveLionsGoldPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "FiveLions":
                        preProcessor = new FiveLionsPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    default:
                        preProcessor = new SpinDataPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                }
            }
            else
            {
                switch (strGameName)
                {
                    case "GodOfWar":
                    case "TheBeastWar":
                    case "Wonderland":
                        preProcessor = new GodOfWarPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "GreekGods":
                    case "GophersWar":
                        preProcessor = new GreekGodsPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "JumpHigh2":
                        preProcessor = new JumpHigh2PreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "ShouXin":
                    case "DiamondTreasure":
                        preProcessor = new ShouXinPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "skrskr":
                        preProcessor = new SkrSkrPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "InvincibleElephant":
                        preProcessor = new InvincibleElephantPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "GuGuGu3":
                        preProcessor = new GuGuGu3PreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "FootballBaby":
                        preProcessor = new FootballBabyPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    case "MoneyTree":
                        preProcessor = new MoneyTreePreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                    default:
                        preProcessor = new SpinDataPreProcess();
                        processTask = preProcessor.startPreProcess(dbWorker, strGameName);
                        break;
                }
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
