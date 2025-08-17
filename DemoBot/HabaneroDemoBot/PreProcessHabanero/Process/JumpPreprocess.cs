using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    public class JumpPreprocess : SpinDBPreprocess
    {
        public JumpPreprocess(string strFolderName, string strGameName):
            base(strFolderName, strGameName)
        {

        }
        public override async Task preprocessDB()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
            {
                await connection.OpenAsync();
                List<SpinData> spinDatas = new List<SpinData>();
                string strCommand = "SELECT id,odd, data FROM spins";
                SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        SpinData data = new SpinData((int)(long)reader["id"], (double)reader["odd"], 0);
                        data.Data = (string)reader["data"];
                        spinDatas.Add(data);
                    }
                }

                List<int> notSuitableIds = new List<int>();
                foreach (SpinData data in spinDatas)
                {
                    List<string> spinStrings = new List<string>(data.Data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                    
                    foreach(string oneSpinString in spinStrings)
                    {
                        dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(oneSpinString);
                        List<List<int>> currentVirtualReels     = convertJArrayToIntArray(resultContext["virtualreels"] as JArray);
                        
                        Dictionary<int, int> symbolAndCounts    = new Dictionary<int, int>();
                        List<int> candidateWinSymbols           = new List<int>();
                        bool isWildInFirstCol                   = false;
                        
                        for (int i = 0; i < currentVirtualReels.Count; i++)
                        {
                            if(i == 0)
                            {
                                for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                                {
                                    if (currentVirtualReels[i][j] == 1)
                                    {
                                        isWildInFirstCol = true;
                                        break;
                                    }
                                }
                            }

                            for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                            {
                                if (currentVirtualReels[i][j] != 1)
                                {
                                    if (symbolAndCounts.ContainsKey(currentVirtualReels[i][j]))
                                        symbolAndCounts[currentVirtualReels[i][j]]++;
                                    else
                                        symbolAndCounts.Add(currentVirtualReels[i][j], 1);

                                    if (i == 0 && isWildInFirstCol)
                                        continue;
                                    if (i == 0 && !candidateWinSymbols.Any(_ => _ == currentVirtualReels[i][j]))
                                        candidateWinSymbols.Add(currentVirtualReels[i][j]);
                                    if (i == 1 && isWildInFirstCol && !candidateWinSymbols.Any(_ => _ == currentVirtualReels[i][j]))
                                        candidateWinSymbols.Add(currentVirtualReels[i][j]);
                                }
                            }
                        }

                        int stackSymbol = 0;
                        List<HabaneroReelAndSymbolIndex> stackPositions = new List<HabaneroReelAndSymbolIndex>();
                        //윈후보심볼의 총개수가 5이상이면 그심벌을 스택시키기
                        foreach (int symbol in candidateWinSymbols)
                        {
                            if (symbolAndCounts[symbol] >= 5)
                            {
                                stackSymbol = symbol;
                                break;
                            }
                        }
                        if (stackSymbol != 0)
                            continue;

                        //후보에 5개이상이 없을때 전체개수가 5이상인 심벌을 스택시키기
                        foreach (KeyValuePair<int, int> pair in symbolAndCounts)
                        {
                            if (pair.Value >= 5)
                            {
                                stackSymbol = pair.Key;
                                break;
                            }
                        }
                        
                        if (stackSymbol != 0)
                            continue;

                        //위의 2경우가 아닌 경우에는 후보이외의 심벌 총개수가 5개보다 작은 아이디를 추가.(삭제를 위해)
                        int totalCnt    = 0;
                        for (int i = 0; i < currentVirtualReels.Count; i++)
                            for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                                totalCnt++;
                        
                        int candidateCnt = 0;
                        foreach(int symbol in candidateWinSymbols)
                            candidateCnt += symbolAndCounts[symbol];

                        if (totalCnt - candidateCnt < 5)
                        {
                            notSuitableIds.Add(data.ID);
                            break;
                        }
                    }
                }

                if(notSuitableIds.Count > 0)
                {
                    strCommand = string.Format("DELETE FROM spins WHERE id IN({0})",string.Join(",",notSuitableIds));
                    command     = new SQLiteCommand(strCommand, connection);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private List<List<int>> convertJArrayToIntArray(JArray virtualReelsJArray)
        {
            List<List<int>> virtualReels = new List<List<int>>();

            for (int i = 0; i < virtualReelsJArray.Count; i++)
            {
                List<int> virtualCol = new List<int>();
                JArray virtaulColJArray = virtualReelsJArray[i] as JArray;
                for (int j = 0; j < virtaulColJArray.Count; j++)
                {
                    virtualCol.Add(Convert.ToInt32(virtaulColJArray[j]));
                }
                virtualReels.Add(virtualCol);
            }
            return virtualReels;
        }

    }
    public class HabaneroReelAndSymbolIndex
    {
        public int reelindex    { get; set; }
        public int symbolindex  { get; set; }
    }
}
