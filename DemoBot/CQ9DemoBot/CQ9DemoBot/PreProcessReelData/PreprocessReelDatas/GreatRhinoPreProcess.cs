using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace PreProcessReelData
{
    public class GreatRhinoMegaPreProcess : SpinDataPreProcess
    {
        private List<MinMaxItem> OddList = new List<MinMaxItem>
        {
            new MinMaxItem(20,50),  //20~50
            new MinMaxItem(10,50),  //10~50
            new MinMaxItem(50,100),  //50~100
            new MinMaxItem(100,300),  //100~300
            new MinMaxItem(300,500),  //300~500
            new MinMaxItem(500,1000),  //500~1000
            new MinMaxItem(1000,3000),  //1000~3000
        };
        public GreatRhinoMegaPreProcess()
        {
        }
        public override async Task startPreProcess(SqliteDatabaseWork databaseWork,string strGameName)
        {
            await base.startPreProcess(databaseWork, strGameName);
            
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
                else if(_FreeSpinDataList[i].SpinType == 200)
                    freeSpinDatas[0].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 201)
                    freeSpinDatas[1].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 202)
                    freeSpinDatas[2].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 203)
                    freeSpinDatas[3].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 204)
                    freeSpinDatas[4].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 205)
                    freeSpinDatas[5].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 206)
                    freeSpinDatas[6].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 207)
                    freeSpinDatas[7].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 208)
                    freeSpinDatas[8].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 209)
                    freeSpinDatas[9].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 210)
                    freeSpinDatas[10].Add(_FreeSpinDataList[i]);
                else if (_FreeSpinDataList[i].SpinType == 211)
                    freeSpinDatas[11].Add(_FreeSpinDataList[i]);
            }


            List<FreeOptionSpinData> processedStartSpinData = doPreProcessStartSpin(startSpinData, freeSpinDatas);
            await databaseWork.updateSpinData(processedStartSpinData);
            //List<FreeOptionSpinData> processedFreeSpinData  = doPreProcessFreeSpin(freeSpinDatas);
            //await databaseWork.updateSpinData(processedFreeSpinData);
            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }

        private List<FreeOptionSpinData> doPreProcessStartSpin(List<FreeOptionSpinData> fsOptStartList,List<FreeOptionSpinData>[] fsOptFreeListArray)
        {
            //write spintype
            for(int i = 0; i < fsOptStartList.Count; i++)
            {
                FreeOptionSpinData startSpinData = fsOptStartList[i];
                string[] startStr = startSpinData.Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                SortedDictionary<string, string> dicParamValues = splitResponse(startStr[startStr.Length - 1]);
                        
                fsOptStartList[i].FreeSpinType = 0;
                if (!dicParamValues.ContainsKey("s"))
                    continue;

                string[] symbols = dicParamValues["s"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                int scatterCnt = 0;
                for(int j = 0; j < symbols.Length; j++)
                {
                    if (Convert.ToInt32(symbols[j]) == 1)
                        scatterCnt++;
                }
                if (scatterCnt == 4)
                    fsOptStartList[i].FreeSpinType = 0;
                if (scatterCnt == 5)
                    fsOptStartList[i].FreeSpinType = 1;
                else if (scatterCnt == 6)
                    fsOptStartList[i].FreeSpinType = 2;
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
                    for (int j = 0; j < 4; j++)
                    {
                        int spinTypeIndex = fsOptStartList[i].FreeSpinType * 4 + j;//200+가 스핀타입
                        int index = fsOptFreeListArray[spinTypeIndex].FindIndex(_ => OddList[k].MinOdd <= startOdd + _.SpinOdd && (OddList[k].MaxOdd >= startOdd + _.SpinOdd));
                        if (index == -1)
                        {
                            flag = false;
                            break;
                        }
                    }
                    
                    if (flag)
                        ranges.Add(k.ToString());
                    
                    if (k != 0 || !flag)//20~50구간에서만  meanRate를 넣어준다
                        continue;

                    for (int j = 0; j < 4; j++)
                    {
                        int spinTypeIndex = fsOptStartList[i].FreeSpinType * 4 + j;//200+가 스핀타입
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

        private List<FreeOptionSpinData> doPreProcessFreeSpin(List<FreeOptionSpinData>[] fsOptFreeListArray)
        {
            List<FreeOptionSpinData> updateSpinTypeData = new List<FreeOptionSpinData>();
            for(int i = 0; i < fsOptFreeListArray.Length;i++)
            {
                for (int j = 0; j < fsOptFreeListArray[i].Count; j++)
                {
                    string strData = fsOptFreeListArray[i][j].Data;

                    if(strData.Contains("fs_opt=15,1,1"))
                    {
                        if (strData.Contains("fsopt_i=0"))
                            fsOptFreeListArray[i][j].SpinType = 200;
                        else if (strData.Contains("fsopt_i=1"))
                            fsOptFreeListArray[i][j].SpinType = 201;
                        else if (strData.Contains("fsopt_i=2"))
                            fsOptFreeListArray[i][j].SpinType = 202;
                        else if (strData.Contains("fsopt_i=3"))
                            fsOptFreeListArray[i][j].SpinType = 203;
                    }
                    else if (strData.Contains("fs_opt=19,1,1"))
                    {
                        if (strData.Contains("fsopt_i=0"))
                            fsOptFreeListArray[i][j].SpinType = 204;
                        else if (strData.Contains("fsopt_i=1"))
                            fsOptFreeListArray[i][j].SpinType = 205;
                        else if (strData.Contains("fsopt_i=2"))
                            fsOptFreeListArray[i][j].SpinType = 206;
                        else if (strData.Contains("fsopt_i=3"))
                            fsOptFreeListArray[i][j].SpinType = 207;
                    }
                    else if (strData.Contains("fs_opt=23,1,1"))
                    {
                        if (strData.Contains("fsopt_i=0"))
                            fsOptFreeListArray[i][j].SpinType = 208;
                        else if (strData.Contains("fsopt_i=1"))
                            fsOptFreeListArray[i][j].SpinType = 209;
                        else if (strData.Contains("fsopt_i=2"))
                            fsOptFreeListArray[i][j].SpinType = 210;
                        else if (strData.Contains("fsopt_i=3"))
                            fsOptFreeListArray[i][j].SpinType = 211;
                    }
                    updateSpinTypeData.Add(fsOptFreeListArray[i][j]);
                }
            }
            return updateSpinTypeData;
        }
    }
}
