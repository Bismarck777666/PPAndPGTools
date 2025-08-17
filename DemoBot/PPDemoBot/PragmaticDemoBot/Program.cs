using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Hocon;
using System.IO;
using PragmaticDemoBot.NewFetchers;
using PragmaticDemoBot;
using System.Security.Cryptography;

namespace PragmaticDemoBot
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
            string  strGameSymbol       = config.GetString("gameSymbol");
            double  defaultC            = config.GetDouble("defaultC");
            int     lineCount           = config.GetInt("lineCount");
            string  strGameURL          = config.GetString("gameURL");
            double  minOdd              = config.GetDouble("minOdd");
            string  clientVersion       = config.GetString("clientVersion");
            double  realBet             = config.GetDouble("realBet");
            bool    isOnlyFree          = config.GetBoolean("onlyFree", false);
            int     mode                = config.GetInt("mode");

            ISpinDataFetcher[]  fetchers = new ISpinDataFetcher[strProxyList.Count];
            List<Task>          taskList = new List<Task>();
            for(int i = 0; i < strProxyList.Count; i++)
            {
                fetchers[i] = createFetcher(strGameName, strProxyList[i], strProxyUserID, strProxyPassword, clientVersion, realBet);
                taskList.Add(fetchers[i].startFetch(strGameSymbol, strGameURL, defaultC, lineCount));
            }

            SpinDataQueue.Instance.setDefaultBet(realBet);
            taskList.Add(SpinDataQueue.Instance.processQueue(strGameName, minOdd, isOnlyFree, mode));

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

        static void calcProbForYumYumPower()
        {
            double[] upProbs  = new double[] { 0.5014, 0.4288, 0.4446, 0.4547 };
            int      startPos = 4;

            Random random     = new Random((int)DateTime.Now.Ticks);
            int[]  callCounts = new int[5];
            for(int i = 0; i < 10000000; i++)
            {
                int currentPos = startPos;
                do
                {
                    if (random.Next(0, 100) < 50)
                    {
                        callCounts[currentPos - 1]++;
                        break;
                    }
                    else
                    {
                        if (random.NextDouble() < upProbs[currentPos - 1])
                            currentPos++;
                        else
                            currentPos--;

                        if (currentPos == 0)
                            break;

                        if(currentPos == 5)
                        { 
                            callCounts[currentPos - 1]++;
                            break;
                        }
                    }

                } while (true);
            }
            double[] probs = new double[5];
            double[] averageOdds = new double[] { 96.510593220339, 96.9302586206896, 164.726819620253, 229.680113636364, 313.268112244898 };
            double sum = 0.0;
            for (int i = 0; i < 5; i++)
            {
                probs[i] = (double)callCounts[i] / (double)10000000;
                sum += (probs[i] * averageOdds[i]);
            }


        }
        static ISpinDataFetcher createFetcher(string strGameName, string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet)
        {
            ISpinDataFetcher fetcher = null;
            switch(strGameName)
            {
                case "FruitParty":
                    fetcher = new FruitPartyFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "AztecKingMega":
                    fetcher = new AztecKingMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "MustangGold":
                    fetcher = new MustangGoldFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "DragonHoldAndSpin":
                    fetcher = new DragonHoldAndSpinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Asgard":
                    fetcher = new AsgardFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "BroncoSpirit":
                    fetcher = new BroncoSpiritFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "EmptyTheBank":
                    fetcher = new EmptyTheBankFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "HotFiesta":
                    fetcher = new GameSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "SweetBonanza":
                case "MadameDestinyMegaWays":
                case "BuffaloKingMega":
                case "BigBassHalloween":
                case "StarlightPrincess1000":
                case "GatesOfOlympus1000":
                case "OViralataCaramelo":
                case "SarayRuyasi":
                case "SweetArgentina":
                case "GatesOfOlympusXmas1000":
                case "RizkBonanza":
                case "PeruCandyland":
                case "CandyStashBonanza":
                case "HadesInferno1000":
                case "GamesInOlympus1000":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "SweetBonanzaDice":
                    fetcher = new SweetBonanzaDiceFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "EightDragons":
                    fetcher = new EightDragonsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "HandOfMidas":
                    fetcher = new HandOfMidasFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "PirateGoldDelux":
                    fetcher = new PirateGoldDeluxFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PeKingLuck":
                    fetcher = new PeKingLuckFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "TheTweetyHouse":
                    fetcher = new TheTweetyHouseFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PyramidBonanza":
                    fetcher = new PyramidBonanzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "PowerOfThorMega":
                    fetcher = new PowerOfThorMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "CashElevator":
                    fetcher = new CashElevatorFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "RockVegas":
                    fetcher = new RockVegasFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Queenie":
                    fetcher = new QueenieFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Cleocatra":
                    fetcher = new CleocatraFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ZombieCarnival":
                    fetcher = new ZombieCarnivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "TheUltimate5":
                    fetcher = new TheUltimate5Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "KoiPond":
                    fetcher = new KoiPondFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "TreasureWild":
                    fetcher = new TreasureWildFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "BigJuan":
                    fetcher = new BigJuanFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SugarRush":
                    fetcher = new SugarRushFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "LeprechaunSong":
                    fetcher = new LeprechaunSongFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "WildWestGoldMega":
                    fetcher = new WildWestGoldMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "StarlightPrincess":
                    fetcher = new StarlightPrincessFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "SweetBonanzaXmas":
                    fetcher = new SweetBonanzaXmasFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "SwordOfAres":
                    fetcher = new SwordOfAresFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "BonanzaGold":
                    fetcher = new BonanzaGoldFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "BarnFestival":
                    fetcher = new BarnFestivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "RiseOfSamuraiMega":
                    fetcher = new RiseOfSamuraiMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "FiveFrozenCharmsMegaways":
                    fetcher = new FiveFrozenCharmsMegawaysFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "BigBassSplash":
                case "RainbowReels":
                case "GravityBonanza":
                case "TwilightPrincess":
                case "InfectiveWild":
                case "HappyHooves":
                case "TheWildGang":
                case "SugarRushXmas":
                case "CandyJarClusters":
                case "FireStampede":
                case "JuicyFruitsMultihold":
                case "BladeAndFangs":
                case "TheBigDawgs":
                case "CastleOfFire":
                case "BlazingWildsMegaways":
                case "LokisRiches":
                case "RedHotLuck":
                case "TheAlterEgo":
                case "PompeiiMegareelsMegaways":
                case "StrawberryCocktail":
                case "GearsOfHorus":
                case "SugarRush1000":
                case "TheDogHouseDogOrAlive":
                case "AztecPowernudge":
                case "IceLobster":
                case "BarnyardMegahaysMegaways":
                case "CandyBlitzBombs":
                case "FruityTreats":
                case "HeartOfCleopatra":
                case "DwarfAndDragon":
                case "MedusasStone":
                case "HandOfMidas2":
                case "SweetKingdom":
                case "CrankItUp":
                case "JokersJewelsWild":
                case "JackpotHunter":
                case "BowOfArtemis":
                case "YetiQuest":
                case "AztecTreasureHunt":
                case "ForgingWilds":
                case "GemElevator":
                case "AngelVSSinner":
                case "MysteryMice":
                case "TheDogHouseMuttleyCrew":
                case "FortuneHitnRoll":
                case "ChestsOfCaiShen":
                case "BigBassHalloween2":
                case "SevenCloversOfFortune":
                case "CandyCorner":
                case "Moleionaire":
                case "HimalayanWild":
                case "EternalEmpressFreezeTime":
                case "TinyToads":
                case "AztecSmash":
                case "AztecGemsMega":
                case "BrickHouseBonanza":
                case "WildWildebeestWins":
                case "SavannahLegend":
                case "AncientIslandMega":
                case "SweetBaklava":
                case "JohnHunterAndGalileosSecrets":
                case "LuckyWildPub":
                case "BlitzSuperWheel":
                case "LuckyMouse":
                case "LuckyTiger":
                case "TheDogHouseRoyalHunt":
                case "BanditMega":
                case "RideTheLightning":
                case "LuckyDog":
                case "MahjongWinsSuperScatter":
                case "RatinhoSortudo":
                case "EyeOfSpartacus":
                case "TempleGuardians":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "BewareTheDeepMegaways":
                    fetcher = new BewareTheDeepMegawaysFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "BigBassDayAtTheRaces":
                    fetcher = new BigBassDayAtTheRacesFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "FloatingDragonNewYearFestivalUltraMegawaysHoldAndSpin":
                    fetcher = new FloatingDragonNewYearFestivalUltraMegawaysHoldAndSpinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "WildWildBananas":
                    fetcher = new WildWildBananasFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "ReelBanks":
                    fetcher = new WildWildBananasFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PizzaPizzaPizza":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "HotPepper":
                    fetcher = new HotPepperFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ShieldOfSparta":
                    fetcher = new HotPepperFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SweetPowernudge":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "DragonHero":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "BigBassKeepingItReel":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "PirateGoldenAge":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "GemsOfSerengeti":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "FuryOfOdinMega":
                    fetcher = new FuryOfOdinMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "FirebirdSpirit":
                    fetcher = new BarnFestivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ToweringFortunes":
                    fetcher = new BarnFestivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "BookOfTutRespin":
                    fetcher = new BookOfTutRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "StarlightChristmas":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "BiggerBassBlizzard":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "RiseOfSamurai3":
                case "LuckyNewYearTigerTreasures":
                case "BookOfAztecKing":
                case "BubblePop":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;
                case "GatesOfAztec":
                case "GatesOfGatotKaca":
                case "HokkaidoWolf":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "KingdomOfAsgard":
                case "DiscoLady":
                case "FruitsOfTheAmazon":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "FiveRabbitsMega":
                    fetcher = new FiveRabbitsMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "SantaGreatGifts":
                    fetcher = new SantaGreatGiftsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "AztecBlaze":
                    fetcher = new AztecBlazeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "LuckyFishing":
                case "LegendOfHeroesMega":
                    fetcher = new LuckyFishingFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "MammothGoldMega":
                case "MuertosMultiplierMega":
                    fetcher = new MammothGoldMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SpinAndScoreMega":
                    fetcher = new SpinAndScoreMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "YumYumPowerWays":
                    fetcher = new YumYumPowerWaysFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SecretCityGold":
                    fetcher = new BarnFestivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "FishEye":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "MonsterSuperlanche":
                case "GoblinHeistPowerNudge":
                    fetcher = new PizzaPizzaPizzaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "WildDepths":
                    fetcher = new WildDepthsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "DragonKingdomEyesOfFire":
                    fetcher = new DragonKingdomEyesOfFireFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "EmeraldKingClassic":
                    fetcher = new EmeraldKingClassicFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ReleaseTheKraken2":
                    fetcher = new ReleaseTheKraken2Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "GoldenBeauty":
                    fetcher = new GoldenBeautyFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PinupGirls":
                    fetcher = new PinupGirlsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "CandyVillage":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "CoffeeWild":
                case "CheekyEmperor":
                case "AztecKing":
                case "BullFiesta":
                case "HockeyAttack":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;
                case "FloatingDragonMega":
                    fetcher = new FloatingDragonMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "LuckyLightning":
                case "TropicalTiki":
                case "GemsBonanza":
                case "GoldParty":
                case "SantasWonderland":
                case "FruitParty2":
                case "RiseOfGizaPowerNudge":
                case "CrystalCavernsMega":
                case "WildBeachParty":
                    fetcher = new BarnFestivalFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ReleaseTheKraken":
                    fetcher = new ReleaseTheKrakenFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "NorthGuardians":
                    fetcher = new NorthGuardiansFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "DragoJewelsOfFortune":
                    fetcher = new DragoJewelsOfFortuneFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Mochimon":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "GatotKacaFury":
                    fetcher = new AsiaGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "ChristmasCarolMega":
                    fetcher = new ChristmasCarolMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "ExtraJuicyMega":                    
                    fetcher = new ExtraJuicyMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "ChilliHeatMega":
                case "WildWildRiches":
                case "SmugglersCove":
                    fetcher = new EuroGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "GatesOfValhalla":
                case "JohnHunterAndTheQuest":
                case "ChickenDrop":
                case "CurseOfTheWerewolfMega":
                case "BookOfTut":
                case "OctobeerFortunes":
                case "JokerJewels":
                case "AztecBonanza":
                case "Triple8Dragons":
                case "FireStrike":
                case "CaishenCash":
                case "JohnHunterTombScarabQueen":
                case "TheAmazingMoneyMachine":
                case "ElementalGemsMega":
                case "PhoenixForge":
                case "BigBassBonanzaMega":
                case "BountyGold":
                case "MightOfRa":
                case "GreekGods":
                case "MasterJoker":
                case "ExtraJuicy":
                case "EyeOfTheStorm":
                case "ChilliHeat":
                case "TreeOfRiches":
                case "PyramidKing":
                case "GoldenPig":
                case "CaishenGold":
                case "PiggyBankBills":
                case "Super7s":
                case "LuckyGraceCharm":
                case "TripleTigers":
                case "MysteriousEgypt":
                case "MysticChief":
                case "TheTigerWarrior":
                case "DragonTiger":
                case "MoneyMoneyMoney":
                case "DayOfDead":
                case "JohnHunterAndTheMayanGods":
                case "WildWalker":
                case "FuFuFu":
                case "LuckyDragonBall":
                case "FruitRainbow":
                case "SuperJoker":
                case "HotChilli":
                case "JohnHunterAndTheAztecTreasure":
                case "WildPixies":
                case "WildGladiators":
                case "TreasureHorse":
                case "SafariKing":
                case "TripleDragons":
                case "MadameDestiny":
                case "MasterChensFortune":
                case "AncientEgypt":
                case "MonkeyMadness":
                case "GoldRush":
                case "Santa":
                case "ThreeGenieWishes":
                case "HerculesSonOfZeus":
                case "Lucky Dragons":
                case "DwarvenGoldDeluxe":
                case "SevenMonkeys":
                case "JewelRush":
                    fetcher = new EuroGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;
                case "ZeusVsHadesGodsOfWar":
                    fetcher = new ZeusVsHadesGodsOfWarFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "KingdomOfTheDead":
                    fetcher = new KingdomOfTheDeadFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "Devils13":
                    fetcher = new Devils13Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "GreatReef":
                    fetcher = new GreatReefFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "EmeraldKing":
                    fetcher = new EmeraldKingFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "DaVinciTreasure":
                case "FairytaleFortune":
                    fetcher = new DaVinciTreasureFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "FishinReels":
                    fetcher = new FishinReelsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "HoneyHoneyHoney":
                    fetcher = new HoneyHoneyHoneyFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "VampiresVSWolves":
                    fetcher = new VampiresVSWolvesFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PixieWings":
                case "DragonKingdom":
                case "JourneyToTheWest":
                case "RiseOfSamurai":
                    fetcher = new PixieWingsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "LadyGodiva":
                case "MightyKong":
                    fetcher = new LadyGodivaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "TheGreatChickenEscape":
                    fetcher = new TheGreatChickenEscapeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "StreetRacer":
                    fetcher = new StreetRacerFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "TheWildMachine":
                    fetcher = new TheWildMachineFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SevenPiggies":
                    fetcher = new SevenPiggiesFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "OlympusGates":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, false);
                    break;
                case "CashChips":
                    fetcher = new CashChipsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "FiveLionsMega":
                    fetcher = new FiveLionsMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "GreatRhinoMega":
                    fetcher = new GreatRhinoMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "CashBonanza":
                case "TheMagicCauldron":
                case "StarPiratesCode":
                case "ThreeStarFortune":
                case "StarBounty":
                case "MagicJourney":
                case "AladdinAndTheSorcerer":
                case "VegasNights":
                case "HotSafari":
                case "PantherQueen":
                case "TheCatfatherPartII":
                case "HockeyLeagueWildMatch":
                case "TheCatfather":
                case "MagicCrystals":
                case "JokersJewelDice":
                case "TheDogHouseDiceShow":
                case "TripleJokers":
                case "JurassicGiants":
                case "BookOfVikings":
                case "JokersJewelsHot":
                case "EmotiWins":
                case "JellyCandy":
                case "BisonSpirit":
                case "FruitPartyDice":
                case "Tukanito":
                case "SweetCherryBlossom":
                case "WealthyFrog":
                case "PlushieWins":
                case "PigFarm":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;
                case "AladdinsTreasure":
                    fetcher = new AladdinsTreasureFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "ThreeKingdomsBattleOfRedCliffs":
                    fetcher = new ThreeKingdomsBattleOfRedCliffsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "DwarvenGold":
                    fetcher = new DwarvenGoldFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "LadyOfTheMoon":
                    fetcher = new LadyOfTheMoonFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "HockeyLeague":
                case "TalesOfEgypt":
                case "GloriousRome":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    (fetcher as EuroNoWinRespinFetcher).setV3False();
                    break;

                case "RomeoAndJuliet":
                    fetcher = new RomeoAndJulietFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "QueenOfAtlantis":
                    fetcher = new QueenOfAtlantisFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "JasmineDreams":
                case "CowboyCoins":
                case "FrogsAndBugs":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true, true);
                    break;
                case "RobberStrike":
                    fetcher = new RobberStrikeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false, true);
                    break;
                case "TheRedQueen":
                case "MoonShot":
                case "WildCelebrityBusMega":
                case "PiggyBankers":
                case "PubKings":
                case "MustangTrail":
                case "CandyBlitz":
                case "CyclopsSmash":
                case "FrozenTropics":
                case "FortunesOfAztec":
                case "GoldOasis":
                case "SugarSupremePowernudge":
                case "RiseOfPyramids":
                case "HeroicSpins":
                case "StarlightPrincessPachi":
                case "TigreSortudo":
                case "BigBassHoldAndSpinnerMegaways":
                case "Wildies":
                case "MiningRush":
                case "MahjongWins":
                case "MahjongWins3BlackScatter":
                case "TouroSortudo":
                case "ResurrectingRiches":
                case "SleepingDragon":
                case "FiestaFortune":
                case "MajesticExpressGoldRun":
                case "LuckyMonkey":
                case "LuckyTiger1000":
                case "LuckyPhoenix":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "WisdomOfAthena":
                case "HellvisWild":
                case "LobsterBobsCrazyCrabShack":
                case "SkyBounty":
                case "DiamondCascade":
                case "ForgeOfOlympus":
                case "BookOfTutMegaways":
                case "EightGoldenDragonChallenge":
                case "DemonPots":
                case "TundrasFortune":
                case "TheMoneyMenMegaways":
                case "VikingForge":
                case "TimberStacks":
                case "NileFortune":
                case "BigBassChristmasBash":
                case "DingDongChristmasBells":
                case "YearOfTheDragonKing":
                case "GoodLuckAndGoodFortune":
                case "TreesOfTreasure":
                case "BigBassFloatsMyBoat":
                case "MightyMunchingMelons":
                case "WheelOGold":
                case "PotOfFortune":
                case "FirePortals":
                case "BigBurgerLoad":
                case "RipeRewards":
                case "BigBassSecretsOfTheGoldenLake":
                case "LobsterBobsSeaFoodAndWinIt":
                case "ReleaseTheBison":
                case "FrontRunnerOddsOn":
                case "BigBassBonanzaReelAction":
                case "SweetBonanza1000":
                case "Devilicious":
                case "BuffaloKingUntamedMega":
                case "HotToBurnMultiplier":
                case "SamuraiCode":
                case "DynamiteDigginDoug":
                case "TheConqueror":
                case "DragonGold88":
                case "RunningSushi":
                case "HotToBurn7Deadly":
                case "SumoSupremeMega":
                case "BigBassDoubleDownDeluxe":
                case "ReleaseTheKrakenMega":
                case "OodlesOfNoodles":
                case "BadgeBlitz":
                case "WisdomOfAthena1000":
                case "ReleaseTheKrakenMegaways":
                case "FangtasticFreespins":
                case "WolfGoldUltimate":
                case "BigBassXmasExtreme":
                case "MightOfFreyaMega":
                case "PenguinsChristmasPartyTime":
                case "DragonKingHotPots":
                case "BigBassBonanza3Reeler":
                case "SantasXmasRush":
                case "MoneyStacksMega":
                case "IrishCrown":
                case "FloatingDragonYearOfTheSnake":
                case "WildWildPearls":
                case "EscapeThePyramidFireIce":
                case "BiggerBassSplash":
                case "MahjongWinsGongXiFaCai":
                case "GreedyFortunePig":
                case "FonzosFelineFortunes":
                case "PeppesPepperoniPizzaPlaza":
                case "BigBassReturnToTheRaces":
                case "RagingWaterfallMega":
                case "TriplePotGold":
                case "WildWildJoker":
                case "BigBassBonanza1000":
                case "GatesOfOlympusSuperScatter":
                case "BookOfMonsters":
                case "MadMuertos":
                case "WitchHeartMegaways":
                case "SweetBonanza1000Dice":
                case "HotSake":
                case "JumboSafari":
                case "WildWestGoldBlazingBounty":
                case "BigBassBoxingBonusRound":
                case "FingerLicknFreeSpins":
                case "GoldParty2AfterHours":
                    fetcher = new EuroNoWinRespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "ChaseForGlory":
                    fetcher = new ChaseForGloryFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet);
                    break;
                case "BigBassAmazonXtreme":
                    fetcher = new BigBassAmazonXtremeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "VoodooMagic":
                    fetcher = new VoodooMagicFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "WildWildRichesMega":
                    fetcher = new WildWildRichesMegaFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "ThreeBuzzingWilds":
                    fetcher = new ThreeBuzzingWildsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Fire88":
                case "TicTacTake":
                    fetcher = new Fire88Fetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;

                case "BookOfTheFallen":
                    fetcher = new BookOfTheFallenFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;                    
                case "MagicianSecrets":
                    fetcher = new MagicianSecretsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SuperX":
                    fetcher = new SuperXFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "EmperorCaishen":
                    fetcher = new EmperorCaishenFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "WildSpells":
                    fetcher = new WildSpellsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "Triple8Gold":
                case "MoneyRoll":
                case "IrishCharms":
                case "DiamondsAreForever":
                    fetcher = new ClassicGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "PowerOfMerlinMegaways":
                    fetcher = new PowerOfMerlinMegawaysFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "RocketBlastMegaways":
                    fetcher = new RocketBlastMegawaysFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;                    
                case "GorillaMayhem":
                case "FireArcher":
                case "CandyStars":
                case "WildHopAndDrop":
                case "BookOfGoldenSands":
                case "StrikingHot5":
                case "SnakesAndLaddersSnakeEyes":
                case "CrownOfFire":
                case "BlackBull":
                case "MagicMoneyMaze":
                case "FireHot5":
                case "FireHot20":
                case "FireHot40":
                case "FireHot100":
                case "ShiningHot5":
                case "ShiningHot20":
                case "ShiningHot40":
                case "ShiningHot100":
                case "BombBonanza":
                case "FireStrike2":
                case "HotToBurnExtreme":
                case "CosmicCash":
                case "TheGreatStickUp":
                case "CloverGold":
                case "MahjongPanda":
                case "PeakPower":
                case "RabbitGarden":
                case "MysteryOfTheOrient":
                case "WildWestDuels":
                case "TheDogHouseMultihold":
                case "TheKnightKing":
                case "ThreeDancingMonkeys":
                case "AfricanElephant":
                case "JaneHunter":
                case "GodsOfGiza":
                case "ExcaliburUnleashed":
                case "WildBisonCharge":
                case "KnightHotSpotz":
                case "DiamondsOfEgypt":
                case "StickyBees":
                case "PiratesPub":
                case "CountryFarming":
                case "FatPanda":
                case "HeistForTheGoldenNuggets":
                case "SpellbindingMystery":
                case "JaneHunterAndTheMaskOfMontezuma":
                case "CashBox":
                    fetcher = new EuroGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "BusyBees":                    
                    fetcher = new BusyBeesFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "LeprechaunCarol":
                    fetcher = new LeprechaunCarolFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "SpiritOfAdventure":
                    fetcher = new SpiritOfAdventureFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;                    
                case "ClubTropicana":
                case "GreedyWolf":
                case "DrillThatGold":
                case "BigBassHoldSpinner":
                case "LampOfInfinity":
                case "FloatingDragonDragonBoatFestival":
                    fetcher = new EuroGameFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "TemujinTreasures":
                    fetcher = new TemujinTreasuresFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "AztecGemsDeluxe":
                    fetcher = new AztecGemsDeluxeFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "DownTheRails":
                    fetcher = new DownTheRailsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "LittleGem":
                    fetcher = new LittleGemFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "QueenOfGods":
                    fetcher = new QueenOfGodsFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
                case "EyeOfCleopatra":
                    fetcher = new EyeOfCleopatraFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true);
                    break;
                case "BigBassMissionFishin":
                    fetcher = new BigBassMissionFishinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "MoneyStacks":
                    fetcher = new MoneyStacksFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "6Jokers":
                    fetcher = new SixJokerFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "VolcanoGoddess":
                    fetcher = new VolcanoGoddessFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "JokersJewelsCash":
                    fetcher = new EuroNoWinV5RespinFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "GemFireFortune":
                case "GatesOfHades":
                case "OlympusWinsSuperScatter":
                case "MummysJewels":
                case "ChilliHeatSpicySpins":
                case "GemTrio":
                case "Argonauts":
                    fetcher = new SInfoFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, true);
                    break;
                case "WavesOfPoseidon":
                case "ClubTropicanaHappyHour":
                case "FortuneOfAztec":
                case "AlienInvaders":
                case "SweetBonanzaSuperScatter":
                case "YouCanPiggyBankOnIt":
                case "ZombieSchoolMega":
                case "BigBassReelRepeat":
                    fetcher = new SInfoFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, true, true);
                    break;
                case "MasterGems":
                    fetcher = new SInfoFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false, false);
                    break;
                default:
                    fetcher = new GameSpinDataFetcher(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, false);
                    break;
            }
            return fetcher;
        }
    }
}
