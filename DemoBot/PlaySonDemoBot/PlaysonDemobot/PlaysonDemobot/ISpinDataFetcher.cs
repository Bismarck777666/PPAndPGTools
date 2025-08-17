using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaysonDemobot
{
    public class ISpinDataFetcher
    {
        public virtual async Task startFetch(string strGameSymbol, string strGameURL, double defaultC,double realBet, int lineCount, string strEnginVersion)
        {

        }
        public virtual void doStop()
        {

        }
    }
}
