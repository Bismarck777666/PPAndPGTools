using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data.Common;
using System.Net.Http;

namespace DatabaseClear
{
    public class Database
    {
        public string           _connectionStrings  = "";
        public SqlConnection    _sqlConnection;
        public List<string>     _agentNames         = new List<string>();
        public List<string>     _userNames          = new List<string>();
        public string           _gameurl            = "";
        public string           _apiurl             = "";
        public string           _gamesymbol         = "";

        public async Task<bool> initConfig()
        {
            try
            {
                string strConfigInfo    = File.ReadAllText("config.bconf");
                ConfigInfo config   = JsonConvert.DeserializeObject<ConfigInfo>(strConfigInfo);

                _agentNames     = config.agentnames;
                _userNames      = config.usernames;
                _gameurl        = config.gameurl;
                _apiurl         = config.apiurl;
                _gamesymbol     = config.gamesymbol;

                SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
                connStringBuilder.DataSource                = config.database.ip;
                connStringBuilder.UserID                    = config.database.user;
                connStringBuilder.Password                  = config.database.pass;
                connStringBuilder.TrustServerCertificate    = false;
                connStringBuilder.PersistSecurityInfo       = true;
                connStringBuilder.InitialCatalog            = config.database.dbname;

                _connectionStrings  = connStringBuilder.ConnectionString;
                _sqlConnection      = new SqlConnection(_connectionStrings);
                await _sqlConnection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task clear()
        {
            try
            {
                List<AgentInfoItem> agentinfos  = await findAgentInfoFromUsername(_agentNames);
                List<UserInfoItem> userinfos    = await findUserItems(agentinfos, _userNames);

                for (int i = 0; i < userinfos.Count; i++)
                {
                    bool opened = await openCustomGame(userinfos[i]);
                    if (!opened)
                        Console.WriteLine("Can't open game");
                }
                
                string useridStr    = "";
                string useridNumStr = "";
                if(userinfos.Count > 0)
                {
                    for(int i = 0; i < userinfos.Count; i++)
                    {
                        useridStr += userinfos[i].username + "','";
                        useridNumStr += userinfos[i].id + ",";
                    }
                    useridStr = useridStr.Substring(0, useridStr.Length - 3);
                    useridStr       = string.Format("'{0}'", useridStr);
                    useridNumStr    = useridNumStr.Substring(0, useridNumStr.Length - 1);

                    await removeUsers(useridStr, useridNumStr);
                }
                
                string agentidStr       = "";
                string agentidNumstr    = "";
                if (agentinfos.Count > 0)
                {
                    for (int i = 0; i < agentinfos.Count; i++)
                    {
                        agentidStr      += agentinfos[i].username + "','";
                        agentidNumstr   += agentinfos[i].id + ",";
                    }
                    agentidStr = string.Format("'{0}'", agentidStr.Substring(0, agentidStr.Length - 3));
                    agentidNumstr = agentidNumstr.Substring(0, agentidNumstr.Length - 1);

                    await removeAgents(agentidStr, agentidNumstr);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception has been occured while clear db ex:{0}",ex);
            }
        }

        public async Task openCustomGames()
        {
            try
            {
                List<AgentInfoItem> agentinfos = await findAgentInfoFromUsername(_agentNames);
                List<UserInfoItem> userinfos = await findUserItems(agentinfos, _userNames);

                for (int i = 0; i < userinfos.Count; i++)
                {
                    bool opened = await openCustomGame(userinfos[i]);
                    if (!opened)
                        Console.WriteLine("Can't open game");
                    else
                        Console.WriteLine("opened custom game");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception has been occured while clear db ex:{0}", ex);
            }
        }
        #region 업체,유저목록얻기
        public async Task<List<AgentInfoItem>> findAgentInfoFromUsername(List<string> agentnames)
        {
            List<AgentInfoItem> agentList = new List<AgentInfoItem>();
            if (agentnames.Count == 0)
                return agentList;

            string agentidLists = string.Join("','", agentnames.ToArray());
            agentidLists = string.Format("'{0}'", agentidLists);
            string strQuery = string.Format("SELECT id,username,level,parentid,agentids FROM agents WHERE username IN ({0})", agentidLists);

            
            SqlCommand command = new SqlCommand(strQuery, _sqlConnection);

            try
            {
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id          = (int)reader["id"];
                        string username = (string)reader["username"];
                        int level       = (int)reader["level"];
                        int parentid    = (int)reader["parentid"];
                        string agentids = (string)reader["agentids"];
                        AgentInfoItem item = new AgentInfoItem(id, username, level, parentid, agentids);
                        agentList.Add(item);
                    }
                }
                return await findChildInfos(agentList);
            }
            catch (Exception ex)
            {
                return agentList;
            }
        }
        public async Task<List<AgentInfoItem>> findChildInfos(List<AgentInfoItem> parentList)
        {
            List<AgentInfoItem> agentList = new List<AgentInfoItem>();
            if (parentList.Count == 0)
                return agentList;

            for(int i = 0; i < parentList.Count; i++)
            {
                agentList.Add(new AgentInfoItem(parentList[i]));
            }

            string parentids = string.Empty;
            for(int i = 0; i < parentList.Count; i++)
            {
                parentids += parentList[i].id + ",";
            }
            parentids = parentids.Substring(0, parentids.Length - 1);

            string strQuery = string.Format("SELECT id,username,level,parentid,agentids FROM agents WHERE parentid IN ({0})",parentids);
            SqlCommand command = new SqlCommand(strQuery, _sqlConnection);
            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int id                  = (int)reader["id"];
                    string username         = (string)reader["username"];
                    int level               = (int)reader["level"];
                    int parentid            = (int)reader["parentid"];
                    string agentids         = (string)reader["agentids"];

                    int index = agentList.FindIndex(_ => _.id == id);
                    if(index == -1)
                    {
                        AgentInfoItem item = new AgentInfoItem(id, username, level, parentid, agentids);
                        agentList.Add(item);
                    }
                }
            }
            if (parentList.Count == agentList.Count)
                return agentList;
            return await findChildInfos(agentList);
        }
        public async Task<List<UserInfoItem>> findUserItems(List<AgentInfoItem> agentInfos, List<string> usernames)
        {
            try
            {
                List<UserInfoItem> userList = new List<UserInfoItem>();

                string strUserNames     = "";
                string strQuery         = "";
                string strAgentids      = "";
                if (usernames.Count > 0)
                    strUserNames = string.Join("','", usernames.ToArray());
                strUserNames = string.Format("'{0}'", strUserNames);
                if (agentInfos.Count > 0)
                {
                    for(int i = 0; i < agentInfos.Count; i++)
                    {
                        strAgentids += agentInfos[i].id + ",";
                    }
                    strAgentids = strAgentids.Substring(0, strAgentids.Length - 1);
                }

                if(usernames.Count > 0 && agentInfos.Count > 0)
                    strQuery = string.Format("SELECT id,username,password FROM users WHERE username IN({0}) OR agentid IN ({1})", strUserNames,strAgentids);
                else if(usernames.Count > 0)
                    strQuery = string.Format("SELECT id,username,password FROM users WHERE username IN({0})", strUserNames);
                else if(agentInfos.Count > 0)
                    strQuery = string.Format("SELECT id,username,password FROM users WHERE agentid IN ({0})", strAgentids);
                else
                    return userList;

                SqlCommand command = new SqlCommand(strQuery, _sqlConnection);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id              = (int)(long)reader["id"];
                        string username     = (string)reader["username"];
                        string password     = (string)reader["password"];
                        UserInfoItem item = new UserInfoItem(id, username,password);
                        userList.Add(item);
                    }
                }
                return userList;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception has been occured in reading userlists");
                return new List<UserInfoItem>();
            }
        }
        #endregion

        #region 유저정보삭제
        public async Task<bool> removeUsers(string userids, string useridNums)
        {

            if (!await removeApiUserAndMoney(userids, useridNums))
                return false;
            if (!await removeUserPPGameLog(userids))
                return false;
#if CQ9EXITS
            if (!await removeUserCQ9GameLog(userids))
                return false;
#endif
            if (!await removeUserOtherInfo(userids))
                return false;
            if (!await removeUserTable(userids))
                return false;

            return true;
        }
        public async Task<bool> removeUserTable(string userids)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {

                string sqlQuery = string.Format("DELETE users WHERE username IN ({0})", userids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection,transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured when removing users ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeApiUserAndMoney(string userids,string useridNums)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                string sqlQuery = string.Format("DELETE userdeposits WHERE username IN ({0})", userids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE userwithdraws WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE rollchangelogs WHERE userdbid IN ({0}) AND isuser=1", useridNums);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();
                
                sqlQuery = string.Format("DELETE gamescorelogs WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE apiusers WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeApiUserAndMoney ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeUserPPGameLog(string userids)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                string sqlQuery = string.Format("DELETE ppuserrecentgamelog WHERE username IN ({0})", userids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE ppusertopgamelog WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeUserPPGameLog ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeUserCQ9GameLog(string userids)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                
                string sqlQuery = string.Format("DELETE cq9gamehistory WHERE userid IN ({0})", userids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeUserCQ9GameLog ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeUserOtherInfo(string userids)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                string sqlQuery     = string.Format("DELETE memos WHERE username IN ({0}) AND isagent=0", userids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery    = string.Format("DELETE userevents WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery    = string.Format("DELETE userrangeevents WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery    = string.Format("DELETE userreports WHERE username IN ({0})", userids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeUserOtherInfo ex: {0}", ex);
                return false;
            }
        }
#endregion

#region 업체정보삭제
        public async Task<bool> removeAgents(string agentids, string agentidNums)
        {
            if (!await removeAgentsMoney(agentids, agentidNums))
                return false;
            if (!await removeAgentOtherInfo(agentids,agentidNums))
                return false;
            if (!await removeAgentTable(agentids,agentidNums))
                return false;

            return true;
        }
        public async Task<bool> removeAgentTable(string agentids,string agentidNums)
        {
            try
            {
                string sqlQuery = string.Format("DELETE agents WHERE id IN ({0})", agentidNums);
                
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception has occured in removeAgentTable ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeAgentsMoney(string agentids, string agentidNums)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                string sqlQuery = string.Format("DELETE agentdeposits WHERE username IN ({0})", agentids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE agentwithdraws WHERE username IN ({0})", agentids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE rollchangelogs WHERE userdbid IN ({0}) AND isuser=0", agentidNums);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE agentscorelogs WHERE username IN ({0})", agentids);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeAgentsMoney ex: {0}", ex);
                return false;
            }
        }
        public async Task<bool> removeAgentOtherInfo(string agentids,string agentidNums)
        {
            
            var transaction = _sqlConnection.BeginTransaction();
            try
            {
                string sqlQuery = string.Format("DELETE memos WHERE username IN ({0}) AND isagent=0", agentids);
                SqlCommand command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE agentreports WHERE agentid IN ({0})", agentidNums);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE agentgameconfigs WHERE agentid IN ({0})", agentidNums);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                sqlQuery = string.Format("DELETE agentnotices WHERE agentid IN ({0})", agentidNums);
                command = new SqlCommand(sqlQuery, _sqlConnection, transaction);
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("exception has occured in removeAgentOtherInfo ex: {0}", ex);
                return false;
            }
            
        }
#endregion

#region 모든게임이력,모든 입출금이력삭제
        public async Task removeAllUserLogs()
        {
            using(var transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    string strQuery = "DELETE userrangeevents";
                    SqlCommand command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE ppuserrecentgamelog";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE ppusertopgamelog";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE ppusercreatedlinks";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

#if CQ9EXITS
                    strQuery = "DELETE cq9gamehistory";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();
#endif
                    strQuery = "DELETE rollchangelogs";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE userdeposits";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE userwithdraws";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE userreports";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception has occured in removeAllUserLogs ex: {0}", ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task removeAllAgentLogs()
        {
            using (var transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    string strQuery = "DELETE agentdeposits";
                    SqlCommand command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE agentwithdraws";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE agentreports";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    strQuery = "DELETE agentscorelogs";
                    command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception has occured in removeAllAgentLogs ex: {0}", ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task removeAllGameLogTable()
        {
            using (var transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    DateTime pointer = DateTime.Now.AddDays(-100);
                    while(pointer < DateTime.Now)
                    {
                        pointer = pointer.AddDays(1);
                        string strYear  = pointer.Year.ToString().Substring(2, 2);
                        string strMonth = pointer.Month > 9 ? pointer.Month.ToString() : "0" + pointer.Month.ToString();
                        string strDay   = pointer.Day > 9 ? pointer.Day.ToString() : "0" + pointer.Day.ToString();
                        string tableName = string.Format("gamelog{0}{1}{2}",strYear,strMonth,strDay);
                        string strQuery = string.Format("IF OBJECT_ID('dbo.{0}','U') IS NOT NULL DROP TABLE dbo.{0}", tableName);
                        SqlCommand command = new SqlCommand(strQuery, _sqlConnection, transaction);
                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception has occured in removeAllGameLogs ex: {0}", ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task removeAllAgents()
        {
            using (var transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    string strQuery = "DELETE agents WHERE level<>0";
                    SqlCommand command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception has been occured in removeAllAgents ex: {0}",ex);
                    transaction.Rollback();
                }
            }
        }
        public async Task removeAllUsers()
        {
            using (var transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    string strQuery = "DELETE users";
                    SqlCommand command = new SqlCommand(strQuery, _sqlConnection, transaction);
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception has been occured in removeAllUsers ex: {0}", ex);
                    transaction.Rollback();
                }
            }
        }
#endregion

#region 유저삭제전에 카스텀 게임으로 들어오게 하기
        public async Task<bool> openCustomGame(UserInfoItem item)
        {
            PPAPIAuthResponse apiResponse = await callAuthTokenRequest(item);
            if (apiResponse.result != "success")
                return false;

            var httpClientHandler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(httpClientHandler);

            KeyValuePair<string, string>[] initPostValues = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("action",  "doInit"),
                new KeyValuePair<string, string>("symbol",  _gamesymbol),
                new KeyValuePair<string, string>("cver",    "82703"),
                new KeyValuePair<string, string>("index",   "1"),
                new KeyValuePair<string, string>("counter", "1"),
                new KeyValuePair<string, string>("repeat",  "0"),
                new KeyValuePair<string, string>("mgckey",  apiResponse.sessiontoken),
            };
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(initPostValues);
            string initUrl = string.Format("{0}/gitapi/pp/service/gameService", _apiurl);
            HttpResponseMessage message = await httpClient.PostAsync(initUrl, postContent);
            string strContent = await message.Content.ReadAsStringAsync();
            message.EnsureSuccessStatusCode();

            return true;
        }
        public async Task<PPAPIAuthResponse> callAuthTokenRequest(UserInfoItem item)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                string strTokenUrl = string.Format("{0}/gitapi/pp/service/auth.do?userid={1}&password={2}", _apiurl, item.username, item.password);

                HttpResponseMessage response = await httpClient.GetAsync(strTokenUrl);
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();
                PPAPIAuthResponse apiResponse = JsonConvert.DeserializeObject<PPAPIAuthResponse>(strResponse);
                return apiResponse;
            }
            catch
            {
                return new PPAPIAuthResponse();
            }
        }
#endregion
    }

    public class AgentInfoItem
    {
        public int      id          { get; set; }
        public string   username    { get; set; }
        public int      level       { get; set; }
        public int      parentid    { get; set; }
        public string   agentids    { get; set; }
        public AgentInfoItem(int idInfo,string usernameInfo,int levelInfo,int parentidInfo,string agentidsInfo)
        {
            id          = idInfo;
            username    = usernameInfo;
            level       = levelInfo;
            parentid    = parentidInfo;
            agentids    = agentidsInfo;
        }
        public AgentInfoItem(AgentInfoItem item)
        {
            id          = item.id;
            username    = item.username;
            level       = item.level;
            parentid    = item.parentid;
            agentids    = item.agentids;
        }
    }
    public class UserInfoItem
    {
        public int      id          { get; set; }
        public string   username    { get; set; }
        public string   password    { get; set; }
        public UserInfoItem(int idInfo,string usernameInfo,string passwordInfo)
        {
            id          = idInfo;
            username    = usernameInfo;
            password    = passwordInfo;
        }
    }
    public class DBConfigItem
    {
        public string       ip      { get; set; }
        public int          port    { get; set; }
        public string       user    { get; set; }
        public string       pass    { get; set; }
        public string       dbname  { get; set; }
    }
    public class ConfigInfo
    {
        public DBConfigItem database    { get; set; }
        public List<string> agentnames  { get; set; }
        public List<string> usernames   { get; set; }
        public string       gameurl     { get; set; }
        public string       apiurl      { get; set; }
        public string       gamesymbol  { get; set; }
    }
    public class PPAPIAuthResponse
    {
        public string result        { get; set; }
        public string sessiontoken  { get; set; }
        public PPAPIAuthResponse()
        {
            result          = string.Empty;
            sessiontoken    = string.Empty;
        }
    }
}
