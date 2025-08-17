using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class gamereport
    {
        public int      gameid      { get; set; }
        public int      agentid     { get; set; }
        public decimal  bet         { get; set; }
        public decimal  win         { get; set; }
        public DateTime reportdate  { get; set; }
    }
}
