using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Hocon;
using System.IO;

namespace HabaneroDemoBot
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
            string  strBrandGameId      = config.GetString("brandgameid");
            string  strGameSymbol       = config.GetString("gameSymbol");
            int     lineCount           = config.GetInt("lineCount");
            string  strGameURL          = config.GetString("gameURL");
            double  minOdd              = config.GetDouble("minOdd");
            string  clientVersion       = config.GetString("clientVersion");
            int     betLevel            = config.GetInt("betLevel");
            double  stake               = config.GetDouble("stake");
            int     coin                = config.GetInt("coin");
            double  realBet             = config.GetDouble("realBet");
            IList<int> freeOpts         = config.GetIntList("freeOpts");
            int gameType                = config.GetInt("gameType");
            int purType                 = config.GetInt("purType");


#if Proxy
            ISpinDataFetcher[]  fetchers = new ISpinDataFetcher[24];
            List<Task>          taskList = new List<Task>();
            for(int i = 0; i < 24; i++)
            {
                fetchers[i] = createHabaneroFetcher(strGameName, strProxyList[i], strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, betLevel, lineCount, coin, stake, realBet, freeOpts, purType);
                taskList.Add(fetchers[i].startFetch(strGameSymbol, strGameURL, realBet, lineCount));
            }
#else
            ISpinDataFetcher[] fetchers = new ISpinDataFetcher[100];
            List<Task> taskList = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                fetchers[i] = createHabaneroFetcher(strGameName, "", strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, betLevel, lineCount, coin, stake, realBet, freeOpts,purType);
                taskList.Add(fetchers[i].startFetch(strGameSymbol, strGameURL, realBet, lineCount));
            }

            
#endif

            SpinDataQueue.Instance.setDefaultBet(realBet);
            //taskList.Add(SpinDataQueue.Instance.processQueue(strGameName, minOdd, freeOpts.Count > 0, false));
            taskList.Add(SpinDataQueue.Instance.processQueue(strGameName, minOdd, gameType, false));

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

        static ISpinDataFetcher createHabaneroFetcher(string strGameName,string strProxyInfo,string strProxyUserID,string strProxyPassword,string strBrandGameId, string clientVersion,int betLevel,int lineCount,int coin,double stake,double realBet,IList<int> freeOpts, int purType)
        {
            ISpinDataFetcher fetcher = null;
            switch (strGameName)
            {
                case "TheKoiGate":
                case "FengHuang":
                case "DiscoBeats":
                case "MysticFortuneDeluxe":
                case "PandaPanda":
                case "MightyMedusa":
                case "SpaceGoonz":
                case "LuckyDurian":
                case "LanternLuck":
                case "NewYearsBash":
                case "BeforeTimeRunsOut":
                case "TotemTowers":
                case "TabernaDeLosMuertos":
                case "WealthInn":
                case "FaCaiShenDeluxe":
                case "NaughtySanta":
                case "HotHotHalloween":
                case "LuckyLucky":
                case "MountMazuma":
                case "TaikoBeats":
                case "HotHotFruit":
                case "WaysOfFortune":
                case "BirdOfThunder":
                case "SuperTwister":
                case "CoyoteCrash":
                case "DragonsThrone":
                case "RomanEmpire":
                case "RuffledUp":
                case "HaFaCaiShen":
                case "WickedWitch":
                case "GoldRush":
                case "Cashosaurus":
                case "DiscoFunk":
                case "RodeoDrive":
                case "SkysTheLimit":
                case "FrontierFortunes":
                case "GoldenUnicorn":
                case "SirBlingalot":
                case "TowerOfPizza":
                case "MummyMoney":
                case "BikiniIsland":
                case "EgyptianDreams":
                case "BarnstormerBucks":
                case "SuperStrike":
                case "JungleRumble":
                case "SpaceFortune":
                case "PamperMe":
                case "HaZeus":
                case "SOS":
                case "PoolShark":
                case "WeirdScience":
                case "BlackbeardsBounty":
                case "DrFeelgood":
                case "VikingsPlunder":
                case "TheDragonCastle":
                case "KingTutsTomb":
                case "TreasureTomb":
                case "DragonTigerGate":
                case "LegendaryBeasts":
                case "NaughtyWukong":
                case "TheBigDealDeluxe":
                case "PiratesPlunder":
                case "ShogunsLand":
                case "AzlandsGold":
                    fetcher = new SpinDataFetcher_1(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "TukTukThailand":
                    fetcher = new TukTukThailandFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, purType);
                    break;
                case "Rainbowmania":
                    fetcher = new RainbowmaniaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, purType);
                    break;
                case "Crystopia":
                case "SirensSpell":
                case "TootyFruityFruits":
                case "LegendOfNezha":
                case "WitchesTome":
                case "BikiniIslandDeluxe":
                case "MeowJanken":
                case "SlimeParty":
                case "FruityHalloween":
                case "SantasInn":
                    fetcher = new TootyFruityFruitsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, purType);
                    break;
                case "ZeusDeluxe":
                    fetcher = new ZeusDeluxeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, purType);
                    break;
                case "AtomicKittens":
                    fetcher = new AtomicKittensFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, purType);
                    break;
                case "SojuBomb":
                    fetcher = new SojuBombFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "NineTails":
                    fetcher = new NineTailsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "Jump":
                    fetcher = new JumpFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "Prost":
                    fetcher = new ProstFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "TabernaDeLosMuertosUltra":
                    fetcher = new TabernaDeLosMuertosUltraFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "HeySushi":
                    fetcher = new HeySushiFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "LaughingBuddha":
                    fetcher = new LaughingBuddhaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, freeOpts);
                    break;
                case "GoldenUnicornDeluxe":
                    fetcher = new GoldenUnicornDeluxeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "HauntedHouse":
                    fetcher = new HauntedHouseFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;




                case "FiveLuckyLions":
                    fetcher = new OptionalSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin, freeOpts);
                    break;
                case "ColossalGems":
                    fetcher = new ColossalGemsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "5Mariachis":
                    fetcher = new FiveMariachisFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "GrapeEscape":
                    fetcher = new GrapeEscapeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
                case "FireRooster":
                case "FourDivineBeasts":
                case "HappyApe":
                case "LuckyFortuneCat":
                case "Nuwa":
                case "ScruffyScallywags":
                case "TheDeadEscape":
                case "Jugglenaut":
                case "OceansCall":
                case "12Zodiacs":
                case "BombsAway":
                case "TheBigDeal":
                case "IndianCashCatcher":
                case "PuckerUpPrince":
                case "CashReef":
                case "QueenOfQueens243":
                case "QueenOfQueens1024":
                case "DragonsRealm":
                case "AllForOne":
                case "FlyingHigh":
                case "MrBling":
                case "MysticFortune":
                case "ArcticWonders":
                case "DoubleODollars":
                case "LittleGreenMoney":
                case "MonsterMashCash":
                case "ShaolinFortunes100":
                case "ShaolinFortunes243":
                case "CarnivalCash":
                case "TreasureDiver":
                case "KanesInferno":
                case "GalacticCash":
                case "HaZeus2":
                case "BuggyBonus":
                case "GlamRock":
                case "RideEmCowboy":
                default:
                    fetcher = new SpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strBrandGameId, clientVersion, realBet, lineCount, betLevel, stake, coin);
                    break;
            }
            return fetcher;
        }
    }
}
