using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class EightDragonsFetcher : GameSpinDataFetcher
    {
        public EightDragonsFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }
        protected override int FreeSpinOptionCount
        {
            get { return 5; }
        }

        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            if (!dicParams.ContainsKey("fs_opt"))
                return 100;
            return 0;
        }

        protected override int selectFreeOption(int freeSpinType)
        {
            return 3;
        }
    }
}
