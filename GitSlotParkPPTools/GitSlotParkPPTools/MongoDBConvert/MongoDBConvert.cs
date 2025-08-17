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
using Amazon.Runtime.Internal;
using System.Net.NetworkInformation;

namespace MongoDBConvert
{
    public class MongoDBConvert
    {
        private static MongoDBConvert   _sInstance  = new MongoDBConvert();
        public static MongoDBConvert    Instance    => _sInstance;

        private List<GameInfo> _gameInfoList = new List<GameInfo>();
        private string _strDBName = "ppspindb";

        public MongoDBConvert()
        {

            GameInfo[] gameInfos = new GameInfo[]
            {



                #region PP New
                new GameInfo("BigBassBoxingBonusRound", true,   true,   false, 0),
                new GameInfo("TempleGuardians",         true,   true,   false, 70),
                new GameInfo("FingerLicknFreeSpins",    true,   true,   false, 100),
                new GameInfo("PigFarm",                 false,  false,  false, 0),
                new GameInfo("GoldParty2AfterHours",    true,   true,   false, 100),
                new GameInfo("GemFireFortune",          true,   true,   false, 0),
                new GameInfo("WavesOfPoseidon",         true,   true,   false, 0),
                new GameInfo("GatesOfHades",            true,   true,   false, 0),
                new GameInfo("ClubTropicanaHappyHour",  true,   true,   false, 0),
                new GameInfo("FortuneOfAztec",          false,  false,  false, 0),
                new GameInfo("AlienInvaders",           true,   true,   false, 0),
                new GameInfo("OlympusWinsSuperScatter", false,  false,  false, 0),
                new GameInfo("MummysJewels",            true,   true,   false, 0),
                new GameInfo("SweetBonanzaSuperScatter",true,   true,   false, 0),
                new GameInfo("ChilliHeatSpicySpins",    true,   true,   false, 100),
                new GameInfo("MasterGems",              false,  false,  false, 0),
                new GameInfo("YouCanPiggyBankOnIt",     true,   true,   false, 100),
                new GameInfo("GemTrio",                 true,   true,   false, 0),
                new GameInfo("ZombieSchoolMega",        true,   true,   false, 0),
                new GameInfo("Argonauts",               true,   true,   false, 60),
                new GameInfo("BigBassReelRepeat",       true,   true,   false, 0),

                #endregion
            };
            _gameInfoList.AddRange(gameInfos);
        }

        public async Task checkAndCreateIndics()
        {
            for (int i = 0; i < _gameInfoList.Count; i++)
                await checkAndCreateIndex(_gameInfoList[i]);
        }
        private async Task checkAndCreateIndex(GameInfo gameInfo)
        {
            try
            {
                if (!gameInfo.SupportPurchaseFree && !gameInfo.SelFreeSpin)
                    return;

                var dbClient = new MongoClient("mongodb://127.0.0.1");
                IMongoDatabase db = dbClient.GetDatabase(_strDBName);
                var collection = db.GetCollection<BsonDocument>(gameInfo.Name);


                var indices = collection.Indexes.List().ToList();
                List<string> indexIds = new List<string>();
                for(int i = 0; i < indices.Count; i++)
                {
                    var index       = indices[i];
                    var indexKeys   = index["key"];
                    List<string> indexKeyNames = new List<string>();
                    foreach(var keyName in indexKeys.AsBsonDocument)
                    {
                        indexKeyNames.Add(keyName.Name);
                    }
                    string strIndexID = string.Join("_", indexKeyNames.ToArray());
                    indexIds.Add(strIndexID);
                }
                if(gameInfo.SelFreeSpin)
                {
                    if (gameInfo.HasPurEnableOption)
                    {
                        if (!indexIds.Contains("puri"))
                            await collection.Indexes.CreateOneAsync("{ puri : 1 }");
                        if (!indexIds.Contains("puri_ranges"))
                            await collection.Indexes.CreateOneAsync("{ puri : 1, ranges: 1 }");
                    }
                    else
                    {
                        if (!indexIds.Contains("spintype_ranges"))
                            await collection.Indexes.CreateOneAsync("{ spintype : 1 , ranges: 1}");
                    }
                    if (!indexIds.Contains("spintype"))
                        await collection.Indexes.CreateOneAsync("{ spintype : 1 }");

                    if (!indexIds.Contains("spintype_odd"))
                        await collection.Indexes.CreateOneAsync("{ spintype : 1, odd:1 }");

                }
                else if(gameInfo.SupportPurchaseFree)
                {
                    if(gameInfo.HasPurEnableOption)
                    {
                        if(!indexIds.Contains("puri"))
                            await collection.Indexes.CreateOneAsync("{ puri : 1 }");

                        if (!indexIds.Contains("puri_odd"))
                            await collection.Indexes.CreateOneAsync("{ puri : 1, odd:1 }");
                    }
                    else
                    {
                        if (!indexIds.Contains("spintype"))
                            await collection.Indexes.CreateOneAsync("{ spintype : 1 }");

                        if (!indexIds.Contains("spintype_odd"))
                            await collection.Indexes.CreateOneAsync("{ spintype : 1, odd:1}");
                    }
                }

                Console.WriteLine(string.Format("{0} created indics", gameInfo.Name));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task writePurSamplesDataToMongoDB(GameInfo gameInfo, List<BsonDocument> documents)
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1");
            IMongoDatabase db = dbClient.GetDatabase(_strDBName);
            var collection = db.GetCollection<BsonDocument>(gameInfo.Name + "_pursamples");
            await collection.InsertManyAsync(documents);
        }
        private async Task writeGambleOddsDataToMongoDB(GameInfo gameInfo, List<BsonDocument> documents)
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1");
            IMongoDatabase db = dbClient.GetDatabase(_strDBName);
            var collection = db.GetCollection<BsonDocument>(gameInfo.Name + "_gambleodds");
            await collection.InsertManyAsync(documents);
        }

        private async Task writeInfoDataToMongoDB(GameInfo gameInfo, BsonDocument infoDocument, List<BsonDocument> documents, List<BsonDocument> sequences = null)
        {
            try
            {
                infoDocument["name"] = gameInfo.Name;
                var             dbClient        = new MongoClient("mongodb://127.0.0.1");
                IMongoDatabase  db              = dbClient.GetDatabase(_strDBName);
                var             gameInfoTable   = db.GetCollection<BsonDocument>("infos");
                
                await gameInfoTable.FindOneAndDeleteAsync(_ => _["name"] == gameInfo.Name);
                await db.DropCollectionAsync(gameInfo.Name);

                await gameInfoTable.InsertOneAsync(infoDocument);
                var collection = db.GetCollection<BsonDocument>(gameInfo.Name);                
                await collection.InsertManyAsync(documents);

                if (gameInfo.SelFreeSpin)
                {
                    if (gameInfo.HasPurEnableOption)
                    {
                        await collection.Indexes.CreateOneAsync("{ puri : 1 }");
                        await collection.Indexes.CreateOneAsync("{ puri : 1, ranges: 1 }");
                    }
                    else
                    {
                        await collection.Indexes.CreateOneAsync("{ spintype : 1 , ranges: 1}");
                    }
                    await collection.Indexes.CreateOneAsync("{ spintype : 1 }");
                    await collection.Indexes.CreateOneAsync("{ spintype : 1, odd:1 }");
                }
                else if (gameInfo.SupportPurchaseFree)
                {
                    if (gameInfo.HasPurEnableOption)
                    {
                        await collection.Indexes.CreateOneAsync("{ puri : 1 }");
                        await collection.Indexes.CreateOneAsync("{ puri : 1, odd:1 }");
                    }
                    else
                    {
                        await collection.Indexes.CreateOneAsync("{ spintype : 1 }");
                        await collection.Indexes.CreateOneAsync("{ spintype : 1, odd:1}");
                    }
                }
                if (sequences != null)
                {
                    collection = db.GetCollection<BsonDocument>(gameInfo.Name + "_sequences");
                    await collection.InsertManyAsync(sequences);
                }

                Console.WriteLine(string.Format("Updated Database of {0}", gameInfo.Name));
            }
            catch
            {

            }
        }
        private BsonDocument createDocument(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte    spinType = (byte)(long)reader["spintype"];
            double  odd      = Math.Round((double)reader["odd"], 2);
            string  data     = (string)reader["data"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data }};

            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            int puri = -1;
            if (columns.Contains("purenabled") && !(reader["purenabled"] is DBNull) && ((int)(long)reader["purenabled"] == 1))
                puri = 0;
            else if (columns.Contains("puri") && !(reader["puri"] is DBNull))
                puri = (int)(long)reader["puri"];

            if (gameInfo.Name == "Thor2" && spinType == 2)
                puri = 0;

            if (puri >= 0)
                document.Add("puri", puri);

            if(spinType >= 100 && spinType < 200)
            {
                int freeSpinType         = (int)(long)reader["freespintype"];
                odd                      = (double)reader["realodd"];
                document["odd"]          = odd;
                document["freespintype"] = freeSpinType;
                document["ranges"]       = new BsonArray(getRanges(gameInfo, odd, freeSpinType, gs.AllFreeSpinOdds));

            }
            if(gameInfo.Name == "PowerOfThorMega")
            {

            }
            if(isNormal)
            {
                if (spinType < 100)
                {
                    gs.NormalOddSum += odd;
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else if (spinType < 200)
                {
                    int     freeSpinType    = (int)(long)reader["freespintype"];
                    int[]   freeSpinTypes   = getPossibleFreeSpins(gameInfo.Name, freeSpinType);
                    double sum = 0.0;
                    for(int i = 0; i < freeSpinTypes.Length; i++)
                    {
                        if(gs.FreeSpinOddsInNormal[freeSpinTypes[i] - 200] == 0.0)
                        {

                        }
                        sum += gs.FreeSpinOddsInNormal[freeSpinTypes[i] - 200];
                    }

                    gs.NormalOddSum += (odd + sum / freeSpinTypes.Length);
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

                if (gameInfo.HasPurEnableOption && puri == -1)
                    return document;

                if(getMultiPurchaseCount(gameInfo.Name) > 1)
                {
                    if (puri < 0)
                        puri = (int) (long) reader["freespintype"];
                    document["puri"] = puri;
                }
                else
                {
                    if (puri < 0)
                        puri = 0;
                }
                if (!gs.PurchaseOdds.ContainsKey(puri))
                    gs.PurchaseOdds[puri] = new List<double>();

                if (spinType < 100)
                {
                    gs.PurchaseOdds[puri].Add(odd);
                }
                else if(spinType < 200)
                {
                    int     freeSpinGroup  = (int)(long)reader["freespintype"];
                    int[]   freeSpinTypes  = getPossibleFreeSpins(gameInfo.Name, freeSpinGroup);
                    double sumOdd = 0.0;
                    for (int i = 0; i < freeSpinTypes.Length; i++)
                        sumOdd += gs.AllFreeSpinAverageOdds[freeSpinTypes[i]];

                    gs.PurchaseOdds[puri].Add(odd + sumOdd / freeSpinTypes.Length);

                    double minOdd = getMinOdd(gameInfo, odd, freeSpinGroup, gs.AllFreeSpinOdds);
                    if(minOdd >= 0.0)
                    {
                        double oldMinOdd = (double)reader["minrate"];
                        if (gs.PurchaseMinOdds.ContainsKey(puri))
                            gs.PurchaseMinOdds[puri].Add(minOdd);
                        else
                            gs.PurchaseMinOdds[puri] = new List<double>(new double[] { minOdd});
                        
                    }
                }
            }
            return document;
        }
        private BsonDocument createDocumentNew(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            if (gameInfo.Name == "BookOfTheFallen")
                return createDocumentNewForBookOfTheFallen(gameInfo, reader, id, gs, isNormal);

            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string)reader["data"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data }};

            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            if (columns.Contains("bettype"))
                document["bettype"] = (int) (long) reader["bettype"];

            int puri = -1;
            if (columns.Contains("purenabled") && !(reader["purenabled"] is DBNull) && ((int)(long)reader["purenabled"] == 1))
                puri = 0;
            else if (columns.Contains("puri") && !(reader["puri"] is DBNull))
                puri = (int)(long)reader["puri"];

            if (puri >= 0)
                document.Add("puri", puri);

            if (spinType >= 100 && spinType < 200)
            {
                odd                         = (double)reader["realodd"];
                document["odd"]             = odd;
                if (gameInfo.Name == "ReleaseTheKraken2")
                {
                    string freeSpinTypes = (string)reader["freespintypes"];
                    document["freespintypes"] = freeSpinTypes;
                }
                else
                {
                    int freeSpinType = (int)(long)reader["freespintype"];
                    document["freespintype"] = freeSpinType;
                }

                string strRanges            = (string)reader["ranges"];
                List<int> rangeIds          = new List<int>();
                string [] strParts          = strRanges.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    rangeIds.Add(int.Parse(strParts[i]));
                document["ranges"] = new BsonArray(rangeIds.ToArray());
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
                    double allFreeWinRate    = (double) reader["allfreewinrate"];
                    gs.NormalOddSum         += allFreeWinRate;
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else
                {
                    gs.NormalTotalCount++;
                }
            }
            if (gameInfo.SupportPurchaseFree)
            {
                if (spinType == 0 || spinType >= 200)
                    return document;

                if (gameInfo.HasPurEnableOption && puri == -1)
                    return document;

                if (!gameInfo.HasPurEnableOption && puri == -1)
                    puri = 0;

                if (!gs.PurchaseOdds.ContainsKey(puri))
                    gs.PurchaseOdds[puri] = new List<double>();

                if (spinType < 100)
                {
                    gs.PurchaseOdds[puri].Add(odd);
                }
                else if (spinType < 200)
                {
                    double allFreeWinRate = (double)reader["allfreewinrate"];
                    gs.PurchaseOdds[puri].Add(allFreeWinRate);

                    if (!(reader["minrate"] is DBNull))
                    {
                        double minRate = (double) reader["minrate"];
                        if (gs.PurchaseMinOdds.ContainsKey(puri))
                            gs.PurchaseMinOdds[puri].Add(minRate);
                        else
                            gs.PurchaseMinOdds[puri] = new List<double>(new double[] { minRate });
                    }
                }
            }
            return document;
        }
        private BsonDocument createDocumentClassic(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {

            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string)reader["data"];
            int     betType     = (int) (long)reader["bettype"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"bettype",     betType },
                                    {"data",        data }
            };

            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            int puri = -1;
            if (columns.Contains("purenabled") && !(reader["purenabled"] is DBNull) && ((int)(long)reader["purenabled"] == 1))
                puri = 0;
            else if (columns.Contains("puri") && !(reader["puri"] is DBNull))
                puri = (int)(long)reader["puri"];

            if (puri >= 0)
                document.Add("puri", puri);

            if (isNormal)
            {
                gs.NormalOddSum += odd;
                gs.NormalSelectCount++;
                gs.NormalTotalCount++;
            }

            if (gameInfo.SupportPurchaseFree)
            {
                if (spinType == 0 || spinType >= 200)
                    return document;

                if (gameInfo.HasPurEnableOption && puri == -1)
                    return document;

                if (!gameInfo.HasPurEnableOption && puri == -1)
                    puri = 0;

                if (!gs.PurchaseOdds.ContainsKey(puri))
                    gs.PurchaseOdds[puri] = new List<double>();

                if (spinType < 100)
                {
                    gs.PurchaseOdds[puri].Add(odd);
                }
                else if (spinType < 200)
                {
                    double allFreeWinRate = (double)reader["allfreewinrate"];
                    gs.PurchaseOdds[puri].Add(allFreeWinRate);

                    if (!(reader["minrate"] is DBNull))
                    {
                        double minRate = (double) reader["minrate"];
                        if (gs.PurchaseMinOdds.ContainsKey(puri))
                            gs.PurchaseMinOdds[puri].Add(minRate);
                        else
                            gs.PurchaseMinOdds[puri] = new List<double>(new double[] { minRate });
                    }
                }
            }
            return document;
        }
        private BsonDocument createDocumentNewForBookOfTheFallen(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte spinType = (byte)(long)reader["spintype"];
            double odd = Math.Round((double)reader["odd"], 2);
            string data = (string)reader["data"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data }};

            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                columns.Add(reader.GetName(i));

            if (columns.Contains("bettype"))
                document["bettype"] = (int)(long)reader["bettype"];

            int puri = -1;
            if (columns.Contains("purenabled") && !(reader["purenabled"] is DBNull) && ((int)(long)reader["purenabled"] == 1))
                puri = 0;
            else if (columns.Contains("puri") && !(reader["puri"] is DBNull))
                puri = (int)(long)reader["puri"];

            if (puri >= 0)
                document.Add("puri", puri);

            if (spinType >= 100 && spinType < 200)
            {
                int freeSpinType = (int)(long)reader["freespintype"];
                odd = (double)reader["realodd"];
                document["odd"] = odd;
                document["freespintype"] = freeSpinType;

                string strRanges = (string)reader["ranges"];
                List<int> rangeIds = new List<int>();
                string[] strParts = strRanges.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    rangeIds.Add(int.Parse(strParts[i]));
                document["ranges"] = new BsonArray(rangeIds.ToArray());
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
                    double allFreeWinRate = (double)reader["allfreewinrate"];
                    gs.NormalOddSum += allFreeWinRate;
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else
                {
                    gs.NormalTotalCount++;
                }
            }
            if (gameInfo.SupportPurchaseFree)
            {
                if (spinType >= 200)
                    return document;
                if (puri < 0)
                    return document;

                if (!gs.PurchaseOdds.ContainsKey(puri))
                    gs.PurchaseOdds[puri] = new List<double>();

                if (spinType < 100)
                {
                    gs.PurchaseOdds[puri].Add(odd);
                    double purchaseMultiple = getMultiPurMultiple(gameInfo, puri);
                    if(odd >= purchaseMultiple * 0.2 && odd <= purchaseMultiple * 0.5)
                    {
                        if (gs.PurchaseMinOdds.ContainsKey(puri))
                            gs.PurchaseMinOdds[puri].Add(odd);
                        else
                            gs.PurchaseMinOdds[puri] = new List<double>(new double[] { odd });
                    }
                }
                else if (spinType < 200)
                {
                    double allFreeWinRate = (double)reader["allfreewinrate"];
                    gs.PurchaseOdds[puri].Add(allFreeWinRate);

                    if (!(reader["minrate"] is DBNull))
                    {
                        double minRate = (double)reader["minrate"];
                        if (gs.PurchaseMinOdds.ContainsKey(puri))
                            gs.PurchaseMinOdds[puri].Add(minRate);
                        else
                            gs.PurchaseMinOdds[puri] = new List<double>(new double[] { minRate });
                    }
                }
            }
            return document;
        }

        private BsonDocument createDocumentForPinupGirl(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string)reader["data"];
            string  spinTypes   = (string)reader["spintypes"];
            string  spinOdds    = (string)reader["odds"];
            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data },
                                    {"spintypes",   spinTypes },
                                    {"odds",        spinOdds }
            };
            gs.NormalOddSum += odd;
            gs.NormalSelectCount++;
            gs.NormalTotalCount++;
            return document;
        }
        private BsonDocument createDocumentForGroupSpin(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string)reader["data"];
            int     count       = (int)(long)reader["count"];
            string  spinOdds    = (string)reader["odds"];
            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"data",        data },
                                    {"count",       count },
                                    {"odds",        spinOdds }
            };
            gs.NormalOddSum += (odd * count);
            gs.NormalSelectCount++;
            gs.NormalTotalCount++;
            return document;
        }

        private BsonDocument createDocumentForCashElevator(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string)reader["data"];
            int     beginFloor  = (int) (long)reader["beginfloor"];
            int     endFloor    = (int) (long)reader["endfloor"];

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"beginfloor",  beginFloor},
                                    {"endfloor",    endFloor},
                                    {"data",        data }};

            if (isNormal)
            {
                gs.NormalOddSum += odd;
                gs.NormalSelectCount++;
                gs.NormalTotalCount++;
            }
            return document;
        }
        private BsonDocument createDocumentForBroncoSpirit(GameInfo gameInfo, DbDataReader reader, int id, GameInternalStatus gs, bool isNormal)
        {
            byte    spinType    = (byte)(long)reader["spintype"];
            double  odd         = Math.Round((double)reader["odd"], 2);
            string  data        = (string) reader["data"];
            int     groupid     = (int)(long)reader["groupid"];
            int     islast      = (int)(long)reader["islast"];

            if (islast == 1)
                groupid += 100;

            var document = new BsonDocument() {
                                    {"_id",         id },
                                    {"spintype",    spinType},
                                    {"odd",         odd },
                                    {"groupid",     groupid },
                                    {"data",        data }};

            if (spinType >= 100 && spinType < 200)
            {
                int freeSpinType            = (int)(long)   reader["freespintype"];
                odd                         = (double)      reader["realodd"];
                document["odd"]             = odd;
                document["freespintype"]    = freeSpinType;
                document["ranges"]          = new BsonArray(getRanges(gameInfo, odd, freeSpinType, gs.AllFreeSpinOdds));
            }
            else if(spinType >= 200)
            {
                document.Remove("groupid");
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
                    int     freeSpinType    = (int)(long)reader["freespintype"];
                    int[]   freeSpinTypes   = getPossibleFreeSpins(gameInfo.Name, freeSpinType);
                    double sum = 0.0;
                    for (int i = 0; i < freeSpinTypes.Length; i++)
                    {
                        if (gs.FreeSpinOddsInNormal[freeSpinTypes[i] - 200] == 0.0)
                        {

                        }
                        sum += gs.FreeSpinOddsInNormal[freeSpinTypes[i] - 200];
                    }

                    document["freewinrate"]  = odd + sum / freeSpinTypes.Length;
                    gs.NormalOddSum         += (odd + sum / freeSpinTypes.Length);
                    gs.NormalSelectCount++;
                    gs.NormalTotalCount++;
                }
                else
                {
                    gs.NormalTotalCount++;
                }
            }
            return document;
        }

        public async Task convertTask()
        {
            /*
            for(int i = 0; i < _gameInfoList.Count; i++)
            {
                if (_gameInfoList[i].Name == "CashElevator")
                {
                    await convertSqliteDBForCashElevator(_gameInfoList[i]);
                }
                else if (_gameInfoList[i].Name == "BroncoSpirit")
                {
                    await convertSqliteDBForBroncoSpirit(_gameInfoList[i]);
                }
                else if (_gameInfoList[i].Name == "OlympusGates")
                {
                    await convertSqliteDB(_gameInfoList[i]);
                    await convertPurSamplesSqliteDB(_gameInfoList[i]);
                }
                else
                {
                    await convertSqliteDB(_gameInfoList[i]);
                }
            }
            */
            for (int i = 0; i < _gameInfoList.Count; i++)
            {
                if (_gameInfoList[i].Name == "Triple8Gold" || _gameInfoList[i].Name == "MoneyRoll" || _gameInfoList[i].Name == "IrishCharms"
                    || _gameInfoList[i].Name == "DiamondsAreForever" || _gameInfoList[i].Name == "SixJokers")
                    await convertClassicSqliteDB(_gameInfoList[i]);
                else if (_gameInfoList[i].Name == "DragonKingdomEyesOfFire" || _gameInfoList[i].Name == "EmeraldKingClassic" || _gameInfoList[i].Name == "WildDepths")
                    await convertNewSqliteDBForGroupSpin(_gameInfoList[i]);
                else if (_gameInfoList[i].Name == "AztecBlaze" || _gameInfoList[i].Name == "GreedyWolf" || _gameInfoList[i].Name == "DrillThatGold" || _gameInfoList[i].Name == "ZeusVsHadesGodsOfWar")
                    await convertNewSqliteDBForAztecBlaze(_gameInfoList[i]);
                else if (_gameInfoList[i].Name == "MoneyStacks")
                    await convertSqliteDB(_gameInfoList[i]);
                else if (_gameInfoList[i].Name == "SantasXmasRush")
                    await convertSantasXmaxRushSqliteDB(_gameInfoList[i]);
                else
                    await convertNewSqliteDB(_gameInfoList[i]);

                /*
                if (_gameInfoList[i].Name == "AztecBlaze" || _gameInfoList[i].Name == "GreedyWolf" || _gameInfoList[i].Name == "DrillThatGold")
                    await convertNewSqliteDBForAztecBlaze(_gameInfoList[i]);
                else if (_gameInfoList[i].Name == "PinupGirls")
                    await convertNewSqliteDBForPinupGirls(_gameInfoList[i]);
                else
                    await convertNewSqliteDB(_gameInfoList[i]);

                if (_gameInfoList[i].Name == "SpiritOfAdventure")
                    await convertGambleOddsSqliteDB(_gameInfoList[i]);
                */
            }
            Console.WriteLine("Completed");
        }
        public async Task<bool> convertSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("./slotdata/{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                {
                    Console.WriteLine("File Not Found");
                    return false;

                }

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
                            defaultBet = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }
                    infoDocument["defaultbet"] = Math.Round(defaultBet, 2);
                    double[] averageFreeSpinOddsInNormal = null;
                    Dictionary<int, List<double>> allFreeSpinOdds           = new Dictionary<int, List<double>>();
                    Dictionary<int, double>       allFreeSpinAverageOdds    = new Dictionary<int, double>();
                    if (gameInfo.SelFreeSpin)
                    {
                        int freeSpinTypeCount = getFreeSpinTypeCount(gameInfo.Name);
                        Dictionary<int, double> dicSumOdds = new Dictionary<int, double>();
                        Dictionary<int, int> dicSpinCounts = new Dictionary<int, int>();

                        strCommand = "SELECT * FROM spins WHERE spintype >= 200 ORDER BY id";
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                int id = (int)(long)reader["id"];
                                int spinType = (int)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);

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

                    if(gameInfo.Name == "GuGuGu2")
                    {
                        double average = 0.0;
                        for (int i = 0; i < gameStatus.FreeSpinOddsInNormal.Length; i++)
                            average += gameStatus.FreeSpinOddsInNormal[i];

                        infoDocument["naturalfreespinrate"] = average / gameStatus.FreeSpinOddsInNormal.Length;
                    }

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
                    if(gameInfo.Name == "PowerOfThorMega" || gameInfo.Name == "ChristmasBigBassBonanza")
                    {
                        Random random = new Random((int)DateTime.Now.Ticks);
                        int count = 0;
                        switch(gameInfo.Name)
                        {
                            case "PowerOfThorMega":
                                count = 59523;
                                break;
                            case "ChristmasBigBassBonanza":
                                count = 2111;
                                break;
                        }
                        for(int i = 0; i < count; i++)
                        {
                            var emptyDocument       = documents[random.Next(0, documents.Count)];
                            emptyDocument           = emptyDocument.DeepClone().ToBsonDocument();
                            emptyDocument["_id"]    = idCounter;
                            documents.Add(emptyDocument);
                            idCounter++;

                            gameStatus.NormalSelectCount++;
                            gameStatus.NormalTotalCount++;
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
                                if((spinType > 0 && spinType < 200))
                                {

                                }
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
                    if(payoutRate < 0.992 || payoutRate > 1.004)
                    {

                    }
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
                        int purchaseCount = getMultiPurchaseCount(gameInfo.Name);
                        List<double> purchaseOdds = new List<double>();
                        for (int i = 0; i < purchaseCount; i++)
                        {
                            double multiple = gameInfo.PurchaseMultiple;
                            if (purchaseCount > 1)
                                multiple = getMultiPurMultiple(gameInfo, i);

                            double  sum = 0.0;
                            double  minSum = 0.0;
                            int     minCount = 0;
                            foreach (double odd in gameStatus.PurchaseOdds[i])
                            {
                                sum += odd;
                                if(odd >= multiple * 0.2 && odd <= multiple * 0.5)
                                {
                                    minCount++;
                                    minSum += odd;
                                }
                            }

                            double average  = sum / gameStatus.PurchaseOdds[i].Count;
                            if(average < multiple && gameInfo.Name != "Thor2")
                                return false;

                            double minAverage = minSum / minCount;
                            if(gameInfo.SelFreeSpin)
                            {
                                minSum   = 0.0;
                                minCount = gameStatus.PurchaseMinOdds[i].Count;
                                foreach (double odd in gameStatus.PurchaseMinOdds[i])
                                    minSum += odd;
                                minAverage = minSum / minCount;
                            }
                            if (minCount <= 50 || average < minAverage)
                                return false;

                            purchaseOdds.Add(minAverage);
                            purchaseOdds.Add(average);
                        }
                        infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                    }
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return false;
            }
        }
        public async Task<bool> convertNewSqliteDBForAztecBlaze(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format(".\\slotdata\\{0}.db", gameInfo.Name);
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

                    int     normalMaxID     = 0;
                    double  defaultBet      = 0.0;
                    int     normalMaxID2    = 0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID  = (int)(long)reader["normalmaxid"];
                            normalMaxID2 = (int)(long)reader["normalmaxid2"];
                            defaultBet   = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }

                    infoDocument["defaultbet"]      = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum         = 0.0;
                    gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount    = 0;
                    gameStatus.NormalTotalCount     = 0;
                    gameStatus.DefaultBet           = defaultBet;

                    int idCounter   = 1;
                    strCommand      = string.Format("SELECT * FROM spins WHERE id <={0} and bettype=0", normalMaxID.ToString());
                    command         = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     id          = (int)(long)reader["id"];
                            byte    spinType    = (byte)(long)reader["spintype"];
                            double  odd         = Math.Round((double)reader["odd"], 2);

                            if (odd == 0.0 && spinType == 0)
                            {
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }

                    infoDocument["emptycount1"] = idCounter - 1;
                    strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype=0", normalMaxID.ToString());
                    command     = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte spinType = (byte)(long)reader["spintype"];
                            double odd = Math.Round((double)reader["odd"], 2);
                            if ((spinType == 0 && odd > 0.0) || (spinType > 0 && spinType < 200))
                            {
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }

                    infoDocument["normalselectcount1"] = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid1"]       = documents.Count;
                    double payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                    if (payoutRate < 0.996 || payoutRate > 1.004)
                    {

                    }
                    strCommand = string.Format("SELECT * FROM spins WHERE id > {0} and bettype=0", normalMaxID.ToString());
                    command    = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }
                    gameStatus.NormalSelectCount = 0;
                    gameStatus.NormalOddSum      = 0.0;
                    int startId = idCounter;
                    infoDocument["antestartid"] = idCounter;
                    strCommand = string.Format("SELECT * FROM spins WHERE id <={0} and bettype=1", normalMaxID2.ToString());
                    command    = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = (int)(long)reader["id"];
                            byte spinType = (byte)(long)reader["spintype"];
                            double odd = Math.Round((double)reader["odd"], 2);

                            if (odd == 0.0 && spinType == 0)
                            {
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }
                    infoDocument["emptycount2"] = idCounter - startId;
                    strCommand                  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype=1", normalMaxID2.ToString());
                    command                     = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte spinType = (byte)(long)reader["spintype"];
                            double odd = Math.Round((double)reader["odd"], 2);
                            if ((spinType == 0 && odd > 0.0) || (spinType > 0 && spinType < 200))
                            {
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }

                    infoDocument["normalselectcount2"] = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid2"]       = documents.Count;
                    payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                    if (payoutRate < 1.5994 || payoutRate > 1.604)
                    {

                    }
                    strCommand = string.Format("SELECT * FROM spins WHERE id > {0} and bettype=1", normalMaxID2.ToString());
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }
                    if (gameInfo.SupportPurchaseFree)
                    {
                        int purchaseCount = getMultiPurchaseCount(gameInfo.Name);
                        List<double> purchaseOdds = new List<double>();
                        for (int i = 0; i < purchaseCount; i++)
                        {
                            double multiple = gameInfo.PurchaseMultiple;
                            if (purchaseCount > 1)
                                multiple = getMultiPurMultiple(gameInfo, i);

                            double sum = 0.0;
                            double minSum = 0.0;
                            int minCount = 0;
                            foreach (double odd in gameStatus.PurchaseOdds[i])
                            {
                                sum += odd;
                                if (odd >= multiple * 0.2 && odd <= multiple * 0.5)
                                {
                                    minCount++;
                                    minSum += odd;
                                }
                            }

                            double average = sum / gameStatus.PurchaseOdds[i].Count;
                            if (average < multiple)
                                return false;

                            double minAverage = minSum / minCount;
                            if (gameInfo.SelFreeSpin)
                            {
                                minSum = 0.0;
                                minCount = gameStatus.PurchaseMinOdds[i].Count;
                                foreach (double odd in gameStatus.PurchaseMinOdds[i])
                                    minSum += odd;
                                minAverage = minSum / minCount;
                            }
                            if (minCount <= 50 || average < minAverage)
                                return false;

                            purchaseOdds.Add(minAverage);
                            purchaseOdds.Add(average);
                        }
                        infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                    }

                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertNewSqliteDBForGroupSpin(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format(".\\slotdata\\{0}.db", gameInfo.Name);
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

                    infoDocument["defaultbet"]      = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum         = 0.0;
                    gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount    = 0;
                    gameStatus.NormalTotalCount     = 0;
                    gameStatus.DefaultBet           = defaultBet;

                    int idCounter = 1;
                    double minSum = 0.0;
                    strCommand    = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command       = new SQLiteCommand(strCommand, connection);
                    int totalCount = 0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     id          = (int)(long)reader["id"];
                            byte    spinType    = (byte)(long)reader["spintype"];
                            double  odd         = Math.Round((double)reader["odd"], 2);
                            int     count       = (int)(long)reader["count"];
                            if (odd <= 0.25 && spinType == 0)
                            {
                                minSum += (odd * count);
                                totalCount += count;
                                documents.Add(createDocumentForGroupSpin(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }
                    infoDocument["emptyRTP"]    = minSum / (double)(totalCount);
                    infoDocument["emptycount"]  = idCounter - 1;
                    strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte    spinType = (byte)(long)reader["spintype"];
                            double  odd      = Math.Round((double)reader["odd"], 2);
                            int     count    = (int)(long)reader["count"];
                            if (odd > 0.25 && spinType == 0)
                            {
                                documents.Add(createDocumentForGroupSpin(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                                minSum += (odd * count);
                                totalCount += count;
                            }
                        }
                    }
                    infoDocument["normalRTP"]           = minSum / (double)totalCount;
                    infoDocument["normalselectcount"]   = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid"]         = documents.Count;
                    
                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentForGroupSpin(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertSantasXmaxRushSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("./slotdata/{0}.db", gameInfo.Name);
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

                    List<int>       startIDs        = new List<int>();
                    List<int>       normalMaxIDs    = new List<int>();
                    List<double>    defaultBets     = new List<double>();
                    int             lineCount       = getClassicLineCount(gameInfo.Name);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            for(int i = 1; i <= lineCount; i++)
                            {
                                int normalMaxID = (int)(long)reader["normalmaxid" + i.ToString()];
                                double defaultBet = Math.Round((double)reader["defaultbet" + i.ToString()], 2);
                                normalMaxIDs.Add(normalMaxID);
                                defaultBets.Add(defaultBet);
                            }
                        }
                    }

                    infoDocument["defaultbet"]   = new BsonArray(defaultBets.ToArray());
                    List<int> emptyCounts        = new List<int>();
                    List<int> normalSelectCounts = new List<int>();
                    List<int> normalMaxIDList    = new List<int>();

                    int idCounter = 1;
                    gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                        
                    for (int i = 0; i < lineCount; i++)
                    {
                        startIDs.Add(idCounter);
                        gameStatus.NormalOddSum         = 0.0;
                        gameStatus.NormalSelectCount    = 0;
                        gameStatus.NormalTotalCount     = 0;
                        gameStatus.DefaultBet           = defaultBets[i];

                        strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}", normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        int emptyCount = 0;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int     id          = (int)(long)reader["id"];
                                byte    spinType    = (byte)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);

                                if (odd == 0.0)
                                {
                                    documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                    emptyCount++;
                                }
                            }
                        }

                        emptyCounts.Add(emptyCount);
                        strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}", normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte spinType = (byte)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);
                                if ((spinType == 0 && odd > 0.0) || (spinType > 0))
                                {
                                    documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                        normalSelectCounts.Add(gameStatus.NormalSelectCount);
                        normalMaxIDList.Add(documents.Count);

                        double payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                        if (payoutRate < 0.996 || payoutRate > 1.004)
                        {

                        }
                        strCommand = string.Format("SELECT * FROM spins WHERE id > {0} and bettype={1}", normalMaxIDs[i], i);
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, false));
                                idCounter++;
                            }
                        }

                        if(i > 0)
                            continue;

                        if (gameInfo.SupportPurchaseFree)
                        {
                            int purchaseCount = getMultiPurchaseCount(gameInfo.Name);
                            List<double> purchaseOdds = new List<double>();
                            for (int j = 0; j < purchaseCount; j++)
                            {
                                double multiple = gameInfo.PurchaseMultiple;
                                if (purchaseCount > 1)
                                    multiple = getMultiPurMultiple(gameInfo, j);

                                double sum      = 0.0;
                                double minSum   = 0.0;
                                int minCount    = 0;
                                foreach (double odd in gameStatus.PurchaseOdds[j])
                                {
                                    sum += odd;
                                    if (odd >= multiple * 0.2 && odd <= multiple * 0.5)
                                    {
                                        minCount++;
                                        minSum += odd;
                                    }
                                }

                                double average = sum / gameStatus.PurchaseOdds[j].Count;
                                if (average < multiple)
                                    return false;

                                double minAverage = minSum / minCount;
                                if (gameInfo.SelFreeSpin)
                                {
                                    minSum = 0.0;
                                    minCount = gameStatus.PurchaseMinOdds[j].Count;
                                    foreach (double odd in gameStatus.PurchaseMinOdds[j])
                                        minSum += odd;
                                    minAverage = minSum / minCount;
                                }
                                if (minCount <= 48 || average < minAverage)
                                    return false;

                                purchaseOdds.Add(minAverage);
                                purchaseOdds.Add(average);
                            }
                            infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                        }
                    }
                    infoDocument["emptycount"]          = new BsonArray(emptyCounts.ToArray());
                    infoDocument["normalselectcount"]   = new BsonArray(normalSelectCounts.ToArray());
                    infoDocument["normalmaxid"]         = new BsonArray(normalMaxIDList.ToArray());
                    infoDocument["startid"]             = new BsonArray(startIDs.ToArray());

                   await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertNewSqliteDBForPinupGirls(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("F:\\Jobs\\GodCasino\\GodCasinoServer\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", gameInfo.Name);
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

                    infoDocument["defaultbet"]      = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum         = 0.0;
                    gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount    = 0;
                    gameStatus.NormalTotalCount     = 0;
                    gameStatus.DefaultBet           = defaultBet;

                    int idCounter = 1;
                    double minSum = 0.0;
                    strCommand  = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         id          = (int)(long)reader["id"];
                            byte        spinType    = (byte)(long)reader["spintype"];
                            double      odd         = Math.Round((double)reader["odd"], 2);

                            if (odd <= 0.2 && spinType == 0)
                            {
                                minSum += odd;
                                documents.Add(createDocumentForPinupGirl(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }
                    infoDocument["emptyRTP"]   = minSum / (double) (idCounter - 1);
                    infoDocument["emptycount"] = idCounter - 1;
                    strCommand = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte    spinType = (byte)(long)reader["spintype"];
                            double  odd      = Math.Round((double)reader["odd"], 2);
                            if (odd > 0.2 && spinType == 0)
                            {
                                documents.Add(createDocumentForPinupGirl(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                                minSum += odd;
                            }
                        }
                    }
                    infoDocument["normalRTP"]           = minSum / (double) documents.Count;
                    infoDocument["normalselectcount"]   = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid"]         = documents.Count;
                    double payoutRate                   = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                    if (payoutRate < 0.996 || payoutRate > 1.004)
                    {

                    }
                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }

                    if (gameInfo.SupportPurchaseFree)
                    {
                        int purchaseCount = getMultiPurchaseCount(gameInfo.Name);
                        List<double> purchaseOdds = new List<double>();
                        for (int i = 0; i < purchaseCount; i++)
                        {
                            double multiple = gameInfo.PurchaseMultiple;
                            if (purchaseCount > 1)
                                multiple = getMultiPurMultiple(gameInfo, i);

                            double sum = 0.0;
                            double minSum1 = 0.0;
                            int minCount = 0;
                            foreach (double odd in gameStatus.PurchaseOdds[i])
                            {
                                sum += odd;
                                if (odd >= multiple * 0.2 && odd <= multiple * 0.5)
                                {
                                    minCount++;
                                    minSum1 += odd;
                                }
                            }

                            double average = sum / gameStatus.PurchaseOdds[i].Count;
                            if (average < multiple)
                                return false;

                            double minAverage = minSum1 / minCount;
                            if (gameInfo.SelFreeSpin)
                            {
                                minSum1 = 0.0;
                                minCount = gameStatus.PurchaseMinOdds[i].Count;
                                foreach (double odd in gameStatus.PurchaseMinOdds[i])
                                    minSum1 += odd;
                                minAverage = minSum1 / minCount;
                            }
                            if (minCount <= 50 || average < minAverage)
                                return false;

                            purchaseOdds.Add(minAverage);
                            purchaseOdds.Add(average);
                        }
                        infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                    }
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertNewSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName            = string.Format("./slotdata/{0}.db", gameInfo.Name);
                string strConnectionString    = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                var infoDocument = new BsonDocument();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int     normalMaxID = 0;
                    double  defaultBet  = 0.0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID     = (int)(long)reader["normalmaxid"];
                            defaultBet      = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }

                    infoDocument["defaultbet"]      = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum         = 0.0;
                    gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                    gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                    gameStatus.NormalSelectCount    = 0;
                    gameStatus.NormalTotalCount     = 0;
                    gameStatus.DefaultBet           = defaultBet;

                    int idCounter = 1;
                    strCommand      = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command         = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         id          = (int)(long)reader["id"];
                            byte        spinType    = (byte)(long)reader["spintype"];
                            double      odd         = Math.Round((double)reader["odd"], 2);

                            if (odd == 0.0 && spinType == 0)
                            {
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }

                    infoDocument["emptycount"] = idCounter - 1;
                    strCommand                 = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            byte    spinType    = (byte)(long)reader["spintype"];
                            double  odd         = Math.Round((double)reader["odd"], 2);
                            if ((spinType == 0 && odd > 0.0) || (spinType > 0 && spinType < 200))
                            {
                                if((spinType > 0 && spinType < 200))
                                {

                                }
                                documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                idCounter++;
                            }
                        }
                    }
                    if (gameInfo.SelFreeSpin)
                    {
                        strCommand  = "SELECT * FROM spins WHERE id <=" + normalMaxID.ToString();
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte    spinType = (byte)(long)reader["spintype"];
                                double  odd      = Math.Round((double)reader["odd"], 2);
                                if (spinType >= 200)
                                {
                                    documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                    }

                    infoDocument["normalselectcount"] = gameStatus.NormalSelectCount;
                    infoDocument["normalmaxid"]       = documents.Count;
                    double payoutRate                 = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                    if (payoutRate < 0.996 || payoutRate > 1.004)
                    {

                    }
                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentNew(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }

                    if (gameInfo.SupportPurchaseFree)
                    {
                        int             purchaseCount   = getMultiPurchaseCount(gameInfo.Name);
                        List<double>    purchaseOdds    = new List<double>();
                        for (int i = 0; i < purchaseCount; i++)
                        {
                            double multiple = gameInfo.PurchaseMultiple;
                            if (purchaseCount > 1)
                                multiple = getMultiPurMultiple(gameInfo, i);

                            double  sum         = 0.0;
                            double  minSum      = 0.0;
                            int     minCount    = 0;
                            foreach (double odd in gameStatus.PurchaseOdds[i])
                            {
                                sum += odd;
                                if (odd >= multiple * 0.2 && odd <= multiple * 0.5)
                                {
                                    minCount++;
                                    minSum += odd;
                                }
                            }

                            double average = sum / gameStatus.PurchaseOdds[i].Count;
                            if (average < multiple)
                                return false;

                            double minAverage = minSum / minCount;
                            if (gameInfo.SelFreeSpin)
                            {
                                minSum = 0.0;
                                minCount = gameStatus.PurchaseMinOdds[i].Count;
                                foreach (double odd in gameStatus.PurchaseMinOdds[i])
                                    minSum += odd;
                                minAverage = minSum / minCount;
                            }
                            if (minCount <= 48 || average < minAverage)
                                return false;

                            purchaseOdds.Add(minAverage);
                            purchaseOdds.Add(average);
                        }
                        infoDocument.Add("purchaseodds", new BsonArray(purchaseOdds.ToArray()));
                    }
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        public async Task<bool> convertClassicSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("./slotdata/{0}.db", gameInfo.Name);
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

                    List<int>       startIDs        = new List<int>();
                    List<int>       normalMaxIDs    = new List<int>();
                    List<double>    defaultBets     = new List<double>();
                    int             lineCount       = getClassicLineCount(gameInfo.Name);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            for(int i = 1; i <= lineCount; i++)
                            {
                                int normalMaxID = (int)(long)reader["normalmaxid" + i.ToString()];
                                double defaultBet = Math.Round((double)reader["defaultbet" + i.ToString()], 2);
                                normalMaxIDs.Add(normalMaxID);
                                defaultBets.Add(defaultBet);
                            }
                        }
                    }

                    infoDocument["defaultbet"]   = new BsonArray(defaultBets.ToArray());
                    List<int> emptyCounts        = new List<int>();
                    List<int> normalSelectCounts = new List<int>();
                    List<int> normalMaxIDList    = new List<int>();

                    int idCounter = 1;
                    for (int i = 0; i < lineCount; i++)
                    {
                        startIDs.Add(idCounter);
                        gameStatus.NormalOddSum         = 0.0;
                        gameStatus.PurchaseOdds         = new Dictionary<int, List<double>>();
                        gameStatus.PurchaseMinOdds      = new Dictionary<int, List<double>>();
                        gameStatus.NormalSelectCount    = 0;
                        gameStatus.NormalTotalCount     = 0;
                        gameStatus.DefaultBet           = defaultBets[i];

                        strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}", normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        int emptyCount = 0;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int     id          = (int)(long)reader["id"];
                                byte    spinType    = (byte)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);

                                if (odd == 0.0)
                                {
                                    documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                    emptyCount++;
                                }
                            }
                        }

                        emptyCounts.Add(emptyCount);
                        strCommand  = string.Format("SELECT * FROM spins WHERE id <={0} and bettype={1}", normalMaxIDs[i], i);
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte spinType = (byte)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);
                                if ((spinType == 0 && odd > 0.0) || (spinType > 0))
                                {
                                    documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                        normalSelectCounts.Add(gameStatus.NormalSelectCount);
                        normalMaxIDList.Add(documents.Count);

                        double payoutRate = gameStatus.NormalOddSum / gameStatus.NormalSelectCount;
                        if (payoutRate < 0.996 || payoutRate > 1.004)
                        {

                        }
                        strCommand = string.Format("SELECT * FROM spins WHERE id > {0} and bettype={1}", normalMaxIDs[i], i);
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                documents.Add(createDocumentClassic(gameInfo, reader, idCounter, gameStatus, false));
                                idCounter++;
                            }
                        }
                    }
                    infoDocument["emptycount"]          = new BsonArray(emptyCounts.ToArray());
                    infoDocument["normalselectcount"]   = new BsonArray(normalSelectCounts.ToArray());
                    infoDocument["normalmaxid"]         = new BsonArray(normalMaxIDList.ToArray());
                    infoDocument["startid"]             = new BsonArray(startIDs.ToArray());
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertPurSamplesSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("F:\\Jobs\\GodCasino\\GodCasinoServer\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM pursamples";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(new BsonDocument()
                            {
                                { "data", (string) reader["data"] }
                            });
                        }
                    }

                }
                await writePurSamplesDataToMongoDB(gameInfo, documents);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertGambleOddsSqliteDB(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("F:\\Jobs\\GodCasino\\GodCasinoServer\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                List<BsonDocument> documents = new List<BsonDocument>();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM gambledata";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(new BsonDocument()
                            {
                                { "minodd",  (double) reader["minodd"] },
                                { "maxodd",  (double) reader["maxodd"] },
                                { "realodd", (double) reader["realodd"] },
                                { "percent", (double) reader["percent"] }
                            });
                        }
                    }

                }
                await writeGambleOddsDataToMongoDB(gameInfo, documents);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> convertSqliteDBForBroncoSpirit(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("D:\\Workshop\\PragmaticPlayGames\\Server\\GITGameServerSolution2021(Sqlite)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                var infoDocument = new BsonDocument();

                List<BsonDocument> documents = new List<BsonDocument>();
                using (SQLiteConnection connection = new SQLiteConnection(strConnectionString))
                {
                    await connection.OpenAsync();
                    string strCommand = "SELECT * FROM info";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);

                    int     normalMaxID = 0;
                    double  defaultBet  = 0.0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            normalMaxID = (int)(long)reader["normalmaxid"];
                            defaultBet  = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }
                    infoDocument["defaultbet"] = defaultBet;

                    double[]                      averageFreeSpinOddsInNormal   = null;
                    Dictionary<int, List<double>> allFreeSpinOdds               = new Dictionary<int, List<double>>();
                    Dictionary<int, double>       allFreeSpinAverageOdds        = new Dictionary<int, double>();
                    if (gameInfo.SelFreeSpin)
                    {
                        int freeSpinTypeCount = getFreeSpinTypeCount(gameInfo.Name);
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

                        foreach (KeyValuePair<int, List<double>> pair in allFreeSpinOdds)
                            allFreeSpinAverageOdds[pair.Key] = allFreeSpinAverageOdds[pair.Key] / pair.Value.Count;

                        if (averageFreeSpinOddsInNormal == null)
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

                    List<int> groupIds = new List<int>();
                    strCommand = "SELECT groupid, islast FROM spins GROUP BY groupid, islast";
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int groupId = (int)(long)reader["groupid"];
                            int isLast = (int)(long)reader["islast"];
                            if (isLast == 1)
                                groupId += 100;

                            groupIds.Add(groupId);
                        }
                    }
                    groupIds.Sort();

                    int[] groupSpinCounts   = new int[groupIds.Count];
                    int[] groupStartIds     = new int[groupIds.Count];
                    int[] groupEmptyCounts  = new int[groupIds.Count];

                    double[] groupMinOdds   = new double[groupIds.Count];
                    double[] groupMeanOdds  = new double[groupIds.Count];

                    int idCounter = 1;
                    for (int i = 0; i < groupIds.Count; i++)
                    {
                        groupStartIds[i] = idCounter;
                        groupMinOdds[i]  = -1.0;
                        int groupID = groupIds[i];
                        int isLast  = 0;
                        if(groupID >= 100)
                        {
                            groupID -= 100;
                            isLast  = 1;
                        }
                        strCommand  = string.Format("SELECT * FROM spins WHERE id <= {0} and groupid={1} and islast={2}", normalMaxID, groupID, isLast);
                        command     = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                byte    spinType    = (byte)(long)reader["spintype"];
                                double  odd         = Math.Round((double)reader["odd"], 2);

                                if (odd == 0.0 && spinType == 0)
                                {
                                    groupMinOdds[i] = 0.0;
                                    documents.Add(createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                        groupEmptyCounts[i] = idCounter - groupStartIds[i];

                        if (groupEmptyCounts[i] == 0)
                        {
                            strCommand = string.Format("SELECT * FROM spins WHERE id <= {0} and groupid={1} and islast={2}", normalMaxID, groupID, isLast);
                            command = new SQLiteCommand(strCommand, connection);
                            int count = 0;
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    byte spinType = (byte)(long)reader["spintype"];
                                    double odd = Math.Round((double)reader["odd"], 2);

                                    if ((odd > 0.0 && spinType == 0) || (spinType >= 1 && spinType < 200))
                                    {
                                        var document = createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, true);
                                        if (document.Contains("freewinrate"))
                                            odd = (double)document["freewinrate"];

                                        if (groupMinOdds[i] == -1.0 || odd < groupMinOdds[i])
                                            groupMinOdds[i] = odd;

                                        groupMeanOdds[i] += odd;
                                        count++;
                                    }
                                }
                            }
                            groupMeanOdds[i] = groupMeanOdds[i] / count;
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    byte spinType = (byte)(long)reader["spintype"];
                                    double odd = Math.Round((double)reader["odd"], 2);

                                    if (odd > 0.0 && odd <= groupMeanOdds[i] && spinType == 0)
                                    {
                                        var document = createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, true);
                                        if (document.Contains("freewinrate"))
                                        {
                                            odd = (double)document["freewinrate"];
                                            document.Remove("freewinrate");
                                        }
                                        if(odd <= groupMeanOdds[i])
                                        {
                                            documents.Add(document);
                                            idCounter++;
                                        }
                                    }
                                }
                            }
                            groupEmptyCounts[i] = idCounter - groupStartIds[i];
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    byte spinType = (byte)(long)reader["spintype"];
                                    double odd = Math.Round((double)reader["odd"], 2);

                                    if ((odd > groupMeanOdds[i] && spinType == 0) || (spinType >= 1 && spinType < 200))
                                    {
                                        var document = createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, true);
                                        if (document.Contains("freewinrate"))
                                        {
                                            odd = (double)document["freewinrate"];
                                            document.Remove("freewinrate");
                                        }
                                        documents.Add(document);
                                        idCounter++;
                                    }
                                }
                            }

                            groupSpinCounts[i] = idCounter - groupStartIds[i];
                        }
                        else
                        {
                            strCommand = string.Format("SELECT * FROM spins WHERE id <= {0} and groupid={1} and islast={2}", normalMaxID, groupID, isLast);
                            command = new SQLiteCommand(strCommand, connection);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    byte spinType = (byte)(long)reader["spintype"];
                                    double odd = Math.Round((double)reader["odd"], 2);

                                    if ((odd > 0.0 && spinType == 0) || (spinType >= 1 && spinType < 200))
                                    {
                                        var document = createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, true);
                                        if (document.Contains("freewinrate"))
                                        {
                                            odd = (double)document["freewinrate"];
                                            document.Remove("freewinrate");
                                        }
                                        if (groupMinOdds[i] == -1.0 || odd < groupMinOdds[i])
                                            groupMinOdds[i] = odd;

                                        groupMeanOdds[i] += odd;
                                        documents.Add(document);
                                        idCounter++;
                                    }
                                }
                            }
                            groupSpinCounts[i]  = idCounter - groupStartIds[i];
                            groupMeanOdds[i]    = groupMeanOdds[i] / groupSpinCounts[i];
                        }

                    }

                    strCommand = string.Format("SELECT * FROM spins WHERE id <= {0} and spintype >= 200", normalMaxID);
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }

                    int normalMaxCount = idCounter - 1;

                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command    = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var document = createDocumentForBroncoSpirit(gameInfo, reader, idCounter, gameStatus, false);
                            document.Remove("freewinrate");
                            documents.Add(document);
                            idCounter++;
                        }
                    }

                    Dictionary<int, int> dicGroupIDs = new Dictionary<int, int>();
                    for (int i = 0; i < groupIds.Count; i++)
                        dicGroupIDs.Add(groupIds[i], i);

                    strCommand = "SELECT * FROM sequence";
                    command = new SQLiteCommand(strCommand, connection);
                    List<BsonDocument> sequences = new List<BsonDocument>();
                    int seqCounter = 1;
                    double  totalOdd    = 0.0;
                    int     totalCount  = 0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string  sequence = (string)reader["sequence"];
                            int     count    = (int) (long)reader["count"];
                            var document = new BsonDocument() {
                                {"_id",         seqCounter },
                                {"sequence",    sequence},
                                {"count",       count} };
                            sequences.Add(document);
                            seqCounter++;

                            string[] strParts = sequence.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            for(int i = 0; i < strParts.Length; i++)
                            {
                                int groupID = int.Parse(strParts[i]);
                                if (i == strParts.Length - 1)
                                    groupID += 100;

                                int index = dicGroupIDs[groupID];
                                totalOdd   += groupMeanOdds[index] * count;
                                totalCount += count;
                            }
                        }
                    }
                    double naturalOdd = totalOdd / totalCount;
                    infoDocument["groupids"]            = new BsonArray(groupIds.ToArray());
                    infoDocument["groupstartids"]       = new BsonArray(groupStartIds);
                    infoDocument["groupemptycounts"]    = new BsonArray(groupEmptyCounts);
                    infoDocument["groupspincounts"]     = new BsonArray(groupSpinCounts);
                    infoDocument["groupminodds"]        = new BsonArray(groupMinOdds);
                    infoDocument["groupmeanodds"]       = new BsonArray(groupMeanOdds);
                    infoDocument["normalmaxid"]         = normalMaxCount;
                    await writeInfoDataToMongoDB(gameInfo, infoDocument, documents, sequences);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }        
        public async Task<bool> convertSqliteDBForCashElevator(GameInfo gameInfo)
        {
            try
            {
                GameInternalStatus gameStatus = new GameInternalStatus();
                string strFileName = string.Format("D:\\Workshop\\PragmaticPlayGames\\Server\\GITGameServerSolution2021(Sqlite)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", gameInfo.Name);
                string strConnectionString = @"Data Source=" + strFileName;
                if (!File.Exists(strFileName))
                    return false;

                var infoDocument = new BsonDocument();

                List<BsonDocument> documents = new List<BsonDocument>();
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
                            defaultBet = Math.Round((double)reader["defaultbet"], 2);
                        }
                    }

                    infoDocument["defaultbet"]      = Math.Round(defaultBet, 2);
                    gameStatus.NormalOddSum         = 0.0;
                    gameStatus.NormalSelectCount    = 0;
                    gameStatus.NormalTotalCount     = 0;
                    gameStatus.DefaultBet           = defaultBet;

                    int[] floorStartIds             = new int[12];
                    int[] floorEmptyCounts          = new int[12];
                    int[] floorSpinCounts           = new int[12];

                    int idCounter = 1;
                    for (int i = 0; i < 12; i++)
                    {
                        floorStartIds[i] = idCounter;
                        strCommand = string.Format("SELECT * FROM spins WHERE id <= {0} and beginfloor={1}", normalMaxID, i + 1);
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = (int)(long)reader["id"];
                                byte spinType = (byte)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);

                                if (odd == 0.0 && spinType == 0)
                                {
                                    documents.Add(createDocumentForCashElevator(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                        floorEmptyCounts[i] = idCounter - floorStartIds[i];

                        strCommand = string.Format("SELECT * FROM spins WHERE id <= {0} and beginfloor={1}", normalMaxID, i + 1);
                        command = new SQLiteCommand(strCommand, connection);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = (int)(long)reader["id"];
                                byte spinType = (byte)(long)reader["spintype"];
                                double odd = Math.Round((double)reader["odd"], 2);

                                if ((odd > 0.0 && spinType == 0) || spinType > 0)
                                {
                                    documents.Add(createDocumentForCashElevator(gameInfo, reader, idCounter, gameStatus, true));
                                    idCounter++;
                                }
                            }
                        }
                        floorSpinCounts[i] = idCounter - floorStartIds[i];
                    }

                    infoDocument["normalmaxid"] = idCounter - 1;
                    strCommand = "SELECT * FROM spins WHERE id >" + normalMaxID.ToString();
                    command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            documents.Add(createDocumentForCashElevator(gameInfo, reader, idCounter, gameStatus, false));
                            idCounter++;
                        }
                    }
                    infoDocument["floorstartids"]    = new BsonArray(floorStartIds);
                    infoDocument["flooremptycounts"] = new BsonArray(floorEmptyCounts);
                    infoDocument["floorspincounts"]  = new BsonArray(floorSpinCounts);

                }
                await writeInfoDataToMongoDB(gameInfo, infoDocument, documents);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private int[] getRanges(GameInfo gameInfo, double realOdd, int freeSpinGroup, Dictionary<int, List<double>> dicSpinOddsPerType)
        {
            double[] minRanges = new double[] { 20.0, 10.0, 50.0, 100.0, 300.0, 500.0,   1000.0 };
            double[] maxRanges = new double[] { 50.0, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0 };

            if(gameInfo.SupportPurchaseFree)
            {
                minRanges[0] = gameInfo.PurchaseMultiple * 0.2;
                maxRanges[0] = gameInfo.PurchaseMultiple * 0.5;
            }
            int[] spinTypes = getPossibleFreeSpins(gameInfo.Name, freeSpinGroup);
            List<int> ranges = new List<int>();
            for(int i = 0; i <minRanges.Length; i++)
            {
                if (realOdd > maxRanges[i])
                    continue;

                double minOdd = minRanges[i] - realOdd;
                if (minOdd < 0.0)
                    minOdd = 0.0;
                double maxOdd = maxRanges[i] - realOdd;
                bool notFound = false;
                for(int j = 0; j < spinTypes.Length; j++)
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
        private double getMinOdd(GameInfo gameInfo, double realOdd, int freeSpinGroup, Dictionary<int, List<double>> dicSpinOddsPerType)
        {
            double minOdd = gameInfo.PurchaseMultiple * 0.2;
            double maxOdd = gameInfo.PurchaseMultiple * 0.5;

            int[] spinTypes = getPossibleFreeSpins(gameInfo.Name, freeSpinGroup);
            if (realOdd > maxOdd)
                return -1.0;

            minOdd = minOdd - realOdd;
            if (minOdd < 0.0)
                minOdd = 0.0;
            maxOdd = maxOdd - realOdd;
            double averageSum = 0.0;
            for (int j = 0; j < spinTypes.Length; j++)
            {
                double  sum     = 0.0;
                int     count   = 0;
                foreach(double odd in dicSpinOddsPerType[spinTypes[j]])
                {
                    if(odd >= minOdd && odd <= maxOdd)
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
        private bool findInRange(List<double> odds, double minOdd, double maxOdd)
        {
            foreach(double odd in odds)
            {
                if (odd >= minOdd && odd <= maxOdd)
                    return true;
            }
            return false;
        }
        private int getFreeSpinTypeCount(string strGameName)
        {
            switch (strGameName)
            {
                case "FiveLionsGold":
                case "FiveLionsMega":
                case "FiveLions":
                    return 7;
                case "TheDogHouseMega":
                case "WildBooster":
                    return 8;
                case "GreatRhinoMega":
                case "AztecKingMega":
                    return 12;
                case "BroncoSpirit":
                    return 6;
                case "EightDragons":
                    return 7;
                case "PowerOfThorMega":
                    return 6;
                case "KoiPond":
                case "GodOfWar":
                case "TheBeastWar":
                case "FiveLuckyLions":
                    return 5;
                case "GuGuGu2":
                case "ShouXin":
                    return 3;
                case "JumpHigh2":
                    return 15;
                case "WildWildRichesMega":
                    return 8;
                case "EmperorCaishen":
                    return 5;
                case "WildSpells":
                    return 3;
                case "FishinReels":
                    return 6;
                case "TheWildMachine":
                    return 2;
                case "HoneyHoneyHoney":
                    return 2;
                case "VampiresVSWolves":
                    return 2;
                case "PixieWings":
                case "DragonKingdom":
                case "JourneyToTheWest":
                case "StreetRacer":
                case "RiseOfSamurai":
                    return 5;
                case "LadyGodiva":
                case "MightyKong":
                    return 4;
                case "ReleaseTheKraken2":
                    return 20;
                case "KingdomOfTheDead":
                    return 2;
                case "RobberStrike":
                    return 2;
                case "BigBassMissionFishin":
                    return 2;
                case "MoneyStacks":
                    return 8;
                case "VolcanoGoddess":
                    return 2;
            }
            return 0;
        }
        private int[] getReleaseTheKrakenFreeSpins(string strFreeSpins)
        {
            string[] strSpinTypes = strFreeSpins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int[]    spinTypes    = new int[strSpinTypes.Length];
            for (int i = 0; i < strSpinTypes.Length; i++)
                spinTypes[i] = int.Parse(strSpinTypes[i]);
            return spinTypes;
        }
        private int[] getPossibleFreeSpins(string strGameName, int freeSpinGroup)
        {
            switch(strGameName)
            {
                case "FiveLionsGold":
                case "FiveLionsMega":
                case "FiveLions":
                    return new int[] { 200, 201, 202, 203, 204, 205, 206 };
                case "TheDogHouseMega":
                case "WildBooster":
                case "MoneyStacks":
                    {
                        switch (freeSpinGroup)
                        {
                            case 0:
                                return new int[] { 200, 201 };
                            case 1:
                                return new int[] { 202, 203 };
                            case 2:
                                return new int[] { 204, 205 };
                            case 3:
                                return new int[] { 206, 207 };
                        }
                    }
                    break;
                case "GreatRhinoMega":
                case "AztecKingMega":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202, 203 };
                        else if (freeSpinGroup == 1)
                            return new int[] { 204, 205, 206, 207 };
                        else if (freeSpinGroup == 2)
                            return new int[] { 208, 209, 210, 211 };
                    }
                    break;
                case "BroncoSpirit":
                    {
                        switch (freeSpinGroup)
                        {
                            case 0:
                                return new int[] { 200, 201 };
                            case 1:
                                return new int[] { 202, 203 };
                            case 2:
                                return new int[] { 204, 205 };
                        }
                    }
                    break;
                case "EightDragons":
                    return new int[] { 200, 201, 202, 203, 204 };
                case "PowerOfThorMega":
                    {
                        switch (freeSpinGroup)
                        {
                            case 0:
                                return new int[] { 200, 201, 202, 203 };
                            case 1:
                                return new int[] { 201, 202, 203 };
                            case 2:
                                return new int[] { 202, 203 };
                            case 3:
                                return new int[] { 203 };
                            case 4:
                                return new int[] { 204 };
                            case 5:
                                return new int[] { 205 };

                        }
                        return new int[] { 200, 201, 202, 203, 204 };
                    }
                case "KoiPond":
                case "GodOfWar":
                case "TheBeastWar":
                case "FiveLuckyLions":
                    return new int[] { 200, 201, 202, 203, 204 };
                case "GuGuGu2":
                case "ShouXin":
                    return new int[] { 200, 201, 202 };
                case "JumpHigh2":
                    {
                        if (freeSpinGroup == 0)
                            return new int[] { 200, 201, 202, 203, 204 };
                        else if (freeSpinGroup == 1)
                            return new int[] { 205, 206, 207, 208, 209 };
                        else if (freeSpinGroup == 2)
                            return new int[] { 210, 211, 212, 213, 214 };
                    }
                    break;
                case "MagicianSecrets":
                    return new int[] { 200, 201 };
                case "BookOfTheFallen":
                    return new int[] { 200, 201, 202, 203, 204, 205, 206, 207, 208 };
                case "QueenOfGods":
                    return new int[] { 200, 201, 202, 203 };
                case "EyeOfCleopatra":
                    {
                        switch (freeSpinGroup)
                        {
                            case 0:
                                return new int[] { 200, 201, 202, 203 };
                            case 1:
                                return new int[] { 204, 205, 206, 207 };
                            default:
                                return new int[] { 208, 209, 210, 211 };
                        }
                    }
                case "WildWildRichesMega":
                    {
                        List<int> freeSpinTypes = new List<int>();
                        for (int i = freeSpinGroup; i < 8; i++)
                            freeSpinTypes.Add(200 + i);

                        return freeSpinTypes.ToArray();
                    }
                case "EmperorCaishen":
                    {
                        return new int[] { 200, 201, 202, 203, 204 };
                    }
                case "WildSpells":
                    {
                        return new int[] { 200, 201, 202};
                    }
                case "FishinReels":
                    {
                        switch (freeSpinGroup)
                        {
                            case 0:
                                return new int[] { 200, 201 };
                            case 1:
                                return new int[] { 202, 203 };
                            default:
                                return new int[] { 204, 205 };
                        }
                    }
                case "TheWildMachine":
                    {
                        return new int[] { 200, 201 };
                    }
                case "HoneyHoneyHoney":
                    {
                        return new int[] { 200, 201 };
                    }
                case "VampiresVSWolves":
                    {
                        return new int[] { 200, 201 };
                    }
                case "PixieWings":
                case "DragonKingdom":
                case "JourneyToTheWest":
                case "StreetRacer":
                case "RiseOfSamurai":
                    {
                        return new int[] { 200, 201, 202, 203, 204 };
                    }
                case "LadyGodiva":
                case "MightyKong":
                    {
                        return new int[] { 200, 201, 202, 203 };
                    }
                case "KingdomOfTheDead":
                    {
                        return new int[] { 200, 201 };
                    }
                case "RobberStrike":
                    {
                        return new int[] { 200, 201 };
                    }

            }
            return new int[0];
        }
        private int getMultiPurchaseCount(string strGameName)
        {
            switch(strGameName)
            {
                case "HandOfMidas":
                    return 3;
                case "TreasureWild":
                    return 12;
                case "StickyPiggy":
                    return 3;
                case "SantaGreatGifts":
                    return 4;
                case "FloatingDragonMega":
                    return 2;
                case "GatotKacaFury":
                    return 3;
                case "OctobeerFortunes":
                    return 12;
                case "BookOfTheFallen":
                    return 10;
                case "PeakPower":
                    return 2;
                case "WildWestDuels":
                    return 3;
                case "BigBassHoldSpinner":
                    return 2;
                case "ReleaseTheKraken2":
                    return 2;
                case "WildCelebrityBusMega":
                    return 2;
                case "ZeusVsHadesGodsOfWar":
                    return 4;
                case "FloatingDragonDragonBoatFestival":
                    return 2;
                case "FatPanda":
                    return 2;
                case "CowboyCoins":
                    return 5;
                case "ForgeOfOlympus":
                    return 4;
                case "BigBassHoldAndSpinnerMegaways":
                    return 2;
                case "GoldOasis":
                    return 2;
                case "TheWildGang":
                    return 2;
                case "FloatingDragonNewYearFestivalUltraMegaways":
                    return 2;
                case "TheBigDawgs":
                    return 2;
                case "YearOfTheDragonKing":
                    return 4;
                case "TheAlterEgo":
                    return 3;
                case "BigBassFloatsMyBoat":
                    return 2;
                case "PompeiiMegareelsMegaways":
                    return 2;
                case "StrawberryCocktail":
                    return 3;
                case "SugarRush1000":
                    return 2;
                case "BigBassSecretsOfTheGoldenLake":
                    return 2;
                case "LobsterBobsSeaFoodAndWinIt":
                    return 2;
                case "ReleaseTheBison":
                    return 3;
                case "FruityTreats":
                    return 2;
                case "RiseOfPyramids":
                case "StarlightPrincessPachi":
                case "SweetBonanza1000":
                case "MedusasStone":
                case "TinyToads":
                case "BigBassBonanza3Reeler":
                case "FonzosFelineFortunes":
                case "WildWildPearls":
                case "BrickHouseBonanza":
                case "BiggerBassSplash":
                case "JohnHunterAndGalileosSecrets":
                case "BigBassReturnToTheRaces":
                case "LuckyWildPub":
                case "WildWildJoker":
                case "BanditMega":
                case "TheDogHouseRoyalHunt":
                case "BigBassBonanza1000":
                case "GatesOfOlympusSuperScatter":
                case "MadMuertos":
                case "SleepingDragon":
                case "SweetBonanza1000Dice":
                case "EyeOfSpartacus":
                case "WildWestGoldBlazingBounty":
                case "BigBassBoxingBonusRound":
                case "GemFireFortune":
                case "WavesOfPoseidon":
                case "ClubTropicanaHappyHour":
                case "AlienInvaders":
                case "MummysJewels":
                case "SweetBonanzaSuperScatter":
                case "ZombieSchoolMega":
                case "GatesOfHades":
                    return 2;
                case "JackpotHunter":
                    return 2;
                case "BigBassDoubleDownDeluxe":
                    return 2;
                case "ForgingWilds":
                    return 2;
                case "MoneyStacks":
                case "MoneyStacksMega":
                    return 4;
                case "TheDogHouseMuttleyCrew":
                case "GemTrio":
                case "BigBassReelRepeat":
                    return 3;
                case "ChestsOfCaiShen":
                case "ReleaseTheKrakenMega":
                case "CandyCorner":
                case "HimalayanWild":
                    return 2;
            }
            return 1;
        }
        private double getMultiPurMultiple(GameInfo gameInfo, int freeGroupType)
        {
            switch (gameInfo.Name)
            {
                case "HandOfMidas":
                    return 100.0 * (freeGroupType + 1);
                case "TreasureWild":
                    {
                        int[] multiples = new int[] { 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 150 };
                        return multiples[freeGroupType];
                    }
                case "StickyPiggy":
                    {
                        int[] multiples = new int[] { 100, 200, 400 };
                        return multiples[freeGroupType];
                    }
                case "SantaGreatGifts":
                    {
                        int[] multiples = new int[] { 100, 200, 400, 500 };
                        return multiples[freeGroupType];
                    }
                case "FloatingDragonMega":
                    {
                        int[] multiples = new int[] { 100, 120 };
                        return multiples[freeGroupType];
                    }
                case "GatotKacaFury":
                    {
                        int[] multiples = new int[] { 100, 200, 300 };
                        return multiples[freeGroupType];
                    }
                case "OctobeerFortunes":
                    {
                        double[] multiples = new double[] { 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 150 };
                        return multiples[freeGroupType];
                    }
                case "BookOfTheFallen":
                    {
                        if (freeGroupType == 0)
                            return 100.0;
                        else
                            return 10.0;
                    }
                case "PeakPower":
                    {
                        if (freeGroupType == 0)
                            return 100.0;
                        else
                            return 300.0;
                    }
                case "WildWestDuels":
                    {
                        double[] multiples = new double[] { 100, 200, 400 };
                        return multiples[freeGroupType];
                    }
                case "BigBassHoldSpinner":
                    {
                        double[] multiples = new double[] { 100, 100 };
                        return multiples[freeGroupType];
                    }
                case "ReleaseTheKraken2":
                    {
                        double[] multiples = new double[] { 100, 250 };
                        return multiples[freeGroupType];
                    }
                case "WildCelebrityBusMega":
                    {
                        double[] multiples = new double[] { 20, 100 };
                        return multiples[freeGroupType];
                    }
                case "ZeusVsHadesGodsOfWar":
                    {
                        double[] multiples = new double[] { 150, 300, 75, 300 };
                        return multiples[freeGroupType];
                    }
                case "FloatingDragonDragonBoatFestival":
                    {
                        double[] multiples = new double[] { 100, 100 };
                        return multiples[freeGroupType];
                    }
                case "FatPanda":
                    {
                        double[] multiples = new double[] { 70, 300 };
                        return multiples[freeGroupType];
                    }
                case "CowboyCoins":
                    {
                        double[] multiples = new double[] { 100, 500, 500, 500, 500 };
                        return multiples[freeGroupType];
                    }
                case "ForgeOfOlympus":
                    {
                        double[] multiples = new double[] { 100, 200, 400, 500 };
                        return multiples[freeGroupType];
                    }
                case "BigBassHoldAndSpinnerMegaways":
                    {
                        double[] multiples = new double[] { 100, 120 };
                        return multiples[freeGroupType];
                    }
                case "GoldOasis":
                    {
                        double[] multiples = new double[] { 100, 200 };
                        return multiples[freeGroupType];
                    }
                case "TheWildGang":
                    {
                        double[] multiples = new double[] { 80, 200 };
                        return multiples[freeGroupType];
                    }
                case "FloatingDragonNewYearFestivalUltraMegaways":
                    {
                        double[] multiples = new double[] { 100, 120 };
                        return multiples[freeGroupType];
                    }
                case "TheBigDawgs":
                    {
                        double[] multiples = new double[] { 80, 250 };
                        return multiples[freeGroupType];
                    }
                case "YearOfTheDragonKing":
                    {
                        double[] multiples = new double[] { 100, 60, 140, 350 };
                        return multiples[freeGroupType];
                    }
                case "TheAlterEgo":
                    {
                        double[] multiples = new double[] { 100, 150, 200 };
                        return multiples[freeGroupType];
                    }
                case "BigBassFloatsMyBoat":
                case "PompeiiMegareelsMegaways":
                    {
                        double[] multiples = new double[] { 100, 300 };
                        return multiples[freeGroupType];
                    }
                case "StrawberryCocktail":
                    {
                        double[] multiples = new double[] { 60, 200, 600 };
                        return multiples[freeGroupType];
                    }
                case "SugarRush1000":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "BigBassSecretsOfTheGoldenLake":
                    {
                        double[] multiples = new double[] { 100, 270 };
                        return multiples[freeGroupType];
                    }
                case "LobsterBobsSeaFoodAndWinIt":
                    {
                        double[] multiples = new double[] { 50, 50 };
                        return multiples[freeGroupType];
                    }
                case "ReleaseTheBison":
                    {
                        double[] multiples = new double[] { 80, 150, 300 };
                        return multiples[freeGroupType];
                    }
                case "FruityTreats":
                    {
                        double[] multiples = new double[] { 100, 200 };
                        return multiples[freeGroupType];
                    }
                case "RiseOfPyramids":
                    {
                        double[] multiples = new double[] { 30, 80 };
                        return multiples[freeGroupType];
                    }
                case "StarlightPrincessPachi":
                    {
                        double[] multiples = new double[] { 70, 300 };
                        return multiples[freeGroupType];
                    }
                case "SweetBonanza1000":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "MedusasStone":
                    {
                        double[] multiples = new double[] { 75, 200 };
                        return multiples[freeGroupType];
                    }
                case "JackpotHunter":
                    {
                        double[] multiples = new double[] { 20, 60 };
                        return multiples[freeGroupType];
                    }
                case "BigBassDoubleDownDeluxe":
                    {
                        double[] multiples = new double[] { 100, 300 };
                        return multiples[freeGroupType];
                    }
                case "ForgingWilds":
                    {
                        double[] multiples = new double[] { 75, 550 };
                        return multiples[freeGroupType];
                    }
                case "MoneyStacks":
                    {
                        double[] multiples = new double[] { 80, 120, 200, 240 };
                        return multiples[freeGroupType];
                    }
                case "TheDogHouseMuttleyCrew":
                    {
                        double[] multiples = new double[] { 100, 250, 500 };
                        return multiples[freeGroupType];
                    }
                case "ChestsOfCaiShen":
                    {
                        double[] multiples = new double[] { 50, 100 };
                        return multiples[freeGroupType];
                    }
                case "ReleaseTheKrakenMega":
                    {
                        double[] multiples = new double[] { 100, 150 };
                        return multiples[freeGroupType];
                    }
                case "CandyCorner":
                    {
                        double[] multiples = new double[] { 100, 250 };
                        return multiples[freeGroupType];
                    }
                case "HimalayanWild":
                    {
                        double[] multiples = new double[] { 100, 300 };
                        return multiples[freeGroupType];
                    }
                case "TinyToads":
                    {
                        double[] multiples = new double[] { 100, 400 };
                        return multiples[freeGroupType];
                    }
                case "BigBassBonanza3Reeler":
                    {
                        double[] multiples = new double[] { 100, 300 };
                        return multiples[freeGroupType];
                    }
                case "MoneyStacksMega":
                    {
                        double[] multiples = new double[] { 120, 180, 240, 300 };
                        return multiples[freeGroupType];
                    }
                case "FonzosFelineFortunes":
                    {
                        double[] multiples = new double[] { 100, 270 };
                        return multiples[freeGroupType];
                    }
                case "WildWildPearls":
                    {
                        double[] multiples = new double[] { 75, 150 };
                        return multiples[freeGroupType];
                    }
                case "BrickHouseBonanza":
                    {
                        double[] multiples = new double[] { 100, 200 };
                        return multiples[freeGroupType];
                    }
                case "BiggerBassSplash":
                    {
                        double[] multiples = new double[] { 100, 350 };
                        return multiples[freeGroupType];
                    }
                case "JohnHunterAndGalileosSecrets":
                    {
                        double[] multiples = new double[] { 80, 200 };
                        return multiples[freeGroupType];
                    }
                case "BigBassReturnToTheRaces":
                    {
                        double[] multiples = new double[] { 100, 270 };
                        return multiples[freeGroupType];
                    }
                case "LuckyWildPub":
                    {
                        double[] multiples = new double[] { 50, 100 };
                        return multiples[freeGroupType];
                    }
                case "WildWildJoker":
                    {
                        double[] multiples = new double[] { 100, 250 };
                        return multiples[freeGroupType];
                    }
                case "BanditMega":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "TheDogHouseRoyalHunt":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "BigBassBonanza1000":
                    {
                        double[] multiples = new double[] { 100, 450 };
                        return multiples[freeGroupType];
                    }
                case "GatesOfOlympusSuperScatter":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "MadMuertos":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "SleepingDragon":
                    {
                        double[] multiples = new double[] { 100, 100 };
                        return multiples[freeGroupType];
                    }
                case "SweetBonanza1000Dice":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "EyeOfSpartacus":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "WildWestGoldBlazingBounty":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "BigBassBoxingBonusRound":
                    {
                        double[] multiples = new double[] { 100, 300 };
                        return multiples[freeGroupType];
                    }
                case "GemFireFortune":
                    {
                        double[] multiples = new double[] { 100, 400 };
                        return multiples[freeGroupType];
                    }
                case "WavesOfPoseidon":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "ClubTropicanaHappyHour":
                    {
                        double[] multiples = new double[] { 100, 350 };
                        return multiples[freeGroupType];
                    }
                case "AlienInvaders":
                    {
                        double[] multiples = new double[] { 65, 400 };
                        return multiples[freeGroupType];
                    }
                case "MummysJewels":
                    {
                        double[] multiples = new double[] { 50, 100 };
                        return multiples[freeGroupType];
                    }
                case "SweetBonanzaSuperScatter":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "GemTrio":
                    {
                        double[] multiples = new double[] { 50, 100, 200 };
                        return multiples[freeGroupType];
                    }
                case "ZombieSchoolMega":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
                case "BigBassReelRepeat":
                    {
                        double[] multiples = new double[] { 100, 1250, 160 };
                        return multiples[freeGroupType];
                    }
                case "GatesOfHades":
                    {
                        double[] multiples = new double[] { 100, 500 };
                        return multiples[freeGroupType];
                    }
            }
            return gameInfo.PurchaseMultiple;
        }

        private int getClassicLineCount(string strGameName)
        {
            switch(strGameName)
            {
                case "Triple8Gold":
                    return 5;
                case "MoneyRoll":
                    return 5;
                case "IrishCharms":
                    return 3;
                case "DiamondsAreForever":
                    return 3;
                case "SixJokers":
                    return 5;
                case "SantasXmasRush":
                    return 3;
            }
            return 0;
        }
        protected Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        public async Task checkMongoDB()
        {
            var dbClient    = new MongoClient("mongodb://127.0.0.1");
            var spinDB      = dbClient.GetDatabase(_strDBName);
            var collection  = spinDB.GetCollection<BsonDocument>("infos");
            var documents   = await collection.Find(_ => true).ToListAsync();
            for(int i = 11; i < documents.Count; i++)
            {
                var document = documents[i];
                string gameName = (string) document["name"];

                var gameCollection = spinDB.GetCollection<BsonDocument>(gameName);
                var gameDocuments  = await gameCollection.Find(_ => true).ToListAsync();

                foreach(var gameDocument in gameDocuments)
                {
                    string strData = (string) gameDocument["data"];
                    string[] strLines = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach(string strLine in strLines)
                    {
                        var dicParams = splitResponseToParams(strLine);
                        if(dicParams.ContainsKey("apwa"))
                        {
                            try
                            {
                                double apwa = double.Parse(dicParams["apwa"]);
                            }
                            catch
                            {

                            }
                        }
                    }

                }
            }
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
