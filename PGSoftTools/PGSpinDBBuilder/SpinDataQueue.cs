using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGSpinDBBuilder
{
    public class SpinDataQueue
    {
        private static SpinDataQueue _sInstance = new SpinDataQueue();

        private double[]    _minOdds        = new double[] { 50, 100, 300, 500, 1000 };
        private double[]    _maxOdds        = new double[] { 100, 300, 500, 1000, 3000 };
        private int[]       _minCounts      = new int[] { 150, 100, 80, 60, 30 };
        private int[]       _maxCounts      = new int[] { 300, 300, 200, 100, 100 };
        private const int   NaturalMaxCount = 200000;

        public static SpinDataQueue Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public List<SpinData>               _responseQueue          = new List<SpinData>();
        public List<GroupSpinData>          _groupResponseQueue     = new List<GroupSpinData>();
        private bool                        _mustStop               = false;
        private double                      _defaultBet             = 0.0;
        public void doStop()
        {
            _mustStop = true;
        }
        public void setDefaultBet(double defaultBet)
        {
            _defaultBet = defaultBet;
        }
        public void addSpinDataToQueue(GroupSpinResponse groupResponse)
        {
            lock(_groupResponseQueue)
            {
                GroupSpinData spinData = new GroupSpinData();
                spinData.SpinCount = groupResponse.ResponseList.Count;
                spinData.SpinOdd   = groupResponse.TotalWin / (_defaultBet * spinData.SpinCount);
                spinData.Response  = groupResponse.Response;
                _groupResponseQueue.Add(spinData);
            }
        }
        public void addSpinDataToQueue(List<SpinResponse> responses)
        {
            lock (_responseQueue)
            {
                if(responses.Count == 1)
                {                    
                    SpinData spinData = new SpinData();
                    spinData.RealOdd = responses[0].RealWin / _defaultBet;
                    spinData.SpinOdd = responses[0].TotalWin / _defaultBet;
                    spinData.Response = responses[0].Response;
                    spinData.SpinType = responses[0].SpinType;
                    _responseQueue.Add(spinData);
                }
                else
                {
                    SpinData spinData = null;

                    spinData = new SpinData();
                    spinData.RealOdd = responses[0].RealWin / _defaultBet;
                    spinData.SpinOdd = responses[0].TotalWin / _defaultBet;
                    spinData.SpinType = responses[0].SpinType;
                    spinData.Response = responses[0].Response;
                    spinData.FreeSpinType = responses[0].FreeSpinType;
                    spinData.ChildSpins = new List<SpinData>();
                    for (int i = 1; i < responses.Count; i++)
                    {
                        SpinData childSpin = new SpinData();
                        childSpin.RealOdd = responses[i].RealWin / _defaultBet;
                        childSpin.SpinOdd = responses[i].TotalWin / _defaultBet;
                        childSpin.SpinType = responses[i].SpinType;
                        childSpin.Response = responses[i].Response;
                        spinData.ChildSpins.Add(childSpin);
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

        public async Task processQueue(string strGameName, double minOdd, double maxOddCondition, bool isOnlyFree, int mode)
        {
            int totalCount      = 0;
            double totalBet     = 0.0;
            double totalWin     = 0.0;
            int freeSpinCount   = 0;
            double maxOdd       = 0;

            Dictionary<double, bool> spinOdds = new Dictionary<double, bool>();
            SqliteWriter writer = new SqliteWriter();
            await writer.initialize(strGameName);
            if (strGameName == "EgyptBookOfMystery" || strGameName == "QueenOfBounty" || strGameName == "ShaolinSoccer" ||
                strGameName == "SecretOfCleopatra" || strGameName == "Genies3Wishes" || strGameName == "HoneyTrapOfDiaoChan" || strGameName == "CaishenWins")
                await writer.createMultiFreeGameTable();
            else
               await writer.createGameTable();

            int[] countsPerRanges = null;
            int step = 0;

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
            List<double> freeSpinOdds = await writer.getFreeSpinOdds();

            int writeCount = 0;
            do
            {
                List<SpinData>          fetchedResponses = new List<SpinData>();

                lock (_responseQueue)
                {
                    for (int i = 0; i < _responseQueue.Count; i++)
                        fetchedResponses.Add(_responseQueue[i]);

                    _responseQueue.Clear();
                }
                if (fetchedResponses.Count == 0)
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

                        if (fetchedResponses[i].SpinOdd < minOdd)
                            continue;

                        if (fetchedResponses[i].ChildSpins != null && fetchedResponses[i].ChildSpins.Count > 0)
                        {
                            if (fetchedResponses[i].ChildSpins[0].SpinOdd < minOdd)
                                continue;
                        }
                        if (maxOddCondition >= 0.0 && fetchedResponses[i].SpinOdd > maxOddCondition)
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
                    
                    if(fetchedResponses[i].ChildSpins != null && fetchedResponses[i].ChildSpins.Count > 0)
                    {
                        for (int j = 0; j < fetchedResponses[i].ChildSpins.Count; j++)
                        {
                            totalWin += fetchedResponses[i].ChildSpins[j].SpinOdd * _defaultBet;
                            spinDataList.Add(fetchedResponses[i].ChildSpins[j]);
                        }
                    }
                   
                    if (!spinOdds.ContainsKey(fetchedResponses[i].SpinOdd))
                        spinOdds.Add(fetchedResponses[i].SpinOdd, true);

                    if (rangeID >= 0)
                        countsPerRanges[rangeID]++;

                    if (fetchedResponses[i].SpinType == 1)
                        freeSpinOdds.Add(fetchedResponses[i].SpinOdd);
                }
                if (spinDataList.Count > 0)
                    await writer.writeSpinData(new WriteSpinDataRequest(spinDataList));

                writeCount += fetchedResponses.Count;
                if(writeCount >= 50)
                {
                    Console.WriteLine(string.Format("{0}: totalCount: {1} totalBet:{2}, totalWin: {3}, maxOdd: {4}, freeCount: {5}, total Odds Count: {6}", DateTime.Now, totalCount, Math.Round(totalBet, 2),
                        Math.Round(totalWin, 2), Math.Round(maxOdd, 2), freeSpinCount, spinOdds.Count));

                    Console.WriteLine(string.Format("{5}: 50-100:{0}, 100-300:{1}, 300-500:{2}, 500-1000:{3}, 1000-3000:{4}",
                         countsPerRanges[0], countsPerRanges[1], countsPerRanges[2], countsPerRanges[3], countsPerRanges[4], DateTime.Now));

                    double sumOdd = 0.0;
                    for (int i = 0; i < freeSpinOdds.Count; i++)
                        sumOdd += freeSpinOdds[i];

                    if(freeSpinOdds.Count > 0)
                        Console.WriteLine(string.Format("Free Spin Count:{0} Mean Odd:{1}", freeSpinOdds.Count, Math.Round(sumOdd / freeSpinOdds.Count, 2)));
                    writeCount = 0;
                }
                if (mode == 1)
                {
                    if (step == 0)
                    {
                        if (totalCount >= NaturalMaxCount)
                        {
                            step = 1;
                            break;
                        }
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
        public async Task processGroupQueue(string strGameName, double minOdd, double maxOddCondition, bool isOnlyFree, int mode)
        {
            int totalCount = 0;
            double totalBet = 0.0;
            double totalWin = 0.0;
            int freeSpinCount = 0;
            double maxOdd = 0;

            Dictionary<double, bool> spinOdds = new Dictionary<double, bool>();
            SqliteWriter writer = new SqliteWriter();
            await writer.initialize(strGameName);
            await writer.createGroupGameTable();

            int[] countsPerRanges = null;
            int step = 0;

            if (mode == 0)
            {
                countsPerRanges = new int[_minOdds.Length];
            }
            else
            {
                totalCount = 0;
                countsPerRanges = new int[_minOdds.Length];
                if (totalCount >= NaturalMaxCount)
                    step = 1;
            }
            List<double> freeSpinOdds = new List<double>();
            int writeCount = 0;
            do
            {
                List<GroupSpinData> fetchedResponses = new List<GroupSpinData>();
                lock (_groupResponseQueue)
                {
                    for (int i = 0; i < _groupResponseQueue.Count; i++)
                        fetchedResponses.Add(_groupResponseQueue[i]);

                    _groupResponseQueue.Clear();
                }
                if (fetchedResponses.Count == 0)
                {
                    if (_mustStop)
                        break;
                    await Task.Delay(100);
                    continue;
                }

                List<GroupSpinData> spinDataList = new List<GroupSpinData>();
                for (int i = 0; i < fetchedResponses.Count; i++)
                {
                    totalWin += fetchedResponses[i].SpinOdd * _defaultBet * fetchedResponses[i].SpinCount;
                    totalBet += _defaultBet * fetchedResponses[i].SpinCount;

                    if (fetchedResponses[i].SpinOdd > maxOdd)
                        maxOdd = fetchedResponses[i].SpinOdd;

                    int rangeID = getRangeID(fetchedResponses[i].SpinOdd);
                    if (mode == 0)
                    {
                        if (fetchedResponses[i].SpinOdd < minOdd)
                            continue;

                        if (maxOddCondition >= 0.0 && fetchedResponses[i].SpinOdd > maxOddCondition)
                            continue;
                        
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

                    if (!spinOdds.ContainsKey(fetchedResponses[i].SpinOdd))
                        spinOdds.Add(fetchedResponses[i].SpinOdd, true);

                    if (rangeID >= 0)
                        countsPerRanges[rangeID]++;
                }
                if (spinDataList.Count > 0)
                    await writer.writeGroupSpinData(new WriteGroupSpinDataRequest(spinDataList));

                writeCount += fetchedResponses.Count;
                if (writeCount >= 100)
                {
                    Console.WriteLine(string.Format("{0}: totalCount: {1} totalBet:{2}, totalWin: {3}, maxOdd: {4}, freeCount: {5}, total Odds Count: {6}", DateTime.Now, totalCount, Math.Round(totalBet, 2),
                        Math.Round(totalWin, 2), Math.Round(maxOdd, 2), freeSpinCount, spinOdds.Count));

                    Console.WriteLine(string.Format("{5}: 50-100:{0}, 100-300:{1}, 300-500:{2}, 500-1000:{3}, 1000-3000:{4}",
                         countsPerRanges[0], countsPerRanges[1], countsPerRanges[2], countsPerRanges[3], countsPerRanges[4], DateTime.Now));

                    double sumOdd = 0.0;
                    for (int i = 0; i < freeSpinOdds.Count; i++)
                        sumOdd += freeSpinOdds[i];

                    if (freeSpinOdds.Count > 0)
                        Console.WriteLine(string.Format("Free Spin Count:{0} Mean Odd:{1}", freeSpinOdds.Count, Math.Round(sumOdd / freeSpinOdds.Count, 2)));
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
