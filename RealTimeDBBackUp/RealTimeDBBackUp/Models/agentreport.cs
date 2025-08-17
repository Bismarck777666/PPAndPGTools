using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class agentreport
    {
        public int      agentid     { get; set; }
        public decimal  bet         { get; set; }
        public decimal  win         { get; set; }
        public DateTime reporttime  { get; set; }
    }
}
