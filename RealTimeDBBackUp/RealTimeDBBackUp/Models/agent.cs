using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp
{
    public class agent
    {
        public int      id          { get; set; }
        public string   username    { get; set; }
        public string   password    { get; set; }
        public string   secretkey   { get; set; }
        public string   callbackurl { get; set; }
        public string   apitoken    { get; set; }
        public string   nickname    { get; set; }
        public int      state       { get; set; }
        public int      currency    { get; set; }
        public int      language    { get; set; }
        public DateTime updatetime  { get; set; }
    }
}
