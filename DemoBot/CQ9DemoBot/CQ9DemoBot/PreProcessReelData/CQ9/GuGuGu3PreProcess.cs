using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace PreProcessReelData
{
    public class GuGuGu3PreProcess : SpinDataPreProcess
    {
        public GuGuGu3PreProcess()
        {
        }
        public override async Task startPreProcess(SqliteDatabaseWork databaseWork,string strGameName)
        {
            _strGameSymbol = strGameName;
            await databaseWork.initialize(strGameName);
            List<SpinData> normalFreeSpinData = await databaseWork.readNormalFreeSpinData(1);
            
            try
            {
                List<SpinData> scatter3SpinData = new List<SpinData>();
                List<SpinData> scatter4SpinData = new List<SpinData>();
                List<SpinData> scatter5SpinData = new List<SpinData>();

                for (int i = 0; i < normalFreeSpinData.Count; i++)
                {
                    string[] responses = normalFreeSpinData[i].Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int scatterCnt      = 0;
                    dynamic response    = JsonConvert.DeserializeObject<dynamic>(responses[0]);

                    if (!object.ReferenceEquals(response["udsOutputWinLine"], null))
                    {
                        
                        for (int j = 0; j < response["udsOutputWinLine"].Count; j++)
                        {
                            if(response["udsOutputWinLine"][j]["SymbolId"] == "F")
                            {
                                scatterCnt = (int)response["udsOutputWinLine"][j]["SymbolCount"];
                                break;
                            }
                        }
                        if (scatterCnt == 3)
                            scatter3SpinData.Add(normalFreeSpinData[i]);
                        else if(scatterCnt == 4)
                            scatter4SpinData.Add(normalFreeSpinData[i]);
                        else if(scatterCnt == 5)
                            scatter5SpinData.Add(normalFreeSpinData[i]);
                    }
                }
                string scatter3Ids = "", scatter4Ids = "", scatter5Ids = "";
                for(int i = 0; i < scatter3SpinData.Count; i++)
                    scatter3Ids += scatter3SpinData[i].Id + ",";
                for (int i = 0; i < scatter4SpinData.Count; i++)
                    scatter4Ids += scatter4SpinData[i].Id + ",";
                for (int i = 0; i < scatter5SpinData.Count; i++)
                    scatter5Ids += scatter5SpinData[i].Id + ",";

                scatter3Ids = scatter3Ids.Substring(0, scatter3Ids.Length - 1);
                scatter4Ids = scatter4Ids.Substring(0, scatter4Ids.Length - 1);
                scatter5Ids = scatter5Ids.Substring(0, scatter5Ids.Length - 1);
                await databaseWork.updateFreeSpinType(scatter3Ids, 0);
                await databaseWork.updateFreeSpinType(scatter4Ids, 1);
                await databaseWork.updateFreeSpinType(scatter5Ids, 2);
            }
            catch (Exception ex)
            {

            }

            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }

    }
}
