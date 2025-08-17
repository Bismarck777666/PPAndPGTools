using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    public class SpinDataQueue
    {
        private static SpinDataQueue _sInstance = new SpinDataQueue();

        public static SpinDataQueue Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public List<SpinData>                   _responseQueue                  = new List<SpinData>();
        public List<PinupGirlsResponse>         _girlsResponseQueue             = new List<PinupGirlsResponse>();
        public List<GroupedSpinDataResponse>    _groupedSpinDataResponseQueue   = new List<GroupedSpinDataResponse>();
        public List<BroncoSequence>             _sequenceQueue                  = new List<BroncoSequence>();

        private bool        _mustStop   = false;
        private double      _defaultBet = 0.0;
        private double[]    _minOdds    = new double[]  { 50, 100,  300, 500,  1000 };
        private double[]    _maxOdds    = new double[]  { 100, 300, 500, 1000, 3000 };
        private int[]       _minCounts  = new int[]     { 150, 100, 80,  60,   30 };
        private int[]       _maxCounts  = new int[]     { 300, 300, 200, 100,  100 };
        private const int NaturalMaxCount = 200000;

        public void doStop()
        {
            _mustStop = true;
        }
        public void setDefaultBet(double defaultBet)
        {
            _defaultBet = defaultBet;
        }
        public void addBroncoSequence(BroncoSequence sequence)
        {
            lock(_sequenceQueue)
            {
                _sequenceQueue.Add(sequence);
            }
        }
        public void addSpinDataToQueue(PinupGirlsResponse response)
        {
            lock(_girlsResponseQueue)
            {
                response.TotalOdd = Math.Round(response.TotalWin  / (response.TotalWins.Count * _defaultBet), 6);
                response.SpinOdds = new List<double>();
                for(int i = 0; i < response.TotalWins.Count; i++)
                    response.SpinOdds.Add(Math.Round(response.TotalWins[i] / _defaultBet, 2));
                
                _girlsResponseQueue.Add(response);
            }
        }
        public void addSpinDataToQueue(GroupedSpinDataResponse response)
        {
            lock (_groupedSpinDataResponseQueue)
            {
                response.TotalOdd = Math.Round(response.TotalWin / (response.TotalWins.Count * _defaultBet), 6);
                response.SpinOdds = new List<double>();
                for (int i = 0; i < response.TotalWins.Count; i++)
                    response.SpinOdds.Add(Math.Round(response.TotalWins[i] / _defaultBet, 2));
                
                _groupedSpinDataResponseQueue.Add(response);
            }
        }
        public void addOnlyFreeSpinDataToQueue(List<SpinResponse> responses)
        {
            lock(_responseQueue)
            {
                for (int i = 0; i < responses.Count; i++)
                {
                    SpinData childSpin = new SpinData();
                    childSpin.SpinOdd = responses[i].TotalWin / _defaultBet;
                    childSpin.SpinType = responses[i].SpinType;
                    childSpin.Response = responses[i].Response;
                    _responseQueue.Add(childSpin);
                }
            }
        }
        public void addSpinDataToQueue(List<SpinResponse> responses)
        {
            lock (_responseQueue)
            {
                if(responses.Count == 1)
                {
                    if(responses[0] is BroncoSpinResponse)
                    {
                        BroncoSpinData spinData = new BroncoSpinData();
                        spinData.RealOdd    = responses[0].RealWin / _defaultBet;
                        spinData.SpinOdd    = responses[0].TotalWin / _defaultBet;
                        spinData.Response   = responses[0].Response;
                        spinData.SpinType   = responses[0].SpinType;
                        spinData.Wilds      = (responses[0] as BroncoSpinResponse).WildCount;
                        spinData.IsLast     = (responses[0] as BroncoSpinResponse).IsLast ? 1 : 0;
                        _responseQueue.Add(spinData);
                    }
                    else if(responses[0] is CashElevatorSpinResponse)
                    {
                        CashElevatorSpinData spinData = new CashElevatorSpinData();
                        spinData.RealOdd = responses[0].RealWin / _defaultBet;
                        spinData.SpinOdd = responses[0].TotalWin / _defaultBet;
                        spinData.Response = responses[0].Response;
                        spinData.SpinType = responses[0].SpinType > 0 ? 1 : 0;
                        spinData.BeginFloor = (responses[0] as CashElevatorSpinResponse).BeginFloor;
                        spinData.EndFloor  = (responses[0] as CashElevatorSpinResponse).EndFloor;
                        _responseQueue.Add(spinData);
                    }
                    else
                    {
                        SpinData spinData = new SpinData();
                        if (responses[0].SpinType == 100)
                        {
                            spinData.RealOdd        = responses[0].RealWin / _defaultBet;
                            spinData.SpinOdd        = responses[0].TotalWin / _defaultBet;
                            spinData.SpinType       = responses[0].SpinType;
                            spinData.Response       = responses[0].Response;
                            spinData.FreeSpinType   = responses[0].FreeSpinType;
                            spinData.FreeSpinTypes  = responses[0].FreeSpinTypes;
                        }
                        else
                        {
                            spinData.RealOdd  = responses[0].RealWin / _defaultBet;
                            spinData.SpinOdd  = responses[0].TotalWin / _defaultBet;
                            spinData.Response = responses[0].Response;
                            spinData.SpinType = responses[0].SpinType > 0 ? 1 : 0;
                            //spinData.SpinType = responses[0].SpinType;
                        }
                        _responseQueue.Add(spinData);
                    }
                }
                else
                {
                    SpinData spinData = null;
                    if (responses[0] is BroncoSpinResponse)
                    {
                        spinData = new BroncoSpinData();
                        spinData.RealOdd        = responses[0].RealWin / _defaultBet;
                        spinData.SpinOdd        = responses[0].TotalWin / _defaultBet;
                        spinData.Response       = responses[0].Response;
                        spinData.SpinType       = responses[0].SpinType;
                        spinData.FreeSpinType   = responses[0].FreeSpinType;

                        (spinData as BroncoSpinData).Wilds      = (responses[0] as BroncoSpinResponse).WildCount;
                        (spinData as BroncoSpinData).IsLast     = (responses[0] as BroncoSpinResponse).IsLast ? 1 : 0;
                        spinData.ChildSpins = new List<SpinData>();
                    }
                    else
                    {
                        spinData                = new SpinData();
                        spinData.RealOdd        = responses[0].RealWin / _defaultBet;
                        spinData.SpinOdd        = responses[0].TotalWin / _defaultBet;
                        spinData.SpinType       = responses[0].SpinType;
                        spinData.Response       = responses[0].Response;
                        spinData.FreeSpinType   = responses[0].FreeSpinType;
                        spinData.FreeSpinTypes  = responses[0].FreeSpinTypes;

                        spinData.ChildSpins = new List<SpinData>();
                    }
                    for (int i = 1; i < responses.Count; i++)
                    {
                        if (responses[i] is BroncoSpinResponse)
                        {
                            BroncoSpinData childSpin = new BroncoSpinData();
                            childSpin.RealOdd   = responses[i].RealWin / _defaultBet;
                            childSpin.SpinOdd   = responses[i].TotalWin / _defaultBet;
                            childSpin.SpinType  = responses[i].SpinType;
                            childSpin.Response  = responses[i].Response;
                            childSpin.Wilds     = (responses[i] as BroncoSpinResponse).WildCount;
                            childSpin.IsLast    = (responses[i] as BroncoSpinResponse).IsLast ? 1 : 0;
                            spinData.ChildSpins.Add(childSpin);
                        }
                        else
                        {
                            SpinData childSpin  = new SpinData();
                            childSpin.RealOdd   = responses[i].RealWin / _defaultBet;
                            childSpin.SpinOdd   = responses[i].TotalWin / _defaultBet;
                            childSpin.SpinType  = responses[i].SpinType;
                            childSpin.Response  = responses[i].Response;
                            spinData.ChildSpins.Add(childSpin);
                        }

                    }
                    _responseQueue.Add(spinData);
                }
            }
        }
        private int getRangeID(double odd)
        {
            for (int i = 0; i < _minOdds.Length; i++)
            {
                if (i == _minOdds.Length - 1)
                {
                    if (odd >= _minOdds[i] && odd <= _maxOdds[i])
                        return i;
                }
                else
                {
                    if (odd >= _minOdds[i] && odd < _maxOdds[i])
                        return i;
                }
            }
            return -1;
        }
        public async Task processQueue(string strGameName, double minOdd, bool isOnlyFree, int mode)
        {
            int totalCount      = 0;
            double totalBet     = 0.0;
            double totalWin     = 0.0;
            int freeSpinCount   = 0;
            double maxOdd       = 0;

            Dictionary<double, bool> spinOdds = new Dictionary<double, bool>();
            SqliteWriter writer = new SqliteWriter();
            await writer.initialize(strGameName);
            if (strGameName == "FiveLionsGold" || strGameName == "TheDogHouseMega" || strGameName == "FiveLions" || strGameName == "WildBooster"
                || strGameName == "FiveLionsMega" || strGameName == "GreatRhinoMega" || strGameName == "AztecKingMega" || strGameName == "EightDragons" ||
                strGameName == "PowerOfThorMega" || strGameName == "KoiPond" || strGameName == "FiveRabbitsMega" || strGameName == "LuckyFishing" ||
                strGameName == "MammothGoldMega" || strGameName == "SpinAndScoreMega" || strGameName == "YumYumPowerWays" ||
                strGameName == "MuertosMultiplierMega" || strGameName == "ChristmasCarolMega" || strGameName == "ExtraJuicyMega" ||
                strGameName == "LegendOfHeroesMega" || strGameName == "MagicianSecrets" || strGameName == "BookOfTheFallen" ||
                strGameName == "DownTheRails" || strGameName == "QueenOfGods" || strGameName == "EyeOfCleopatra" ||
                strGameName == "WildWildRichesMega" || strGameName == "EmperorCaishen" || strGameName == "WildSpells" ||
                strGameName == "FishinReels" || strGameName == "TheWildMachine" || strGameName == "HoneyHoneyHoney" ||
                strGameName == "VampiresVSWolves" || strGameName == "PixieWings" || strGameName == "DragonKingdom" ||
                strGameName == "JourneyToTheWest" || strGameName == "StreetRacer" || strGameName == "RiseOfSamurai" ||
                strGameName == "LadyGodiva" || strGameName == "MightyKong" || strGameName == "TheGreatChickenEscape" ||
                strGameName == "ReleaseTheKraken2" || strGameName == "KingdomOfTheDead" || strGameName == "RobberStrike" ||
                strGameName == "PowerOfMerlinMegaways" || strGameName == "ThreeBuzzingWilds" || strGameName == "RocketBlastMegaways" ||
                strGameName == "CashChips" || strGameName == "ThreeKingdomsBattleOfRedCliffs" || strGameName == "ChaseForGlory" ||
                strGameName == "FiveFrozenCharmsMegaways" || strGameName == "BewareTheDeepMegaways" || strGameName == "BigBassMissionFishin"
                 || strGameName == "MoneyStacks" || strGameName == "VolcanoGoddess")
                await writer.createMultiFreeGameTable();
            else if (strGameName == "BroncoSpirit")
                await writer.createBroncoGameTable();
            else if (strGameName == "CashElevator")
                await writer.createCashElevatorGameTable();
            else if (strGameName == "DragonKingdomEyesOfFire" || strGameName == "EmeraldKingClassic" || strGameName == "WildDepths")
                await writer.createGroupedSpinDataGameTable();
            else if (strGameName == "GoldenBeauty")
                await writer.createSequenceGameTable();
            else
                await writer.createGameTable();

            int[] countsPerRanges = null;
            int   step = 0;

            if (mode == 0)
            {
                countsPerRanges = new int[_minOdds.Length];
            }
            else
            {
                totalCount = await writer.getSpinCount();
                countsPerRanges = await writer.countPerRanges(_minOdds, _maxOdds);
                if (totalCount >= NaturalMaxCount)
                    step = 1;
            }

            int writeCount = 0;
            do
            {
                List<SpinData>                      fetchedResponses                = new List<SpinData>();
                List<BroncoSequence>                fetchedSequences                = new List<BroncoSequence>();
                List<PinupGirlsResponse>            fetchedGirlsResponses           = new List<PinupGirlsResponse>();
                List<GroupedSpinDataResponse>       fetchedGroupedSpinDataResponses = new List<GroupedSpinDataResponse>();
                
                lock (_girlsResponseQueue)
                {
                    for (int i = 0; i < _girlsResponseQueue.Count; i++)
                        fetchedGirlsResponses.Add(_girlsResponseQueue[i]);

                    _girlsResponseQueue.Clear();
                }
                lock(_groupedSpinDataResponseQueue)
                {
                    for (int i = 0; i < _groupedSpinDataResponseQueue.Count; i++)
                        fetchedGroupedSpinDataResponses.Add(_groupedSpinDataResponseQueue[i]);
                    _groupedSpinDataResponseQueue.Clear();
                }
                lock (_responseQueue)
                {
                    for (int i = 0; i < _responseQueue.Count; i++)
                        fetchedResponses.Add(_responseQueue[i]);

                    _responseQueue.Clear();
                }
                lock(_sequenceQueue)
                {
                    for (int i = 0; i < _sequenceQueue.Count; i++)
                        fetchedSequences.Add(_sequenceQueue[i]);
                    _sequenceQueue.Clear();
                }

                if (fetchedResponses.Count == 0 && fetchedSequences.Count == 0 && fetchedGirlsResponses.Count == 0 && fetchedGroupedSpinDataResponses.Count == 0)
                {
                    if (_mustStop)
                        break;
                    await Task.Delay(100);
                    continue;
                }

                List<SpinData> spinDataList = new List<SpinData>();
                for (int i = 0; i < fetchedResponses.Count; i++)
                {
                    totalWin += fetchedResponses[i].SpinOdd * _defaultBet;
                    totalBet += _defaultBet;
                    if (fetchedResponses[i].SpinOdd > maxOdd)
                        maxOdd = fetchedResponses[i].SpinOdd;

                    if (fetchedResponses[i].SpinType == 1 || fetchedResponses[i].SpinType == 100)
                        freeSpinCount++;

                    int rangeID = getRangeID(fetchedResponses[i].SpinOdd);
                   
                    if (mode == 0)
                    {
                        //if (fetchedResponses[i].SpinOdd != 0)
                        //    continue;

                        if (fetchedResponses[i].SpinOdd < minOdd)
                            continue;

                        if (isOnlyFree)
                        {
                            if (fetchedResponses[i].ChildSpins == null && fetchedResponses[i].SpinType == 0)
                                continue;
                        }
                    }
                    else
                    {
                        if (step == 1)
                        {
                            if (rangeID >= 0 && countsPerRanges[rangeID] >= _maxCounts[rangeID])
                                continue;

                            if (rangeID < 0)
                                continue;
                        }
                    }

                    totalCount++;
                    spinDataList.Add(fetchedResponses[i]);
                    if (fetchedResponses[i].ChildSpins != null && fetchedResponses[i].ChildSpins.Count > 0)
                    {
                        for (int j = 0; j < fetchedResponses[i].ChildSpins.Count; j++)
                            spinDataList.Add(fetchedResponses[i].ChildSpins[j]);
                    }

                    if (!spinOdds.ContainsKey(fetchedResponses[i].SpinOdd))
                        spinOdds.Add(fetchedResponses[i].SpinOdd, true);

                    if (rangeID >= 0)
                        countsPerRanges[rangeID]++;
                }
                if (spinDataList.Count > 0)
                    await writer.writeSpinData(new WriteSpinDataRequest(spinDataList));

                if(fetchedSequences.Count > 0 && minOdd == 0.0)
                    await writer.writeSequenceData(fetchedSequences);

                if(fetchedGirlsResponses.Count > 0)
                    await writer.writeGirlsResponse(fetchedGirlsResponses);
                if(fetchedGroupedSpinDataResponses.Count > 0)
                    await writer.writeGroupedSpinDataResponse(fetchedGroupedSpinDataResponses);

                writeCount += fetchedResponses.Count;
                if(writeCount >= 100)
                {
                    Console.WriteLine(string.Format("{0}: totalCount: {1} totalBet:{2}, totalWin: {3}, maxOdd: {4}, freeCount: {5}, total Odds Count: {6}", DateTime.Now, totalCount, Math.Round(totalBet, 2),
                        Math.Round(totalWin, 2), Math.Round(maxOdd, 2), freeSpinCount, spinOdds.Count));

                    Console.WriteLine(string.Format("{5}: 50-100:{0}, 100-300:{1}, 300-500:{2}, 500-1000:{3}, 1000-3000:{4}",
                         countsPerRanges[0], countsPerRanges[1], countsPerRanges[2], countsPerRanges[3], countsPerRanges[4], DateTime.Now));

                    writeCount = 0;
                }
                if (mode == 1)
                {
                    if (step == 0)
                    {
                        if (totalCount >= NaturalMaxCount)
                            step = 1;
                    }
                    else
                    {
                        bool isComplete = true;
                        for (int i = 0; i < countsPerRanges.Length; i++)
                        {
                            if (countsPerRanges[i] < _minCounts[i])
                            {
                                isComplete = false;
                                break;
                            }
                        }
                        if (isComplete)
                        {
                            Console.WriteLine("Completed");
                            break;
                        }
                    }
                }
                if (_mustStop)
                    break;
            } while (true);
        }
    }
}
