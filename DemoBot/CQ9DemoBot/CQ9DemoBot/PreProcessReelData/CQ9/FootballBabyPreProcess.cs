using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PreProcessReelData
{
    public class FootballBabyPreProcess : SpinDataPreProcess
    {
        protected int[][]  _winLines = new int[][] {
                new int[] {1, 1, 1, 1, 1},
                new int[] {0, 0, 0, 0, 0},
                new int[] {2, 2, 2, 2, 2},
                new int[] {0, 1, 2, 1, 0},
                new int[] {2, 1, 0, 1, 2},
                new int[] {0, 0, 1, 2, 2},
                new int[] {2, 2, 1, 0, 0},
                new int[] {1, 0, 1, 2, 1},
                new int[] {1, 2, 1, 0, 1},
            };

        protected int[][] _reelZokbos = new int[][] {
                new int[] { 0,  10, 200, 800, 5000 },
                new int[] { 0,  5,  80,  200, 600  },
                new int[] { 0,  5,  25,  100, 500  },
                new int[] { 0,  0,  25,  80,  300  },
                new int[] { 0,  0,  25,  80,  250  },
                new int[] { 0,  0,  0,   0,   0    },
                new int[] { 0,  0,  0,   0,   0    },
                new int[] { 0,  0,  0,   0,   0    },
                new int[] { 0,  0,  0,   0,   0    },
                new int[] { 0,  0,  0,   0,   0    },
                new int[] { 0,  0,  20,  50,  150  },
                new int[] { 0,  0,  15,  30,  120  },
                new int[] { 0,  0,  15,  30,  120  },
                new int[] { 0,  0,  10,  25,  100  },
                new int[] { 0,  0,  10,  25,  100  },
            };

        protected int[] _scatterZokbo = new int[] { 0, 0, 18, 45, 450 };

        protected string[][] _reelColumns = new string[][] {
            new string[] { "13","14","SC","11","11","12","5","3","14","5","3","13","13","4","15","3","12","13","4","13","5","14","14","11","14","5","12","11","11","15","SC","15","3","14","12","12","15","15","2","1","12","12","15","3"},
            new string[] { "15","12","2","4","13","2","3","11","12","12","5","15","15","W","W","13","W","14","14","12","5","14","15","15","3","1","3","SC","2","3","11","11","3","14","4","15","12","15","12","4","13","13","12","5","15","15","SC","14","14"},
            new string[] { "12","SC","12","2","11","5","11","3","2","15","15","W","W","W","14","14","3","12","12","SC","2","1","15","3","1","5","13","13","4","14","14","3","15","15","4","5","15","3","12","12","5","14","12" },
            new string[] { "12","5","11","SC","5","W","W","11","W","12","13","3","13","11","1","12","1","1","13","3","4","13","3","4","13","14","14","12","4","2","11","W","2","13","15","W","4","12","SC","3","14","3","11","1","5","12","2","2","13","3","4","5","3" },
            new string[] { "W","W","14","W","15","15","1","12","12","1","11","12","5","4","4","12","11","2","SC","15","2","13","12","13","12","14","SC","15","14","4","3","14","2","15","2","2","14","15","1","1","15","1","14","15" },
        };
        protected string _resultTemplate    = "{\"ID\":131,\"RngData\":[39,14,13,6,1],\"SymbolResult\":[\"15,15,W,5,15\",\"2,W,W,W,W\",\"1,W,W,W,W\"],\"WinType\":1,\"BaseWin\":11090,\"TotalWin\":11090,\"IsTriggerFG\":false,\"NextModule\":0,\"WinLineCount\":8,\"ExtraDataCount\":1,\"ExtraData\":[0],\"ReellPosChg\":[0],\"BonusType\":0,\"SpecialAward\":0,\"SpecialSymbol\":0,\"ReelLenChange\":[],\"ReelPay\":[],\"IsRespin\":false,\"RespinReels\":[0,0,0,0,0],\"Multiple\":\"0\",\"NextSTable\":0,\"FreeSpin\":[0],\"IsHitJackPot\":false,\"udsOutputWinLine\":[]}";
        protected string _winTemplate       = "{\"SymbolId\":\"2\",\"LinePrize\":600,\"NumOfKind\":5,\"SymbolCount\":5,\"WinLineNo\":0,\"LineMultiplier\":1,\"WinPosition\":[[0,0,0,0,0],[1,2,2,2,2],[0,0,0,0,0]],\"LineExtraData\":[0],\"LineType\":0}";
        
        public FootballBabyPreProcess()
        {
        }
        
        public override async Task startPreProcess(SqliteDatabaseWork databaseWork,string strGameName)
        {
            int minOdd = 300, maxOdd = 500;
            _strGameSymbol = strGameName;
            await databaseWork.initialize(_strGameSymbol);
            try
            {
                List<SpinData> resultSpinData = new List<SpinData>();

                dynamic resultTemplate = JsonConvert.DeserializeObject<dynamic>(_resultTemplate);
                List<int[]> rngStopList = getRngStopList();
                for(int i = 0; i < rngStopList.Count; i++)
                {
                    string[][] symbolStatus     = getSymbolResultFromRng(rngStopList[i]);
                    List<winResult> winInfos    = getWinLineResults(symbolStatus);
                    int winTotalMoney = 0;
                    for (int j = 0; j < winInfos.Count; j++)
                        winTotalMoney += winInfos[j].winMoney;

                    if (winTotalMoney < 9 * minOdd || winTotalMoney > 9 * maxOdd)
                        continue;

                    JArray RngData = new JArray();
                    for(int j = 0; j < rngStopList[i].Length; j++)
                        RngData.Add(rngStopList[i][j]);
                    resultTemplate["RngData"] = RngData;

                    JArray SymbolArray = new JArray();

                    for (int j = 0; j < 3; j++)
                    {
                        JArray row = new JArray();
                        for(int k = 0; k < 5; k++)
                            row.Add(symbolStatus[j][k]);
                        
                        SymbolArray.Add(string.Join(",",row));
                    }
                    resultTemplate["SymbolResult"] = SymbolArray;

                    JArray udsOutputWinLine = new JArray();
                    for (int j = 0; j < winInfos.Count; j++)
                        udsOutputWinLine.Add(JsonConvert.DeserializeObject<dynamic>(winInfos[j].winString));
                    resultTemplate["udsOutputWinLine"] = udsOutputWinLine;

                    resultTemplate["BaseWin"]       = winTotalMoney;
                    resultTemplate["TotalWin"]      = winTotalMoney;
                    resultTemplate["WinLineCount"]  = winInfos.Count;

                    string newResult = JsonConvert.SerializeObject(resultTemplate);

                    resultSpinData.Add(new SpinData()
                    {
                        SpinType    = 0,
                        SpinOdd     = Math.Round(winTotalMoney / 9.0, 2),
                        Data        = newResult 
                    });
                }

                Console.WriteLine("{0} ~ {1} : {2}", minOdd, maxOdd, resultSpinData.Count);
                for(int i = 0; i < resultSpinData.Count; i++)
                {
                    await databaseWork.insertGeneratedSpinData(resultSpinData[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error has been occured in game {0}", strGameName));
            }

            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }

        private List<int[]> getRngStopList()
        {
            int[] reelsetLengths = new int[] { 0, 0, 0, 0, 0 };
            for (int i = 0; i < 5; i++)
                reelsetLengths[i] = _reelColumns[i].Length;

            List<int[]> rngNoList = new List<int[]>();
            for (int i = 0; i < 20000000; i++)
            {
                int[] rngNos = new int[] { 1, 1, 1, 1, 1 };
                for (int j = 0; j < 5; j++)
                {
                    Random rnd      = new Random(Guid.NewGuid().GetHashCode());
                    int stopIndex   = rnd.Next(0, _reelColumns[j].Length);
                    rngNos[j]       = stopIndex + 1;
                }
                rngNoList.Add(rngNos);
            }

            return rngNoList;
        }

        private string[][] getSymbolResultFromRng(int[] rngNos)
        {
            string[][] symbolResult = new string[][]
            {
                new string[]{"0", "0", "0", "0", "0" },
                new string[]{"0", "0", "0", "0", "0" },
                new string[]{"0", "0", "0", "0", "0" },
            };

            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    int index = (rngNos[i] + _reelColumns[i].Length - 2 + j) % _reelColumns[i].Length;
                    symbolResult[j][i] = _reelColumns[i][index];
                }
            }

            return symbolResult;
        }

        private string getWinInfoFromLine(string symbol, int multiplier,int repeatCnt,int lineNo,string[][] symbolStatus)
        {
            dynamic template = JsonConvert.DeserializeObject<dynamic>(_winTemplate);
            template["SymbolId"]    = symbol;
            template["LinePrize"]   = multiplier;
            template["NumOfKind"]   = repeatCnt;
            template["SymbolCount"] = repeatCnt;
            template["WinLineNo"]   = lineNo;

            for(int i = 0; i < symbolStatus.Length; i++)
            {
                for(int j = 0; j < symbolStatus[i].Length; j++)
                {
                    template["WinPosition"][i][j] = 0;

                }
            }
            
            if (symbol != "SC")
            {
                for (int i = 0; i < _winLines[lineNo].Length; i++)
                {
                    int rowIndex = _winLines[lineNo][i];

                    if (symbol == symbolStatus[rowIndex][i])
                        template["WinPosition"][rowIndex][i] = 1;
                    else if (symbolStatus[rowIndex][i] == "W")
                        template["WinPosition"][rowIndex][i] = 2;
                    else
                        break;
                }
            }
            else
            {
                for(int i = 0; i < symbolStatus.Length; i++)
                {
                    for(int j = 0; j < symbolStatus[i].Length; j++)
                    {
                        if (symbol == symbolStatus[i][j])
                            template["WinPosition"][i][j] = 1;
                    }
                }
            }

            return JsonConvert.SerializeObject(template);
        }

        private List<winResult> getWinLineResults(string[][] symbolStatus)
        {
            List<winResult> winResults = new List<winResult>();
            try
            {
                int scatterCnt = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        string symbol = symbolStatus[j][i];
                        if (symbol == "SC")
                            scatterCnt++;
                    }
                }

                string[][] lineSymbols = getLineSymbols(symbolStatus);
                for (int i = 0; i < lineSymbols.Length; i++)
                {
                    string firstSymbol = lineSymbols[i][0];
                    if (firstSymbol == "SC")
                        continue;

                    int repeatCount = 1;
                    for (int j = 1; j < 5; j++)
                    {
                        if (firstSymbol != lineSymbols[i][j] && lineSymbols[i][j] != "W")
                            break;
                        repeatCount++;
                    }

                    int multiplier = _reelZokbos[Convert.ToInt32(firstSymbol) - 1][repeatCount - 1];
                    if (multiplier != 0)
                    {
                        string winInfo = getWinInfoFromLine(firstSymbol, multiplier, repeatCount, i, symbolStatus);
                        winResults.Add(new winResult() { winMoney = multiplier, winString = winInfo });
                    }
                }

                if (scatterCnt >= 3)
                {
                    string winInfo = getWinInfoFromLine("SC", scatterCnt, scatterCnt, 998, symbolStatus);
                    winResults.Add(new winResult() { winMoney = _scatterZokbo[scatterCnt - 1], winString = winInfo });
                }
            }
            catch (Exception ex)
            {

            }
            return winResults;
        }

        private string[][] getLineSymbols(string[][] symbolStatus)
        {
            string[][] lineSymbols = new string[_winLines.Length][];

            for (int i = 0; i < _winLines.Length; i++)
            {
                lineSymbols[i] = new string[5];
                for (int j = 0; j < 5; j++)
                {
                    int rowIndex        = _winLines[i][j];
                    string symbol       = symbolStatus[rowIndex][j];
                    lineSymbols[i][j]   = symbol;
                }
            }
            return lineSymbols;
        }

    }

    public class winResult
    {
        public int      winMoney      { get; set; }
        public string   winString     { get; set; }  
    }
}
