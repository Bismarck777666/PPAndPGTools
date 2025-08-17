using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDBBackUp.Database
{
    public class WriterSnapshot
    {
        private static WriterSnapshot   _sInstance  = new WriterSnapshot();
        public static WriterSnapshot    Instance    => _sInstance;

        private List<gameconfig>      _gameConfigList       = new List<gameconfig>();
        private List<agentgameconfig> _agentGameConfigList  = new List<agentgameconfig>();
        private List<agent>           _agentList            = new List<agent>();
        private List<agentreport>     _agentReportList      = new List<agentreport>();
        private List<gamereport>      _gameReportList       = new List<gamereport>();

        private Dictionary<string, List<string>> _createdBackupTables = new Dictionary<string, List<string>>();

        public List<gameconfig> PopGameConfigUpdates(int count = 5000)
        {
            if (_gameConfigList.Count == 0)
                return null;

            List<gameconfig> configItems = new List<gameconfig>();
            
            for(int i = 0; i < _gameConfigList.Count; i++)
            {
                configItems.Add(_gameConfigList[i]);
                if (configItems.Count > count)
                    break;
            }
            _gameConfigList.RemoveRange(0, configItems.Count);

            return configItems;
        }
        public void PushGameConfigUpdateItems(List<gameconfig> gameConfigList)
        {
            for (int i = 0; i < gameConfigList.Count; i++)
            {
                _gameConfigList.Add(gameConfigList[i]);
            }
        }
        public List<agentgameconfig> PopAgentGameConfigUpdates(int count = 50000)
        {
            if (_agentGameConfigList.Count == 0)
                return null;

            List<agentgameconfig> agentConfigItems = new List<agentgameconfig>();

            for (int i = 0; i < _agentGameConfigList.Count; i++)
            {
                agentConfigItems.Add(_agentGameConfigList[i]);
                if (agentConfigItems.Count > count)
                    break;
            }
            _agentGameConfigList.RemoveRange(0, agentConfigItems.Count);

            return agentConfigItems;
        }
        public void PushAgentGameConfigUpdateItems(List<agentgameconfig> agentGameConfigList)
        {
            for (int i = 0; i < agentGameConfigList.Count; i++)
            {
                _agentGameConfigList.Add(agentGameConfigList[i]);
            }
        }
        public List<agent> PopAgentUpdates(int count = 5000)
        {
            if (_agentList.Count == 0)
                return null;

            List<agent> agentItems = new List<agent>();

            for (int i = 0; i < _agentList.Count; i++)
            {
                agentItems.Add(_agentList[i]);
                if (agentItems.Count > count)
                    break;
            }
            _agentList.RemoveRange(0, agentItems.Count);

            return agentItems;
        }
        public void PushAgentUpdateItems(List<agent> agentList)
        {
            for (int i = 0; i < agentList.Count; i++)
            {
                _agentList.Add(agentList[i]);
            }
        }
        public List<agentreport> PopAgentReportUpdates(int count = 5000)
        {
            if (_agentReportList.Count == 0)
                return null;

            List<agentreport> reportItems = new List<agentreport>();

            for (int i = 0; i < _agentReportList.Count; i++)
            {
                reportItems.Add(_agentReportList[i]);
                if (reportItems.Count > count)
                    break;
            }
            _agentReportList.RemoveRange(0, reportItems.Count);

            return reportItems;
        }
        public void PushAgentReportUpdateItems(List<agentreport> agentReportList)
        {
            for (int i = 0; i < agentReportList.Count; i++)
            {
                _agentReportList.Add(agentReportList[i]);
            }
        }
        public List<gamereport> PopGameReportUpdates(int count = 5000)
        {
            if (_gameReportList.Count == 0)
                return null;

            List<gamereport> reportItems = new List<gamereport>();
            for (int i = 0; i < _gameReportList.Count; i++)
            {
                reportItems.Add(_gameReportList[i]);
                if (reportItems.Count > count)
                    break;
            }
            _gameReportList.RemoveRange(0, reportItems.Count);

            return reportItems;
        }
        public void PushGameReportUpdateItems(List<gamereport> gameReportList)
        {
            for (int i = 0; i < gameReportList.Count; i++)
            {
                _gameReportList.Add(gameReportList[i]);
            }
        }

        public bool IsServerBackupTableCreated(string servername, string tablename)
        {
            if (_createdBackupTables.Keys.Contains(servername))
            {
                if (_createdBackupTables[servername] != null && _createdBackupTables[servername].Contains(tablename))
                    return true;
            }

            return false;
        }
        public void HasServerBackupTable(string servername, string tablename)
        {
            if (!_createdBackupTables.Keys.Contains(servername))
            {
                _createdBackupTables.Add(servername, new List<string>());
            }

            if (!_createdBackupTables[servername].Contains(tablename))
            {
                _createdBackupTables[servername].Add(tablename);
            }
        }
    }
}
