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

        protected List<MinMaxItem> OddList = new List<MinMaxItem>
        {
            new MinMaxItem(20,50),  //20~50
            new MinMaxItem(10,50),  //10~50
            new MinMaxItem(50,100),  //50~100
            new MinMaxItem(100,300),  //100~300
            new MinMaxItem(300,500),  //300~500
            new MinMaxItem(500,1000),  //500~1000
            new MinMaxItem(1000,3000),  //1000~3000
        };

        public SpinDataPreProcess()
        {
        }

        public void doStop()
        {
            _mustStop = true;
        }
        
        public virtual async Task startPreProcess(SqliteDatabaseWork dbWorker, string strGameName, int cols, int freecols)
        {
            _strGameSymbol          = strGameName;
            await dbWorker.initialize(strGameName);
            _FreeSpinDataList = await dbWorker.readSpinData();
        }
    }
}
