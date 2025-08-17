using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNGSpinFetcher
{
    class TheUltimate5Fetcher : GameSpinDataFetcher
    {
        public TheUltimate5Fetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            bool isFreeOrBonus = base.isFreeOrBonus(dicParams);
            if (isFreeOrBonus)
                return true;

            if (dicParams.ContainsKey("rs"))
                return true;

            return false;
        }
    }
}
