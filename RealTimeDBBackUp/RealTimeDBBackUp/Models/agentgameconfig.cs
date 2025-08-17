using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class agentgameconfig
    {
        public int      agentid     { get; set; }
        public int      gameid      { get; set; }
        public decimal  payoutrate  { get; set; }
        public DateTime updatetime  { get; set; }
    }
}
