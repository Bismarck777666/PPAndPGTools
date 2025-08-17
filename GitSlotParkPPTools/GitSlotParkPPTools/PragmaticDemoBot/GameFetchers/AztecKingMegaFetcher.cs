using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNGSpinFetcher
{
    class AztecKingMegaFetcher : GameSpinDataFetcher
    {
        public AztecKingMegaFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {

            _strHostName = "demogamesfree-asia.pragmaticplay.net";

        }
        protected override int FreeSpinOptionCount
        {
            get { return 4; }
        }

        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("fs"))
                return true;

            if (dicParams.ContainsKey("tmb_win"))
                return true;

            return false;
        }
        protected override int findFreeSpinType(SortedDictionary<string, string> dicParams)
        {
            if (!dicParams.ContainsKey("fs_opt"))
                return 100;

            string strValue = dicParams["fs_opt"].Split(new string[] { "~" }, StringSplitOptions.None)[0];
            if (strValue == "15,1,1")
                return 0;
            else if (strValue == "19,1,1")
                return 1;
            else if (strValue == "23,1,1")
                return 2;

            return 100;
        }
    }
}
