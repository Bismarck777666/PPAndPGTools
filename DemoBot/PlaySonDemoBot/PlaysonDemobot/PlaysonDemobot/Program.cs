using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hocon;

namespace PlaysonDemobot
{
    class Program
    {
        static void Main(string[] args)
        {
            string strConfigText = File.ReadAllText("config.hocon");
            var config = HoconParser.Parse(strConfigText);

            IList<string> strProxyList  = config.GetStringList("proxyList");
            string strProxyUserID       = config.GetString("proxyUserID");
            string strProxyPassword     = config.GetString("proxyPassword");
            string strGameName          = config.GetString("gameName");
            string strGameSymbol        = config.GetString("gameSymbol");
            double defaultC             = config.GetDouble("defaultC");
            int lineCount               = config.GetInt("lineCount");
            string strGameURL           = config.GetString("gameURL");
            double minOdd               = config.GetDouble("minOdd");
            string clientVersion        = config.GetString("clientVersion");
            double realBet              = config.GetDouble("realBet");
            bool isOnlyFree             = config.GetBoolean("onlyFree", false);
            bool isBuy                  = config.GetBoolean("isBuy", false);
            IList<int> optionList       = config.GetIntList("optionList",new List<int>());

            List<Task> taskList = new List<Task>();
#if PROXY
            ISpinDataFetcher[] fetchers = new ISpinDataFetcher[strProxyList.Count];
            for (int i = 0; i < strProxyList.Count; i++)
            {
                fetchers[i] = createPlaysonFetcher(strGameName, strProxyList[i], strProxyUserID, strProxyPassword,isBuy,optionList);
                taskList.Add(fetchers[i].startFetch(strGameSymbol, strGameURL, defaultC, realBet, lineCount,clientVersion));
            }
#else
            ISpinDataFetcher[] fetchers = new ISpinDataFetcher[50];
            for (int i = 0; i < 50; i++)
            {
                fetchers[i] = createPlaysonFetcher(strGameName, strProxyList[0], strProxyUserID, strProxyPassword, isBuy, optionList);
                taskList.Add(fetchers[i].startFetch(strGameSymbol, strGameURL, defaultC, realBet, lineCount, clientVersion));
            }
#endif

            SpinDataQueue.Instance.setDefaultBet(realBet);
            taskList.Add(SpinDataQueue.Instance.processQueue(strGameName, minOdd, isOnlyFree));

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

        static ISpinDataFetcher createPlaysonFetcher(string strGameName, string strProxyInfo, string strProxyUserID, string strProxyPassword,bool isBuy, IList<int> optionList)
        {
            switch (strGameName)
            {
                case "SolarQueen":
                case "SolarKing":
                case "SolarTemple":
                    return new SolarQueenSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword,isBuy);
                case "SolarQueenMegaways":
                    return new SolarQueenMegaSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "RoyalJoker":
                case "CoinStrike":
                case "RoyalCoins2":
                case "DiamondFort":
                case "RoyalCoins":
                case "SuperSunnyFruits":
                case "DiamondWins":
                case "RoyalFort":
                case "CrownNDiamonds":
                case "EnergyCoins":
                case "FireCoins":
                case "DiamondsPower":
                case "PowerCrown":
                case "CloverCharm":
                case "EnergyJoker":
                    return new HoldAndWinSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "GizaNights":
                case "MammothPeak":
                case "PirateChest":
                case "LuxorGold":
                case "LionGems":
                case "BuffaloXmas":
                case "SpiritOfEgypt":
                case "EaglePower":
                case "RichDiamonds":
                case "DivineDragon":
                case "WolfPower":
                case "PearlBeauty":
                case "BuffaloPower":
                case "SunnyFruits":
                case "VikingsFortune":
                case "BuffaloPower2":
                case "EmpireGold":
                case "WolfLand":
                case "PearlOcean":
                case "SherwoodCoins":
                case "FireTemple":
                case "3PotsRiches":
                case "3PotsExtra":
                case "3MagicLamps":
                case "SunnyFruits2":
                case "ArizonaHeist":
                    return new GizaNightsSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "HitTheBank":
                case "UltraFort":
                case "RubyHit":
                    return new HitTheBankSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "TreasuresFire":
                case "LegendOfCleopatraMega":
                case "SuperBurningWinsRespin":
                case "JellyValley":
                case "CrystalLand":
                    return new TembleSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "CrystalLand2":
                    return new CrystalLand2Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "WildBurningWins5":
                case "MegaBurningWins27":
                    return new WildBurningWins5Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "WolfPowerMega":
                case "BuffaloMegaways":
                    return new WolfPowerMegaDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "JokersCoins":
                case "HotCoins":
                    return new JokersCoinsSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
                case "BookOfGoldChoice":
                    return new PlaysonOptionSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy, optionList);
                case "PirateSharky":
                case "BurningFort":
                case "BurningWinsX2":
                case "FiveFortunator":
                case "HotBurningWins":
                case "NineHappyPharaohs":
                case "BookOfGoldDh":
                case "HandOfGold":
                case "ThreeFruitsWin2Hit":
                case "StarsNFruits2Hit":
                case "FiveSuperSevensNFruits6":
                case "RiseOfEgyptDeluxe":
                case "BookOfGoldMultichance":
                case "SevensNFruits6":
                case "ImperialFruits100":
                case "BookOfGoldClassic":
                case "ThreeFruitsWin10":
                case "FruitsAndJokers100":
                case "ImperialFruits5":
                case "SevensNFruits20":
                case "FruitsAndClovers20":
                case "MightyAfrica":
                case "HundredJokerStaxx":
                case "WildWarriors":
                case "BookOfGold":
                case "JokerExpand40":
                case "FruitsAndJokers20":
                case "SuperBurningWins":
                case "RiseOfEgypt":
                case "JokerExpand":
                case "BurningWins":
                case "FourtyJokerStaxx":
                case "SevensNFruits":
                case "LegendOfCleopatra":
                case "FruitsNStarsC":
                case "JuiceAndFruitsC":
                case "777sizzlingWins":
                case "BlazingWins":
                default:
                    return new PlaysonSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, isBuy);
            }
        }
    }
}
