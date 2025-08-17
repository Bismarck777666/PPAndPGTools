using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Common;
using System.Collections;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDBConvert
{
    public class MongoDBConvert
    {
        private static MongoDBConvert _sInstance = new MongoDBConvert();
        public static MongoDBConvert Instance
        {
            get
            {
                return _sInstance;
            }
        }
        private List<GameInfo> _gameInfoList = new List<GameInfo>();

        public MongoDBConvert()
        {
            GameInfo[] gameInfos = new GameInfo[]
            {
                /*
                new GameInfo("MahjongWays2",        false, false, false),
                new GameInfo("MahjongWays",         false, false, false),
                new GameInfo("FortuneOx",           false, false, false),
                new GameInfo("WildBandito",         false, false, false),

                new GameInfo("LegendOfPerseus",      false, false, false),
                new GameInfo("MidasFortune",         false, false, false),
                new GameInfo("FortuneTiger",         false, false, false),

                new GameInfo("CaptainBounty",        false, false, false),
                new GameInfo("DragonHatch",          false, false, false),
                new GameInfo("LeprechaunRiches",     false, false, false),
                new GameInfo("TheGreatIcescape",     false, false, false),
                new GameInfo("TheQueenBanquet",     true,  false, false, 75.0),

                new GameInfo("WildFireworks",        false, false, false),
                new GameInfo("ShaolinSoccer",        false, false, true),
                new GameInfo("PhoenixRises",         false, false, false),
                new GameInfo("RoosterRumble",        false, false, false),
                new GameInfo("DoubleFortune",        false, false, false),
                new GameInfo("FortuneMouse",         false, false, false),
                new GameInfo("ButterflyBlossom",     false, false, false),

                new GameInfo("SpiritedWonders",      false, false, false),
                new GameInfo("MajesticTreasures",    false, false, false),
                new GameInfo("GarudaGems",           false, false, false),
                new GameInfo("SupermarketSpree",     false, false, false),
                new GameInfo("MuayThaiChampion",     false, false, false),
                new GameInfo("FortuneRabbit",        false, false, false),
                new GameInfo("AlchemyGold",          false, false, false),
                new GameInfo("CandyBonanza",         false, false, false),
                
                
                new GameInfo("CandyBurst",              false, false, false),
                new GameInfo("CircusDelight",           false, false, false),
                new GameInfo("DestinyOfSunAndMoon",     false, false, false),
                new GameInfo("DragonLegend",            false, false, false),
                new GameInfo("DragonTigerLuck",         false, false, false),
                new GameInfo("EmperorsFavour",          false, false, false),
                new GameInfo("FortuneGods",             false, false, false),
                new GameInfo("GalacticGems",            false, false, false),
                new GameInfo("GaneshaGold",             false, false, false),
                new GameInfo("GemSaviourConquest",      false, false, false),
                new GameInfo("Genies3Wishes",           false, false, true ),
                new GameInfo("GuardiansOfIceAndFire",   false, false, false),
                new GameInfo("HeistStakes",             false, false, false),
                new GameInfo("JewelsOfProsperity",      false, false, false),
                new GameInfo("JourneyToTheWealth",      false, false, false),
                new GameInfo("LegendaryMonkeyKing",     false, false, false),
                new GameInfo("MermaidRiches",           false, false, false),
                new GameInfo("TotemWonders",            false, false, false),
                new GameInfo("WinWinWon",               false, false, false),
                */
                /*
                new GameInfo("BikiniParadise",             false, false, false),
                new GameInfo("FlirtingScholar",            false, false, false),
                new GameInfo("HawaiianTiki",               false, false, false),
                new GameInfo("HoneyTrapOfDiaoChan",        false, false, true),
                new GameInfo("JungleDelight",              false, false, false),
                new GameInfo("LegendOfHouYi",              false, false, false),
                new GameInfo("MaskCarnival",               false, false, false),
                new GameInfo("MrHallowWin",                false, false, false),
                new GameInfo("NinjaVsSamurai",             false, false, false),
                new GameInfo("PiggyGold",                  false, false, false),
                new GameInfo("PlushieFrenzy",              false, false, false),
                new GameInfo("ProsperityLion",             false, false, false),
                new GameInfo("RaiderJanesCryptOfFortune",  false, false, false),
                new GameInfo("ReelLove",                   false, false, false),
                new GameInfo("WinWinFishPrawnCrab",        false, false, false),
                new GameInfo("Medusa2",                    false, false, false),

                new GameInfo("GemSaviourSword",            false, false, false),
                new GameInfo("HipHopPanda",                false, false, false),
                new GameInfo("Medusa",                     false, false, false),
                new GameInfo("RavePartyFever",             false, false, false),
                new GameInfo("SantasGiftRush",             false, false, false),
                new GameInfo("SongkranSplash",             false, false, false),
                new GameInfo("VampiresCharm",              false, false, false),

                new GameInfo("MermaidRiches",            false, false, false),
                new GameInfo("UltimateStriker",          true,  true, false,  75.0),
                new GameInfo("GaneshaFortune",           true,  true, false, 75.0),
                new GameInfo("WildHeistCashout",         true,  true, false, 75.0),
                new GameInfo("ForgeOfWealth",            true,  true, false, 75.0),
                new GameInfo("MafiaMayhem",              true,  true, false, 75.0),
                new GameInfo("TsarTreasures",    true,  true, false, 75.0),

                new GameInfo("WereWolfsHunt",    true,  true, false, 75.0),                
                new GameInfo("CaishenWins",      true,  true, true,  50.0),
                new GameInfo("SecretOfCleopatra",true,  true, true,  75.0),                
                new GameInfo("SuperGolfDrive",   true,  true, false, 75.0),
                new GameInfo("MysticalSpirits",  true,  true, false, 75.0),
                new GameInfo("BakeryBonanza",    true,  true, false, 75.0),
                new GameInfo("EmojiRiches",                false, false, false),
                new GameInfo("DragonHatch2",     false, false, false),

                new GameInfo("TreasureOfAztec",     true,  true, false, 50.0),
                new GameInfo("LuckyNeko",           true,  true, false, 75.0),
                new GameInfo("WaysOfTheQilin",      true,  true, false, 75.0),
                new GameInfo("JurassicKingdom",     true,  true, false, 75.0),
                new GameInfo("CocktailNights",      true,  true, false, 75.0),
                new GameInfo("FortuneDragon",       true,  true, false, 5.0),

                new GameInfo("DinnerDelights",       true,  true, false, 75.0),
                new GameInfo("ThaiRiverWonders",     true,  true, false, 75.0),
                new GameInfo("CryptoGold",           true,  true, false, 75.0),
                new GameInfo("LuckyPiggy",           true,  true, false, 75.0),
                new GameInfo("SpeedWinner",          true,  true, false, 50.0),
                new GameInfo("TheQueenBanquet",      true,  true, false, 75.0),
                new GameInfo("DreamsOfMacau",        true,  true, false, 75.0),

                new GameInfo("WildBountyShowdown",   true,  true, false, 75.0),
                new GameInfo("AsgardianRising",      true,  true, false, 75.0),
                new GameInfo("WildCoaster",          true,  true, false, 75.0),
                new GameInfo("RiseOfApollo",         true,  true, false, 75.0),
                new GameInfo("QueenOfBounty",        false, false, true),
                new GameInfo("OrientalProsperity",   true,  true, false, 75.0),
                new GameInfo("ProsperityFortuneTree",true,  true, false, 75.0),
                new GameInfo("GemstonesGold",        true,  true, false, 75.0),
                new GameInfo("EgyptBookOfMystery",   true,  true, true,  75.0),
                new GameInfo("BaliVacation",             true,  true, false,  75.0),
                new GameInfo("BattlegroundRoyale",       true,  true, false,  75.0),
                new GameInfo("BuffaloWin",               true,  true, false,  75.0),
                new GameInfo("CruiseRoyale",             true,  true, false,  75.0),
                new GameInfo("LuckyCloverLady",          true,  true, false,  75.0),
                new GameInfo("FruityCandy",              true,  true, false,  75.0),
                new GameInfo("SafariWilds",              true,  true, false,  75.0),
                new GameInfo("GladiatorsGlory",          true,  true, false,  75.0),
                new GameInfo("NinjaRaccoonFrenzy",       true,  true, false,  75.0),
                new GameInfo("JackFrostsWinter",         true,  true, false,  75.0),
                new GameInfo("OperaDynasty",             true,  true, false,  75.0),
                new GameInfo("CashMania",                false,  false, false),
                new GameInfo("WildApe3258",              true,  true, false,  75.0),
                new GameInfo("PinataWins",             true,  true, false,  75.0),               
                new GameInfo("MysticPotion",            true,  true, false,  75.0),
                new GameInfo("AnubisWrath",             true,  true, false,  75.0),
                 */
                new GameInfo("YakuzaHonor",             true,  true, false,  75.0),
                new GameInfo("WingsOfIguazu",           false, false, false, 75.0),
                new GameInfo("ThreeCrazyPiggies",       true,  true, false,  75.0),
                new GameInfo("OishiDelights",           true,  true, false,  75.0),
                new GameInfo("MuseumMystery",           true,  true, false,  75.0),
                new GameInfo("RioFantasia",             false, false,false,  0.0),
                new GameInfo("ChocolateDeluxe",         true,  true, false,  75.0),
                new GameInfo("GeishasRevenge",          true,  true, false,  75.0),
                new GameInfo("FortuneSnake",            false, false,false,  0.0),
                new GameInfo("IncanWonders",            false, false,false,  0.0),
                new GameInfo("MrTreasuresFortune",      false, false,false,  0.0),
                new GameInfo("GraffitiRush",            true,  true, false,  75.0),
                new GameInfo("DoomsdayRampage",         false, false,false,  0.0),
                new GameInfo("KnockoutRiches",          true,  true, false,  75.0),
                new GameInfo("JackTheGiantHunter",      true,  true, false,  75.0),

            };
            _gameInfoList.AddRange(gameInfos);
        }
        private async Task writeInfoDataToMongoDB(GameInfo gameInfo, BsonDocument infoDocument, List<BsonDocument> documents, List<BsonDocument> sequences = null)
        {
            try
            {
                infoDocument["name"] = gameInfo.Name;
                var             dbClient        = new MongoClient("mongodb://127.0.0.1");
                IMongoDatabase  db              = dbClient.GetDatabase("pgspindb");
                var             gameInfoTable   = db.GetCollection<BsonDocument>("infos");
                await gameInfoTable.FindOneAndDeleteAsync(_ => _["name"] == gameInfo.Name);
                await db.DropCollectionAsync(gameInfo.Name);

                await gameInfoTable.InsertOneAsync(infoDocument);

                var collection                  = db.GetCollection<BsonDocument>(gameInfo.Name);

                if(gameInfo.SelFreeSpin)
                {
                    await collection.Indexes.CreateOneAsync("{ spintype : 1, odd: 1 }");
                    await collection.Indexes.CreateOneAsync("{ spintype : 1, ranges: 1 }");
                }
                else if (gameInfo.SupportPurchaseFree)
                {
                    await collection.Indexes.CreateOneAsync("{ spintype : 1, odd: 1 }");
                    await collection.Indexes.CreateOneAsync("{ odd: 1 }");
                }
                else
                {
                    await collection.Indexes.CreateOneAsync("{ odd: 1 }");
                }
                await collection.InsertManyAsync(documents);

                if(sequences != null)
                {
                    collection = db.GetCollection<BsonDocument>(gameInfo.Name + "_sequences");
                    await collection.InsertManyAsync(sequences);
                }
            }
            catch
            {

            }
        }
        private bool IsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
        private int findFreeSpinTypeForCaishenWins(string strData)
        {
            string[] strLines = strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < strLines.Length; i++)
            {
                JToken context = JToken.Parse(strLines[i]);
                if (!IsNullOrEmpty(context["fs"]) && !IsNullOrEmpty(context["fs"]["s"]))
                {
                    int freeSpinCount = (int) context["fs"]["s"];
                    if(freeSpinCount > 0)
                    {
                        if (i != strLines.Length - 1)
                            break;
                        return (freeSpinCount - 8) / 2;
                    }
                }
            }
            return -1;
        }
        private double getAverageOddOfCaishenWins(double[] odds)
        {
            double   meanOdd   = odds[odds.Length - 1];
            double[] multiples = new double[] { 20, 18, 16, 14, 12, 10, 8 };
            int      index     = 0;
            for(int i =  odds.Length - 2; i >=0; i--)
            {
                meanOdd = odds[i] * 0.5 + 0.5 * multiples[index + 1] / multiples[index] * meanOdd;
                index++;
            }
            return meanOdd;
        }

        private double getAverageOddOfEgyptBookOfMystery(double[] odds)
        {
            double sumOdd = 0.0;
            for (int i = 0; i < odds.Length; i++)
                sumOdd += odds[i];

            return sumOdd / odds.Length;
        }
        private double getAverageOddOfSecretOfCleopatra(double[] odds)
        {
            double sumOdd = 0.0;
            double[] probs = new double[] { 0.55, 0.7, 0.85 };
            for (int i = odds.Length - 1; i >= 0; i--)
            {
                if (i == odds.Length - 1)
                    sumOdd = odds[i];
                else
                    sumOdd = odds[i] * 0.5 + sumOdd * 0.5 * probs[i];
            }
            return sumOdd;
        }
        
        private int[] getRanges(GameInfo gameInfo, double realOdd, int freeSpinGroup, Dictionary<int, List<double>> dicSpinOddsPerType)
        {
            double[] minRanges = new double[] { 20.0, 10.0, 50.0, 100.0, 300.0, 500.0, 1000.0 };
            double[] maxRanges = new double[] { 50.0, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0 };

            if (gameInfo.SupportPurchaseFree)
            {
                minRanges[0] = 0.0;
                maxRanges[0] = gameInfo.PurchaseMultiple * 0.5;
            }
            int[] spinTypes = getPossibleFreeSpins(gameInfo.Name, freeSpinGroup);
            List<int> ranges = new List<int>();
            for (int i = 0; i < minRanges.Length; i++)
            {
                if (realOdd > maxRanges[i])
                    continue;

                double minOdd = minRanges[i] - realOdd;
                if (minOdd < 0.0)
                    minOdd = 0.0;
                double maxOdd = (maxRanges[i] - realOdd);
                if (gameInfo.Name == "CaishenWins")
                    maxOdd = maxOdd / 2.5;

                bool notFound = false;
                for (int j = 0; j < spinTypes.Length; j++)
                {
                    if (!findInRange(dicSpinOddsPerType[spinTypes[j]], minOdd, maxOdd))
                    {
                        notFound = true;
                        break;
                    }
                }
                if (notFound)
                    continue;
                ranges.Add(i);
            }
            return ranges.ToArray();
        }
        private bool findInRange(List<double> odds, double minOdd, double maxOdd)
        {
            foreach (double odd in odds)
            {
                if (odd >= minOdd && odd <= maxOdd)
                    return true;
            }
            return false;
        }

        private BsonDocument createDocument(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal, int bettype = -1)
        {
            byte    spinType = (byte)(long)reader["spintype"];
            double  odd      = Math.Round((double)reader["odd"], 2);
            string  data     = (string)reader["data"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data }};

            if (bettype >= 0)
                document["bettype"] = bettype;
            if(spinType >= 100 && spinType < 200)
            {
                int freeSpinType = 0;
                if (gameInfo.Name == "CaishenWins")
                {
                    freeSpinType = findFreeSpinTypeForCaishenWins(data);
                    if(freeSpinType != (int)(long)reader["freespintype"])
                    {

                    }
                }
                else
                {
                    freeSpinType = (int)(long)reader["freespintype"];

                }

                document["odd"]          = odd;
                document["freespintype"] = freeSpinType;
                if(gameInfo.Name == "EgyptBookOfMystery" || gameInfo.Name == "QueenOfBounty" || gameInfo.Name == "ShaolinSoccer" || gameInfo.Name == "Genies3Wishes" || gameInfo.Name == "HoneyTrapOfDiaoChan" ||
                    gameInfo.Name == "CaishenWins" || gameInfo.Name == "SecretOfCleopatra")
                    document["ranges"] = new BsonArray(getRanges(gameInfo, odd, freeSpinType, gs.AllFreeSpinOdds));
            }
            if (isNormal)
            {
                if (spinType < 100)
                {
                    gs.NormalOddSum += odd;
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else if (spinType < 200)
                {
                    int      freeSpinType    = (int) document["freespintype"];
                    int[]    freeSpinTypes   = getPossibleFreeSpins(gameInfo.Name, freeSpinType);
                    double[] freeSpinOdds    = new double[freeSpinTypes.Length];
                    for(int i = 0; i < freeSpinTypes.Length; i++)
                        freeSpinOdds[i] = gs.AllFreeSpinAverageOdds[freeSpinTypes[i]];
                    if (gameInfo.Name == "EgyptBookOfMystery" || gameInfo.Name == "QueenOfBounty" || gameInfo.Name == "ShaolinSoccer" || 
                        gameInfo.Name == "Genies3Wishes" || gameInfo.Name == "HoneyTrapOfDiaoChan")
                        gs.NormalOddSum += (odd + getAverageOddOfEgyptBookOfMystery(freeSpinOdds));
                    else if(gameInfo.Name == "SecretOfCleopatra")
                        gs.NormalOddSum += (odd + getAverageOddOfSecretOfCleopatra(freeSpinOdds));
                    else
                        gs.NormalOddSum += (odd + getAverageOddOfCaishenWins(freeSpinOdds));
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else
                {
                    gs.NormalTotalCount++;
                }
            }
            if(gameInfo.SupportPurchaseFree)
            {
                if (spinType == 0 || spinType >= 200)
                    return document;

                if (!gs.PurchaseOdds.ContainsKey(0))
                    gs.PurchaseOdds[0] = new List<double>();

                if (!gs.PurchaseMinOdds.ContainsKey(0))
                    gs.PurchaseMinOdds[0] = new List<double>();

                if(gameInfo.HasPurEnableOption)
                {
                    if (reader["purenabled"] is DBNull)
                        return document;

                    document["puri"] = 0;
                }    
                if (spinType < 100)
                {
                    gs.PurchaseOdds[0].Add(odd);
                }
                else if(spinType == 100)
                {
                    int      freeSpinType  = (int)document["freespintype"];
                    int[]    freeSpinTypes = getPossibleFreeSpins(gameInfo.Name, freeSpinType);
                    double[] freeSpinOdds  = new double[freeSpinTypes.Length];
                    for (int i = 0; i < freeSpinTypes.Length; i++)
                        freeSpinOdds[i] = gs.AllFreeSpinAverageOdds[freeSpinTypes[i]];
                    double allfreewinrate = odd;
                    if (gameInfo.Name == "EgyptBookOfMystery" || gameInfo.Name == "QueenOfBounty" || gameInfo.Name == "ShaolinSoccer" || gameInfo.Name == "Genies3Wishes" || gameInfo.Name == "HoneyTrapOfDiaoChan")
                        allfreewinrate += getAverageOddOfEgyptBookOfMystery(freeSpinOdds);
                    else if (gameInfo.Name == "SecretOfCleopatra")
                        allfreewinrate += getAverageOddOfSecretOfCleopatra(freeSpinOdds);
                    else
                        allfreewinrate += getAverageOddOfCaishenWins(freeSpinOdds);

                    gs.PurchaseOdds[0].Add(allfreewinrate);
                    double minOdd = getMinOdd(gameInfo, odd, freeSpinType, gs.AllFreeSpinOdds);
                    if (minOdd >= 0.0)
                        gs.PurchaseMinOdds[0].Add(minOdd);
                }
            }
            return document;
        }
        private double getMinOdd(GameInfo gameInfo, double realOdd, int freeSpinGroup, Dictionary<int, List<double>> dicSpinOddsPerType)
        {
            double minOdd = 0.0;
            double maxOdd = gameInfo.PurchaseMultiple * 0.5;

            int[] spinTypes = getPossibleFreeSpins(gameInfo.Name, freeSpinGroup);
            if (realOdd > maxOdd)
                return -1.0;

            minOdd = minOdd - realOdd;
            if (minOdd < 0.0)
                minOdd = 0.0;
            maxOdd = maxOdd - realOdd;
            if (gameInfo.Name == "CaishenWins")
                maxOdd = maxOdd / 2.5;
            double averageSum = 0.0;
            for (int j = 0; j < spinTypes.Length; j++)
            {
                double sum = 0.0;
                int count = 0;
                foreach (double odd in dicSpinOddsPerType[spinTypes[j]])
                {
                    if (odd >= minOdd && odd <= maxOdd)
                    {
                        sum += odd;
                        count++;
                    }
                }
                if (count == 0)
                    return -1.0;

                averageSum += (sum / count);
            }
            return realOdd + averageSum / spinTypes.Length;
        }

        public async Task convertTask()
        {
            for (int i = 0; i < _gameInfoList.Count; i++)
            {
                if (_gameInfoList[i].Name == "DragonTigerLuck" ||
                    _gameInfoList[i].Name == "WinWinWon")
                    await convertSqliteDBWithBetTypes(_gameInfoList[i], 3);
                else if (_gameInfoList[i].Name == "Medusa2")
                    await convertSqliteDBWithSpinGroup(_gameInfoList[i]);
                else
                    await convertSqliteDB(_gameInfoList[i]);
            }
            Console.ReadLine();
        }
        private async Task checkDB(GameInfo gameInfo)
        {
            string strFileName = string.Format("F:\\Jobs\\PGSoft\\New\\{0}.db", gameInfo.Name);
            string strConnectionString = @"Data Source=" + strFileName;
            using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
            {
                await connection.OpenAsync();
                string strCommand = "SELECT * FROM spins";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)(long)reader["id"];
                        string   strData  = (string)reader["data"];
                        string[] strLines = strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        int spinType = (int)(long)reader["spintype"];
                        if (spinType >= 100)
                            continue;

                        int prevState  = 1;
                        for(int i = 0; i < strLines.Length; i++)
                        {
                            var context   = JToken.Parse(strLines[i]);
                            int state     = (int)context["st"];
                            int nextState = (int)context["nst"];
                            if (prevState != state)
                            {

                            }
                            prevState = nextState;
                        }
                        if(prevState != 1)
                        {

                        }
                    }
                }

            }
        }
        public async Task correctMedusa2DB()
        {
            string strFileName = string.Format("F:\\Jobs\\PGSoft\\Slotdatabase\\Medusa2.db");
            string strConnectionString = @"Data Source=" + strFileName;
            using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
            {
                await connection.OpenAsync();

                Dictionary<int, int>    dicUpdateSpinCounts = new Dictionary<int, int>();
                Dictionary<int, string> dicUpdateDatas      = new Dictionary<int, string>();
                Dictionary<int, double> dicUpdateOdds       = new Dictionary<int, double>();

                string strCommand = "SELECT * FROM spins";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int         id          = (int)(long)reader["id"];
                        string      strData     = (string)reader["data"];
                        double      odd         = (double)reader["odd"];
                        string[]    strGroups   = strData.Split(new string[] { "####" }, StringSplitOptions.RemoveEmptyEntries);

                        List<string> newGroups   = new List<string>();
                        double       totalWin    = 0.0;
                        bool         isFirstLine = true;
                        for(int i = 0; i < strGroups.Length; i++)
                        {
                            string[] strLines = strGroups[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);


                            List<string> newLines = new List<string>();
                            List<string> lines    = new List<string>();
                            for (int j = 0; j < strLines.Length; j++)
                            {
                                var context     = JToken.Parse(strLines[j]);
                                int state       = (int)context["st"];
                                int nextState   = (int)context["nst"];
                                int ms          = (int)context["ms"];
                                lines.Add(strLines[j]);                                
                                if (nextState == 1)
                                {
                                    newLines.Add(string.Join("\r\n", lines.ToArray()));
                                    lines.Clear();
                                    totalWin += (double)context["aw"];
                                }
                            }
                            newGroups.AddRange(newLines);
                        }
                        double newOdd = Math.Round(totalWin / (newGroups.Count * 1.5), 2);

                        if (newGroups.Count == strGroups.Length)
                            continue;


                        dicUpdateSpinCounts.Add(id, newGroups.Count);
                        dicUpdateDatas.Add(id, string.Join("####", newGroups.ToArray()));
                        dicUpdateOdds.Add(id, newOdd);
                    }
                }

                strCommand = "UPDATE spins SET spincount=@spincount, odd=@odd, data=@data WHERE id=@id";
                var transaction = connection.BeginTransaction();
                
                foreach(KeyValuePair<int, int> pair in dicUpdateSpinCounts)
                {
                    command = new SQLiteCommand(strCommand, connection, transaction);
                    command.Parameters.AddWithValue("@id", pair.Key);
                    command.Parameters.AddWithValue("@spincount", pair.Value);
                    command.Parameters.AddWithValue("@odd",     dicUpdateOdds[pair.Key]);
                    command.Parameters.AddWithValue("@data", dicUpdateDatas[pair.Key]);
                    await command.ExecuteNonQueryAsync();

                }
                transaction.Commit();
            }
        }

        public async Task<bool> convertSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("Slotdatabase\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                var infoDocument = new BsonDocument();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int normalMaxID = 0;
                    double defaultBet = 0.0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet  = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }
                    infoDocument["defaultbet"] = Math.Round(defaultBet, 2);
                    double[] averageFreeSpinOddsInNormal = null;
                    Dictionary<int, List<double>> allFreeSpinOdds           = new Dictionary<int, List<double>>();
                    Dictionary<int, double>       allFreeSpinAverageOdds    = new Dictionary<int, double>();
                    if (gameInfo.SelFreeSpin)
                    {
                        int freeSpinTypeCount = 1;
                        Dictionary<int, double> dicSumOdds      = new Dictionary<int, double>();
                        Dictionary<int, int>    dicSpinCounts   = new Dictionary<int, int>();

                        strCommand = "SELECT * FROM spins WHERE spintype >= 200 ORDER BY id";
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                int     id          = (int)(long)reader["id"];
                                int     spinType    = (int)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);

                                if (id > normalMaxID && averageFreeSpinOddsInNormal == null)
                                {                                    
                                    averageFreeSpinOddsInNormal = new double[freeSpinTypeCount];
                                    for (int i = 0; i < freeSpinTypeCount; i++)
                                    {
                                        double  sum      = 0.0;
                                        int     freeType = 200 + i;
                                        if(allFreeSpinOdds.ContainsKey(freeType))
                                        {
                                            foreach (double freeOdd in allFreeSpinOdds[freeType])
                                                sum += freeOdd;

                                            averageFreeSpinOddsInNormal[i] = sum / allFreeSpinOdds[freeType].Count;
                                        }
                                        else
                                        {
                                            averageFreeSpinOddsInNormal[i] = 0.0;
                                        }

                                    }
                                }
                                if (allFreeSpinOdds.ContainsKey(spinType))
                                {
                                    allFreeSpinAverageOdds[spinType] += odd;
                                    allFreeSpinOdds[spinType].Add(odd);
                                }
                                else
                                {
                                    allFreeSpinOdds.Add(spinType, new List<double>(new double[] { odd }));
                                    allFreeSpinAverageOdds[spinType] = odd;
                                }                                
                            }
                        }

                        foreach(KeyValuePair<int, List<double>> pair in allFreeSpinOdds)
                            allFreeSpinAverageOdds[pair.Key] = allFreeSpinAverageOdds[pair.Key] / pair.Value.Count;

                        if(averageFreeSpinOddsInNormal == null)
                        {
                            averageFreeSpinOddsInNormal = new double[freeSpinTypeCount];
                            for (int i = 0; i < freeSpinTypeCount; i++)
                            {
                                double sum = 0.0;
                                int freeType = 200 + i;
                                if (allFreeSpinOdds.ContainsKey(freeType))
                                {
                                    foreach (double freeOdd in allFreeSpinOdds[freeType])
                                        sum += freeOdd;

                                    averageFreeSpinOddsInNormal[i] = sum / allFreeSpinOdds[freeType].Count;
                                }
                                else
                                {
                                    averageFreeSpinOddsInNormal[i] = 0.0;
                                }
                            }
                        }
                    }

                    gameStatus.FreeSpinOddsInNormal     = averageFreeSpinOddsInNormal;
                    gameStatus.AllFreeSpinOdds          = allFreeSpinOdds;
                    gameStatus.AllFreeSpinAverageOdds   = allFreeSpinAverageOdds;
                    gameStatus.NormalOddSum             = 0.0;
                    gameStatus.PurchaseOdds             = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds          = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount        = 0;
                    gameStatus.NormalTotalCount         = 0;
                    gameStatus.DefaultBet               = defaultBet;

                    int idCounter = 1;
                    strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     id       = (int)(long)reader["id"];
                            byte    spinType = (byte)(long)reader["spintype"];
                            double  odd      = Math.Round((double)reader["odd"], 2);

                            if (odd == 0.0 && spinType == 0)
                            {
                                documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }

                    infoDocument["emptycount"] = idCounter - 1;
                    strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte spinType = (byte)(long)reader["spintype"];
                            double odd = Math.Round((double)reader["odd"], 2);

                            if((spinType == 0 && odd > 0.0) || (spinType > 0 && spinType < 200))
                            {
                                documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }
                    if(gameInfo.SelFreeSpin)
                    {
                        strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte spinType = (byte)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);

                                if (spinType >= 200)
                                {
                                    documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                    }

                    infoDocument["normalselectcount"]   = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid"]         = documents.Count;

                    double payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                    infoDocument["normalrtp"] = Math.Round(payoutRate, 4);
                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command     = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }

                    if (gameInfo.SupportPurchaseFree)
                    {
                        List<double> purchaseOdds   = new List<double>();
                        double       multiple       = gameInfo.PurchaseMultiple;

                        double  sum         = 0.0;
                        double  minSum      = 0.0;
                        int     minCount    = 0;
                        foreach (double odd in gameStatus.PurchaseOdds[0])
                        {
                            sum += odd;
                            if(odd >= 0.0 && odd <= multiple * 0.5)
                            {
                                minCount++;
                                minSum += odd;
                            }
                        }

                        double average  = sum / gameStatus.PurchaseOdds[0].Count;
                        if(average < multiple)
                            return false;

                        double minAverage = minSum / minCount;
                        if (gameInfo.SelFreeSpin)
                        {
                            minSum = 0.0;
                            minCount = gameStatus.PurchaseMinOdds[0].Count;
                            foreach (double odd in gameStatus.PurchaseMinOdds[0])
                                minSum += odd;
                            minAverage = minSum / minCount;
                        }
                        if (minCount <= 50 || average < minAverage)
                            return false;

                        purchaseOdds.Add(minAverage);
                        purchaseOdds.Add(average);
                        infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                    }
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);

                    Console.WriteLine("{0} completed!", gameInfo.Name);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertSqliteDBWithSpinGroup(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("F:\\Jobs\\PGSoft\\Slotdatabase\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                var infoDocument = new BsonDocument();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int normalMaxID = 0;
                    double defaultBet = 0.0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet  = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }
                    infoDocument["defaultbet"]   = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum      = 0.0;
                    gameStatus.PurchaseOdds      = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds   = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount = 0;
                    gameStatus.NormalTotalCount  = 0;
                    gameStatus.DefaultBet        = defaultBet;

                    int idCounter = 1;
                    strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    int     minTotalCount   = 0;
                    double  minTotalOdd     = 0.0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     spinCount   = (int)(long)reader["spincount"];
                            double  odd         = Math.Round((double)reader["odd"], 2);
                            if (odd <= 0.5)
                            {
                                var document = new BsonDocument() {
                                    {"_id",         idCounter },
                                    {"spincount",   spinCount},
                                    {"odd",         odd },
                                    {"data",        (string) reader["data"] }};

                                documents.Add(document);
                                minTotalCount   += spinCount;
                                minTotalOdd     += spinCount * odd;
                                idCounter++;
                            }
                        }
                    }
                    int     totalCount  = minTotalCount;
                    double  totalOdd    = minTotalOdd;
                    infoDocument["mincount"] = idCounter - 1;
                    strCommand               = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     spinCount = (int) (long)reader["spincount"];
                            double  odd       = Math.Round((double)reader["odd"], 2);

                            if (odd > 0.5)
                            {
                                var document = new BsonDocument() {
                                    {"_id",         idCounter },
                                    {"spincount",   spinCount},
                                    {"odd",         odd },
                                    {"data",        (string) reader["data"] }};

                                documents.Add(document);
                                totalCount  += spinCount;
                                totalOdd    += spinCount * odd;
                                idCounter++;
                            }
                        }
                    }
                    infoDocument["normalselectcount"]   = idCounter - 1;
                    infoDocument["normalmaxid"]         = documents.Count;
                    double  normalPayoutRate            = totalOdd / totalCount;
                    double  minPayoutRate               = minTotalOdd / minTotalCount;

                    double[] payoutRates = new double[2];
                    payoutRates[0]       = normalPayoutRate;
                    payoutRates[1]       = minPayoutRate;
                    infoDocument.Add("rtps", new BsonArray(payoutRates));
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> convertSqliteDBWithBetTypes(GameInfo gameInfo, int typeCount)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("F:\\Jobs\\PGSoft\\Slotdatabase\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                var infoDocument = new BsonDocument();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    List<int>       normalMaxIDs = new List<int>();
                    List<double>    defaultBets  = new List<double>();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            for(int i = 0; i < typeCount; i++)
                            {
                                int     normalMaxID = (int)(long)reader[string.Format("normalmaxid{0}", i+1)];
                                double  defaultBet  = Math.Round((double)reader[string.Format("defaultbet{0}", i+1)], 2);
                                normalMaxIDs.Add(normalMaxID);
                                defaultBets.Add(defaultBet);
                            }
                        }
                    }

                    infoDocument.Add("defaultbets", new BsonArray(defaultBets.ToArray()));
                    double[] averageFreeSpinOddsInNormal = null;
                    Dictionary<int, List<double>> allFreeSpinOdds = new Dictionary<int, List<double>>();
                    Dictionary<int, double> allFreeSpinAverageOdds = new Dictionary<int, double>();

                    gameStatus.FreeSpinOddsInNormal = averageFreeSpinOddsInNormal;
                    gameStatus.AllFreeSpinOdds = allFreeSpinOdds;
                    gameStatus.AllFreeSpinAverageOdds = allFreeSpinAverageOdds;
                    gameStatus.PurchaseOdds = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds = new Dictionary<int, List<double>>();

                    int idCounter = 1;
                    List<int>       typeStartIds      = new List<int>();
                    List<int>       typeEmptyCounts   = new List<int>();
                    List<int>       typeNormalCounts  = new List<int>();
                    List<int>       typeNormalMaxIds  = new List<int>();
                    List<double>    typeNormalRTPS    = new List<double>();

                    for (int i = 0; i < typeCount; i++)
                    {
                        gameStatus.DefaultBet = defaultBets[i];
                        typeStartIds.Add(idCounter);
                        gameStatus.NormalSelectCount = 0;
                        gameStatus.NormalTotalCount = 0;
                        gameStatus.NormalOddSum = 0.0;

                        int emptyCount = 0;
                        strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}" , normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int     id          = (int)(long)reader["id"];
                                byte    spinType    = (byte)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);
                                if (odd == 0.0 && spinType == 0)
                                {
                                    documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, true, i));
                                    idCounter++;
                                    emptyCount++;
                                }
                            }
                        }
                        typeEmptyCounts.Add(emptyCount);
                        strCommand = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}", normalMaxIDs[i], i);
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte    spinType    = (byte)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);
                                if ((spinType == 0 && odd > 0.0) || (spinType > 0 && spinType < 200))
                                {
                                    documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, true, i));
                                    idCounter++;
                                }
                            }
                        }
                        typeNormalCounts.Add(gameStatus.NormalSelectCount);
                        typeNormalMaxIds.Add(documents.Count);

                        double payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                        typeNormalRTPS.Add(Math.Round(payoutRate, 4));
                        strCommand  = string.Format("SELECT * FROM spins WHERE id > {0} and bettype={1}", normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                documents.Add(createDocument(gameInfo, reader, idCounter, gameStatus, false, i));
                                idCounter++;
                            }
                        }
                       
                    }
                    
                    infoDocument.Add("startids",            new BsonArray(typeStartIds.ToArray()));
                    infoDocument.Add("emptycounts",         new BsonArray(typeEmptyCounts.ToArray()));
                    infoDocument.Add("normalselectcounts",  new BsonArray(typeNormalCounts.ToArray()));
                    infoDocument.Add("normalmaxids",        new BsonArray(typeNormalMaxIds.ToArray()));
                    infoDocument.Add("normalrtps",          new BsonArray(typeNormalRTPS.ToArray()));
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private int[] getPossibleFreeSpins(string strGameName, int freeSpinGroup)
        {
            switch (strGameName)
            {
                case "CaishenWins":
                    {
                        List<int> freeSpinTypes = new List<int>();
                        for (int i = freeSpinGroup; i < 7; i++)
                            freeSpinTypes.Add(200 + i);

                        return freeSpinTypes.ToArray();
                    }
                case "EgyptBookOfMystery":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202, 203 };
                        else if (freeSpinGroup == 1)
                            return new int[] { 204, 205, 206, 207 };
                        else
                            return new int[] { 208, 209, 210, 211 };
                    }
                case "QueenOfBounty":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202 };
                        else if(freeSpinGroup == 1)
                            return new int[] { 203, 204, 205 };
                        else
                            return new int[] { 206, 207, 208 };
                    }
                case "ShaolinSoccer":
                    {
                        return new int[] { 200, 201, 202, 203, 204 };
                    }
                case "SecretOfCleopatra":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202, 203 };
                        else if (freeSpinGroup == 1)
                            return new int[] { 204, 205, 206, 207 };
                        else
                            return new int[] { 208, 209, 210, 211 };
                    }
                case "Genies3Wishes":
                    {
                        return new int[] { 200, 201, 202 };
                    }
                case "HoneyTrapOfDiaoChan":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202 };
                        else if (freeSpinGroup == 1)
                            return new int[] { 203, 204, 205 };
                        else
                            return new int[] { 206, 207, 208 };
                    }
            }
            return new int[0];
        }

    }

    public class GameInternalStatus
    {
        public double[]                         FreeSpinOddsInNormal    { get; set; }
        public Dictionary<int, List<double>>    AllFreeSpinOdds         { get; set; }
        public Dictionary<int, double>          AllFreeSpinAverageOdds  { get; set; }

        public Dictionary<int, List<double>>    PurchaseOdds            { get; set; }
        public Dictionary<int, List<double>>    PurchaseMinOdds         { get; set; }

        public double                           DefaultBet              { get; set; }
        public double                           NormalOddSum            { get; set; }
        public int                              NormalSelectCount       { get; set; }
        public int                              NormalTotalCount        { get; set; }        
        public int                              TotalSpinCount          { get; set; }
    }
    public class GameInfo
    {
        public string   Name                { get; set; }
        public bool     SupportPurchaseFree { get; set; }
        public bool     HasPurEnableOption  { get; set; }
        public bool     SelFreeSpin         { get; set; }
        public double   PurchaseMultiple    { get; set; }
        public GameInfo(string strName, bool supportPurchaseFree, bool hasPurEnabledOption, bool selFreeSpin, double purchaseMultiple = 0.0) 
        {
            this.Name                   = strName;
            this.SupportPurchaseFree    = supportPurchaseFree;
            this.HasPurEnableOption     = hasPurEnabledOption;
            this.SelFreeSpin            = selFreeSpin;
            this.PurchaseMultiple       = purchaseMultiple;
        }

    }
}
