using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class BuffaloKingMegaFetcher : GameSpinDataFetcher
    {
        public BuffaloKingMegaFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) : 
            base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("fs"))
                return true;

            if (dicParams.ContainsKey("tmb_win"))
                return true;
            return false;
        }
    }
}
