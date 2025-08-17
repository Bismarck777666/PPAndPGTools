using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNGSpinFetcher
{
    class DragonHoldAndSpinFetcher : GameSpinDataFetcher
    {
        public DragonHoldAndSpinFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }

        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("rs_p"))
                return true;

            return false;
        }
    }
}
