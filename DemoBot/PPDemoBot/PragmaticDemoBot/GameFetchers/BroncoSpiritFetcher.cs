using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    public class BroncoSpiritFetcher : GameSpinDataFetcher
    {
        public BroncoSpiritFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }

        protected override int FreeSpinOptionCount
        {
            get { return 2;  }
        }
        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            if (!dicParams.ContainsKey("fs_opt"))
                return 100;

            string strValue = dicParams["fs_opt"].Split(new string[] { "~" }, StringSplitOptions.None)[0];
            if (strValue == "15,1,4")
                return 0;
            else if (strValue == "30,1,4")
                return 1;
            else if (strValue == "45,1,4")
                return 2;

            return 100;
        }
        protected override async Task<DoSpinsResults> doSpins(HttpClient httpClient, string strToken)
        {
            int count = 0;
            do
            {
                int prevWildCount = 0;
                List<List<SpinResponse>> responseList = new List<List<SpinResponse>>();
                for(int i = 1; i <= 10; i++)
                {
                    List<SpinResponse> responses = await doSpin(httpClient, strToken, i, prevWildCount);
                    if (responses == null)
                        return DoSpinsResults.NEEDRESTARTSESSION;

                    responseList.Add(responses);
                    await Task.Delay(500);
                    prevWildCount += (responses[0] as BroncoSpinResponse).WildCount;
                }
                BroncoSequence broncoSequence = new BroncoSequence();
                broncoSequence.WildCounts   = new List<int>();
                broncoSequence.Odds         = new List<double>();
                int totalWildCount = 0;
                for (int i = 0; i < 10; i++)
                {
                    SpinDataQueue.Instance.addSpinDataToQueue(responseList[i]);
                    totalWildCount += (responseList[i][0] as BroncoSpinResponse).WildCount;
                    if(i == 9)
                        broncoSequence.WildCounts.Add(totalWildCount);
                    else
                        broncoSequence.WildCounts.Add((responseList[i][0] as BroncoSpinResponse).WildCount);

                    broncoSequence.Odds.Add((responseList[i][0] as BroncoSpinResponse).TotalWin / _realBet);
                }                
                SpinDataQueue.Instance.addBroncoSequence(broncoSequence);

                count++;
                if (count >= 500)
                    return DoSpinsResults.NEEDRESTARTSESSION;

                if (_mustStop)
                    break;

            } while (true);
            return DoSpinsResults.USERSTOPPED;
        }

        protected BroncoAccumInfo parseAccumInfo(SortedDictionary<string, string> dicParams)
        {
            if (!dicParams.ContainsKey("accv"))
                return null;

            string[] strParts = dicParams["accv"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            return new BroncoAccumInfo(int.Parse(strParts[0]), int.Parse(strParts[3]));
        }
        protected async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken, int index, int prevWildCount)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinResponse> responseList = new List<SpinResponse>();
            string strResponse = "";

            try
            {
                strResponse                                     = await sendSpinRequest(httpClient, strToken);
                SortedDictionary<string, string> dicParamValues = splitAndRemoveCommonResponse(strResponse);
                string strNextAction                            = dicParamValues["na"];

                strResponseHistory.Add(combineResponse(dicParamValues));
                BroncoAccumInfo accumInfo = parseAccumInfo(dicParamValues);
                if (strNextAction == "c")
                {
                    await doCollect(httpClient, strToken);

                    BroncoSpinResponse response = new BroncoSpinResponse();
                    response.SpinType   = 0;
                    response.TotalWin   = double.Parse(dicParamValues["tw"]);
                    response.Response   = string.Join("\n", strResponseHistory);
                    response.IsLast     = (accumInfo.Index == 10);
                    response.WildCount  = accumInfo.TotalWildCount - prevWildCount;
                    responseList.Add(response);
                    return responseList;
                }
                else if (strNextAction == "s" && !isFreeOrBonus(dicParamValues))
                {
                    //윈값이 0인 경우
                    BroncoSpinResponse  response    = new BroncoSpinResponse();
                    response.SpinType   = 0;
                    response.TotalWin   = double.Parse(dicParamValues["tw"]);
                    response.Response   = string.Join("\n", strResponseHistory);
                    response.IsLast     = (accumInfo.Index == 10);
                    response.WildCount  = accumInfo.TotalWildCount - prevWildCount;
                    responseList.Add(response);
                    return responseList;
                }

                //프리스핀이나 보너스로 이행한다.
                double  beforeFreeTotalWin = 0.0;
                int     selectedFreeOption = -1;
                do
                {
                    if (strNextAction == "fso")
                    {
                        if(selectedFreeOption != -1)
                        {
                            Console.WriteLine("Multi FreeSpin Found");
                            return null;
                        }
                        int freeSpinType    = findFreeSpinType(dicParamValues);
                        double totalWin     = double.Parse(dicParamValues["tw"]);

                        BroncoSpinResponse   response   = new BroncoSpinResponse();
                        response.SpinType       = 100;
                        response.Response       = string.Join("\n", strResponseHistory.ToArray());
                        response.TotalWin       = totalWin;
                        response.RealWin        = totalWin;
                        response.FreeSpinType   = freeSpinType;
                        response.IsLast         = (accumInfo.Index == 10);
                        response.WildCount      = accumInfo.TotalWildCount - prevWildCount;

                        strResponseHistory.Clear();
                        responseList.Add(response);

                        beforeFreeTotalWin = totalWin;
                        int freeSpinOption = selectFreeOption(freeSpinType);
                        selectedFreeOption = 200 + freeSpinType * FreeSpinOptionCount + freeSpinOption;
                        strResponse        = await doFreeSpinOption(httpClient, strToken, freeSpinOption);
                        dicParamValues     = splitAndRemoveCommonResponse(strResponse);
                        strNextAction      = dicParamValues["na"];
                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    else if (strNextAction == "s")
                    {
                        strResponse = await sendSpinRequest(httpClient, strToken);
                        dicParamValues = splitAndRemoveCommonResponse(strResponse);
                        strNextAction = dicParamValues["na"];

                        strResponseHistory.Add(combineResponse(dicParamValues, beforeFreeTotalWin));
                    }
                    
                    else if (strNextAction == "c")
                    {
                        await doCollect(httpClient, strToken);
                        BroncoSpinResponse  response    = new BroncoSpinResponse();
                        response.SpinType               = selectedFreeOption;
                        response.TotalWin               = double.Parse(dicParamValues["tw"]) - beforeFreeTotalWin;
                        response.Response               = string.Join("\n", strResponseHistory.ToArray());
                        response.IsLast                 = (accumInfo.Index == 10);
                        response.WildCount              = accumInfo.TotalWildCount - prevWildCount;
                        responseList.Add(response);
                        responseList[0].TotalWin = double.Parse(dicParamValues["tw"]);
                        return responseList;
                    }

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                return null;
            }
        }
    }

    public class BroncoAccumInfo
    {
        public int Index                { get; set; }
        public int TotalWildCount       { get; set; }

        public BroncoAccumInfo(int index, int wildCount)
        {
            this.Index      = index;
            this.TotalWildCount  = wildCount;
        }
    }

    public class BroncoSequence
    {
        public List<double> Odds        { get; set; }
        public List<int>    WildCounts  { get; set; }

        public double getTotalOdd()
        {
            double totalOdd = 0.0;
            for (int i = 0; i < Odds.Count; i++)
                totalOdd += Odds[i];
            return Math.Round(totalOdd, 2);
        }

        public string toString()
        {
            List<string> stringParts = new List<string>();
            for(int i = 0; i < 10; i++)
            {
                stringParts.Add(string.Format("{0}~{1}", Math.Round(Odds[i], 2), WildCounts[i]));
            }
            return string.Join(",", stringParts.ToArray());
        }
    }
     
    public class BroncoResponseList
    {
        public List<SpinResponse> Responses { get; set; }

        public int                WildCount { get; set; }

        public BroncoResponseList(List<SpinResponse> responseList, int wildCount)
        {
            this.Responses = responseList;
            this.WildCount = wildCount;
        }
    }
}
