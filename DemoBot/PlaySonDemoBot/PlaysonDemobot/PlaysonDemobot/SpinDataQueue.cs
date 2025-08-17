using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaysonDemobot
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

        public List<SpinData> _responseQueue = new List<SpinData>();

        private bool    _mustStop   = false;
        private double  _defaultBet = 0.0;
        
        public void doStop()
        {
            _mustStop = true;
        }
        
        public void setDefaultBet(double defaultBet)
        {
            _defaultBet = defaultBet;
        }
        
        public void addSpinDataToQueue(List<SpinData> responses)
        {
            lock (_responseQueue)
            {
                if (responses.Count == 1)
                {
                    SpinData spinData = new SpinData();
                    spinData.RealOdd    = responses[0].RealOdd;
                    spinData.SpinOdd    = responses[0].SpinOdd;
                    spinData.Response   = responses[0].Response;
                    spinData.SpinType   = responses[0].SpinType > 0 ? 1 : 0;
                    //_responseQueue.Add(spinData);
                    _responseQueue.Add(responses[0]);
                }
                else
                {
                    SpinData spinData = null;
                    spinData = new SpinData();
                    spinData.RealOdd        = responses[0].RealOdd;
                    spinData.SpinOdd        = responses[0].SpinOdd;
                    spinData.SpinType       = responses[0].SpinType;
                    spinData.Response       = responses[0].Response;
                    spinData.FreeSpinType   = responses[0].FreeSpinType;
                    
                    //_responseQueue.Add(spinData);
                    _responseQueue.Add(responses[0]);

                    for (int i = 1; i < responses.Count; i++)
                    {
                        SpinData childSpin = new SpinData();
                        childSpin.RealOdd   = responses[i].RealOdd;
                        childSpin.SpinOdd   = responses[i].SpinOdd;
                        childSpin.SpinType  = responses[i].SpinType;
                        childSpin.Response  = responses[i].Response;
                        //_responseQueue.Add(childSpin);
                        _responseQueue.Add(responses[i]);
                    }
                }
            }
        }

        public async Task processQueue(string strGameName, double minOdd, bool isOnlyFree)
        {
            int totalCount      = 0;
            double totalBet     = 0.0;
            double totalWin     = 0.0;
            int freeSpinCount   = 0;
            double maxOdd       = 0;

            Dictionary<double, bool> spinOdds = new Dictionary<double, bool>();
            SqliteWriter writer = new SqliteWriter();
            await writer.initialize(strGameName);
            if (strGameName == "BookOfGoldChoice" || strGameName == "CloverRiches")
                await writer.createMultiFreeGameTable();
            else
                await writer.createGameTable();

            int writeCount = 0;
            do
            {
                List<SpinData> fetchedResponses = new List<SpinData>();

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

                    if (fetchedResponses[i].SpinOdd < minOdd)
                        continue;

                    if (isOnlyFree)
                    {
                        if (fetchedResponses[i].ChildSpins == null && fetchedResponses[i].SpinType == 0)
                            continue;
                    }

                    totalCount++;
                    spinDataList.Add(fetchedResponses[i]);
                    
                    if (!spinOdds.ContainsKey(fetchedResponses[i].SpinOdd))
                        spinOdds.Add(fetchedResponses[i].SpinOdd, true);

                }
                if (spinDataList.Count > 0)
                    await writer.writeSpinData(new WriteSpinDataRequest(spinDataList));

                writeCount += fetchedResponses.Count;
                if (writeCount >= 100)
                {
                    Console.WriteLine(string.Format("{0}: totalCount: {1} totalBet:{2}, totalWin: {3}, maxOdd: {4}, freeCount: {5}, total Odds Count: {6}", DateTime.Now, totalCount, Math.Round(totalBet, 2),
                        Math.Round(totalWin, 2), Math.Round(maxOdd, 2), freeSpinCount, spinOdds.Count));

                    writeCount = 0;
                }

                if (_mustStop)
                    break;
            } while (true);
        }
    }
}
