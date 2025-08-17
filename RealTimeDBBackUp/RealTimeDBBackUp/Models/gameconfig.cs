using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class gameconfig
    {
        public int      gameid      { get; set; }
        public int      gametype    { get; set; }
        public string   gamename    { get; set; }
        public string   gamesymbol  { get; set; }
        public decimal  payoutrate  { get; set; }
        public bool     openclose   { get; set; }
        public string   gamedata    { get; set; }
        public DateTime updatetime  { get; set; }
        public DateTime releasedate { get; set; }
    }
}
