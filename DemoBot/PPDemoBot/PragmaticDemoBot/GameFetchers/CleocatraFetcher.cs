using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class CleocatraFetcher : GameSpinDataFetcher
    {
        public CleocatraFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = true;
        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            bool isFreeOrBonus = base.isFreeOrBonus(dicParams);
            if (isFreeOrBonus)
                return true;

            if (dicParams.ContainsKey("rs_p"))
                return true;

            return false;
        }
    }
}
