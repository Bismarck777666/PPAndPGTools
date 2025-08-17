using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class DBMonitorSnapshot
    {
        private static DBMonitorSnapshot    _sInstance = new DBMonitorSnapshot();
        public static DBMonitorSnapshot     Instance    => _sInstance;

        public DateTime GameConfigUpdateTimes       { get; set; }
        public DateTime AgentGameConfigUpdateTimes  { get; set; }
        public DateTime AgentUpdateTime             { get; set; }
        public DateTime AgentReportUpdateTime       { get; set; }
        public DateTime GameReprotUpdateTime        { get; set; }
        
        public DBMonitorSnapshot()
        {
            GameConfigUpdateTimes       = new DateTime(1970, 1, 1);
            AgentGameConfigUpdateTimes  = new DateTime(1970, 1, 1);
            AgentUpdateTime             = new DateTime(1970, 1, 1);
            AgentReportUpdateTime       = new DateTime(1970, 1, 1);
            GameReprotUpdateTime        = new DateTime(1970, 1, 1);
        }
    }
}