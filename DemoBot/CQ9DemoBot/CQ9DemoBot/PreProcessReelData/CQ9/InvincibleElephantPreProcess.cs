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
    public class InvincibleElephantPreProcess : SpinDataPreProcess
    {
        public InvincibleElephantPreProcess()
        {
        }
        public override async Task startPreProcess(SqliteDatabaseWork databaseWork,string strGameName)
        {
            _strGameSymbol = strGameName;
            await databaseWork.initialize(strGameName);
            List<SpinData> normalFreeSpinData = await databaseWork.readNormalFreeSpinData(1);
            
            int bufCnt = 0;
            try
            {
                List<SpinData> over4SpinData = new List<SpinData>();
                for (int i = 0; i < normalFreeSpinData.Count; i++)
                {
                    string[] responses = normalFreeSpinData[i].Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int beforeLockCnt = 0, currentLockCnt = 0;
                    for (int j = 0; j < responses.Length; j++)
                    {
                        currentLockCnt = 0;
                        dynamic response = JsonConvert.DeserializeObject<dynamic>(responses[j]);
                        if (!object.ReferenceEquals(response["LockPos"], null))
                        {
                            for (int ii = 0; ii < response["LockPos"].Count; ii++)
                            {
                                for (int jj = 0; jj < response["LockPos"][ii].Count; jj++)
                                {
                                    if ((int)response["LockPos"][ii][jj] != 0)
                                    {
                                        currentLockCnt++;
                                    }
                                }
                            }
                            if (currentLockCnt - beforeLockCnt >= 4)
                            {
                                over4SpinData.Add(normalFreeSpinData[i]);
                                break;
                            }
                            beforeLockCnt = currentLockCnt;
                        }
                    }
                }
                string ids = "";
                for(int i = 0; i < over4SpinData.Count; i++)
                    ids += over4SpinData[i].Id + ",";
                ids = ids.Substring(0, ids.Length - 1);

                await databaseWork.deleteSpinData(ids);
            }
            catch (Exception ex)
            {

            }

            Console.WriteLine(string.Format("{0} PreProcess Has Fininshed", strGameName));
            Console.ReadLine();
        }

    }
}
