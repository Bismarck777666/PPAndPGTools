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
    public class MoneyTreePreProcess : SpinDataPreProcess
    {
        public MoneyTreePreProcess()
        {
        }

        public override async Task startPreProcess(SqliteDatabaseWork databaseWork, string strGameName)
        {
            _strGameSymbol = strGameName;
            await databaseWork.initialize(strGameName);
            List<SpinData> normalSpinData = await databaseWork.readNormalFreeSpinData(0);

            try
            {
                List<SpinData> triggerSpinData  = new List<SpinData>();
                List<SpinData> respinSpinData   = new List<SpinData>();
                List<SpinData> doubleTrigger    = new List<SpinData>();

                for (int i = 0; i < normalSpinData.Count; i++)
                {
                    string[] responses = normalSpinData[i].Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if(responses.Length > 1)
                    {
                        respinSpinData.Add(normalSpinData[i]);
                        dynamic freeResponse = JsonConvert.DeserializeObject<dynamic>(responses.Last());

                        if (object.ReferenceEquals(freeResponse["NextSTable"], null) || (int)freeResponse["NextSTable"] == 0)
                            continue;

                        doubleTrigger.Add(normalSpinData[i]);
                        continue;
                    }

                    dynamic response = JsonConvert.DeserializeObject<dynamic>(responses[0]);
                    if (object.ReferenceEquals(response["NextSTable"], null) || (int)response["NextSTable"] == 0)
                        continue;

                    triggerSpinData.Add(normalSpinData[i]);
                }

                string ids = "";
                for (int i = 0; i < doubleTrigger.Count; i++)
                    ids += doubleTrigger[i].Id + ",";
                if(doubleTrigger.Count > 0)
                    ids = ids.Substring(0, ids.Length - 1);

                await databaseWork.deleteSpinData(ids);

                ids = "";
                for (int i = 0; i < respinSpinData.Count; i++)
                    ids += respinSpinData[i].Id + ",";
                if (respinSpinData.Count > 0)
                    ids = ids.Substring(0, ids.Length - 1);
                await databaseWork.updateSpinType(ids, 1);

                ids = "";
                for (int i = 0; i < triggerSpinData.Count; i++)
                    ids += triggerSpinData[i].Id + ",";
                if (triggerSpinData.Count > 0)
                    ids = ids.Substring(0, ids.Length - 1);
                await databaseWork.updateSpinType(ids, 2);

            }
            catch (Exception ex)
            {

            }

            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }
    }
}
