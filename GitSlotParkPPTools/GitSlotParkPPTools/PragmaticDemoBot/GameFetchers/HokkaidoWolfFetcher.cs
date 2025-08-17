using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNGSpinFetcher
{
    class HokkaidoWolfFetcher : GameSpinDataFetcher
    {
        public HokkaidoWolfFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

            _strHostName = "demogamesfree-asia.pragmaticplay.net";

        }
    }
}
