using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using PreProcessReelData.Amatic;

namespace PreProcessReelData
{
    public class LuckyZodiacPreProcess : SpinDataPreProcess
    {
        public LuckyZodiacPreProcess()
        {
        }
        public override async Task startPreProcess(SqliteDatabaseWork databaseWork,string strGameName, int cols, int freecols)
        {
            await base.startPreProcess(databaseWork, strGameName, cols, freecols);
            
            List<FreeOptionSpinData> startSpinData  = new List<FreeOptionSpinData>();
            List<FreeOptionSpinData>[] freeSpinDatas  = new List<FreeOptionSpinData>[]{
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
                new List<FreeOptionSpinData>(),
            };

            for (int i = 0; i < _FreeSpinDataList.Count; i++)
            {
                if (_FreeSpinDataList[i].SpinType == 100)
                    startSpinData.Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 201)
                    freeSpinDatas[0].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 202)
                    freeSpinDatas[1].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 203)
                    freeSpinDatas[2].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 204)
                    freeSpinDatas[3].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 205)
                    freeSpinDatas[4].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 206)
                    freeSpinDatas[5].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 207)
                    freeSpinDatas[6].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 208)
                    freeSpinDatas[7].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 209)
                    freeSpinDatas[8].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 210)
                    freeSpinDatas[9].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 211)
                    freeSpinDatas[10].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 212)
                    freeSpinDatas[11].Add(_FreeSpinDataList[i]);
            }

            for(int i = 0; i < freeSpinDatas.Length; i++)
            {
                for (int j = 0; j < freeSpinDatas[i].Count; j++)
                {
                    string[] lines = freeSpinDatas[i][j].Data.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
                    for(int k = 0; k < lines.Length; k++)
                    {
                        AmaticPacket packet     = new AmaticPacket(lines[k], cols, freecols);
                        AmaticEncrypt encrypt   = new AmaticEncrypt();

                        string oldStr = string.Format("{0}{1}", encrypt.WriteLengthAndDec("",packet.messageid), encrypt.WriteLengthAndDec("", packet.win));
                    }
                }
            }

            List<FreeOptionSpinData> processedStartSpinData = doPreProcessStartSpin(startSpinData, freeSpinDatas);
            await databaseWork.updateSpinData(processedStartSpinData);
            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }

        private List<FreeOptionSpinData> doPreProcessStartSpin(List<FreeOptionSpinData> fsOptStartList,List<FreeOptionSpinData>[] fsOptFreeListArray)
        {
            //write spintype
            for(int i = 0; i < fsOptStartList.Count; i++)
            {
                fsOptStartList[i].FreeSpinType = 0;
            }
            
            //write ranges meanrate
            for (int i = 0; i < fsOptStartList.Count; i++)
            {
                double startOdd = fsOptStartList[i].RealOdd;
                List<string> ranges = new List<string>();
                double  oddSum  = 0.0;
                int     oddCnt  = 0;
                for(int k = 0; k < OddList.Count; k++)
                {
                    bool flag = true;
                    for (int j = 0; j < 12; j++)
                    {
                        int spinTypeIndex = j;//200+가 스핀타입
                        int index = fsOptFreeListArray[spinTypeIndex].FindIndex(_ => (OddList[k].MinOdd <= startOdd + _.SpinOdd) && (OddList[k].MaxOdd >= startOdd + _.SpinOdd));
                        if (index == -1)
                        {
                            flag = false;
                            break;
                        }
                    }
                    
                    if (flag)
                        ranges.Add(k.ToString());
                    
                    if (k != 0 || !flag)//20~50구간에서만  minRate를 넣어준다
                        continue;

                    for (int j = 0; j < 12; j++)
                    {
                        int spinTypeIndex = j;//200+가 스핀타입
                        foreach(FreeOptionSpinData item in fsOptFreeListArray[spinTypeIndex])
                        {
                            if(fsOptStartList[i].RealOdd + item.SpinOdd >= OddList[0].MinOdd && fsOptStartList[i].RealOdd + item.SpinOdd <= OddList[0].MaxOdd)
                            {
                                oddSum += item.SpinOdd;
                                oddCnt++;
                            }
                        }
                    }
                    fsOptStartList[i].MinRate = Math.Round((fsOptStartList[i].RealOdd + oddSum) / oddCnt,4);
                }
                fsOptStartList[i].Ranges = string.Join(",", ranges);
            }
            return fsOptStartList;
        }
    }
}
