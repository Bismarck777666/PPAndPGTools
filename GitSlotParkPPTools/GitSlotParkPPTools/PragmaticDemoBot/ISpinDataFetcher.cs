using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNGSpinFetcher
{
    public class ISpinDataFetcher
    {
        public virtual async Task startFetch(string strGameSymbol, string strGameURL, double defaultC, int lineCount)
        {

        }
        public virtual void doStop()
        {

        }
    }
}
