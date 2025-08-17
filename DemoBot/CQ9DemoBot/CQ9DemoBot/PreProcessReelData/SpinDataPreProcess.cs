using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace PreProcessReelData
{
    public class SpinDataPreProcess
    {
        protected bool    _mustStop                             = false;
        protected string  _strGameSymbol                        = null;
        protected List<FreeOptionSpinData> _FreeSpinDataList    = null;
        public  int         _optionCnt                          = 0;
        public SpinDataPreProcess()
        {
        }

        public void doStop()
        {
            _mustStop = true;
        }
        public virtual async Task startPreProcess(SqliteDatabaseWork dbWorker, string strGameName)
        {
            _strGameSymbol              = strGameName;
            await dbWorker.initialize(strGameName);
            _FreeSpinDataList = await dbWorker.readSpinData();
        }
        protected SortedDictionary<string, string> splitResponse(string strResponse)
        {
            string[] strEntries = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            SortedDictionary<string, string> dicParamValues = new SortedDictionary<string, string>();
            for (int i = 0; i < strEntries.Length; i++)
            {
                string[] strParams = strEntries[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParams.Length == 2)
                {
                    dicParamValues.Add(strParams[0], strParams[1]);
                }
                else
                {
                    Console.WriteLine(strEntries[i]);
                    dicParamValues.Add(strParams[0], null);
                }
            }
            return dicParamValues;
        }
    }
}
