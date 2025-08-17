using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Hocon;
using System.IO;
using PGSpinDBBuilder.PGFetcher;

namespace PGSpinDBBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string strConfigText = File.ReadAllText("config.hocon");
            var config = HoconParser.Parse(strConfigText);

            IList<string> strProxyList  = config.GetStringList("proxyList");
            string  strProxyUserID      = config.GetString("proxyUserID");
            string  strProxyPassword    = config.GetString("proxyPassword");
            string  strGameName         = config.GetString("gameName");
            int     gameID              = config.GetInt("gameID");
            float   betSize             = config.GetFloat("betSize");
            int     betLevel            = config.GetInt("betLevel");
            double  minOdd              = config.GetDouble("minOdd");
            double  maxOdd              = config.GetDouble("maxOdd");
            double  realBet             = config.GetDouble("realBet");
            bool    isOnlyFree          = config.GetBoolean("onlyFree", false);
            int     mode                = config.GetInt("mode");

            List<Task> taskList = new List<Task>();
            
            PGSpinDataFetcher[] fetchers = new PGSpinDataFetcher[100];
            for (int i = 0; i < 100; i++)
            {
                fetchers[i] = createFetcher("", "", "", gameID, betSize, betLevel);
                taskList.Add(fetchers[i].startFetch());
            }
            SpinDataQueue.Instance.setDefaultBet(realBet);
            if(strGameName == "Medusa2")
                taskList.Add(SpinDataQueue.Instance.processGroupQueue(strGameName, minOdd, maxOdd, isOnlyFree, mode));
            else           
                taskList.Add(SpinDataQueue.Instance.processQueue(strGameName, minOdd, maxOdd, isOnlyFree, mode));

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                Console.WriteLine("Stopping.. Please wait..");
                e.Cancel = true;
                for (int i = 0; i < fetchers.Length; i++)
                    fetchers[i].doStop();
                SpinDataQueue.Instance.doStop();
            };

            Task.WhenAll(taskList.ToArray()).Wait();
        }

        protected static PGSpinDataFetcher createFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel)
        {
            switch(gameID)
            {
                case 74:
                case 65:
                case 104:
                    return new MahjongWays2Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 106:
                    return new WaysOfTheQilinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 135:
                    return new WildBountyShowdownFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 71:
                    return new CaishenWinsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 98:
                    return new FortuneOxFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 1340277:
                    return new AsgardianRisingFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 117:
                    return new CocktailNightsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 126:
                    return new FortuneTigerFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 73:
                    return new EgyptBookOfMysteryFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 67:
                    return new ShaolinSoccerFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 1372643:
                    return new DinnerDelightsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 53:
                    return new TheGreatIcescapeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 84:
                    return new QueenOfBountyFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 90:
                    return new SecretOfCleopatraFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 80:
                    return new CircusDelightFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 63:
                    return new DragonTigerLuckFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 24:
                    return new WinWinWonFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);                    
                case 85:
                    return new Genies3WishesFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 61:
                    return new FlirtingScholarFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 1:
                    return new HoneyTrapOfDiaoChanFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                case 6:
                    return new Medusa2Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);
                default:
                    return new NewPGSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel);

            }
        }
    }

    
}
