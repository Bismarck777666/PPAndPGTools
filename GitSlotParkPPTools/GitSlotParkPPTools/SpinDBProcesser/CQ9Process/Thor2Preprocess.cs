using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpinDBProcesser
{
    class Thor2Preprocess : SpinDBPreprocess
    {
        public Thor2Preprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }

        public override async Task preprocessDB()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    List<SpinData> spinDatas = new List<SpinData>();
                    string strCommand = "SELECT id, odd, data FROM spins WHERE spintype=1";
                    SQLiteCommand command = new SQLiteCommand(strCommand, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            SpinData spin = new SpinData((int)(long)reader["id"], (double)reader["odd"], 0);
                            spin.Data = (string)reader["data"];
                            spinDatas.Add(spin);
                        }
                    }
                    strCommand = "UPDATE spins SET canpay = 0";
                    command = new SQLiteCommand(strCommand, connection);
                    await command.ExecuteNonQueryAsync();

                    Dictionary<int, double> minRateUpdates = new Dictionary<int, double>();

                    List<int> canPayList = new List<int>();
                    for (int i = 0; i < spinDatas.Count; i++)
                    {
                        dynamic spinResult = JsonConvert.DeserializeObject<dynamic>(spinDatas[i].Data.Split('\n')[0]);
                        dynamic freeStartSymbols = spinResult["SymbolResult"];
                        int bigSymbol = 0;
                        bool canPay = true;
                        for (int j = 0; j < freeStartSymbols.Count; j++)
                        {
                            string[] rowSymbols = Convert.ToString(freeStartSymbols[j]).Split(',');
                            for (int k = 0; k < rowSymbols.Length; k++)
                            {
                                if (rowSymbols[k] != "W" && rowSymbols[k] != "SC")
                                {
                                    int symbol = Convert.ToInt32(rowSymbols[k]);
                                    if (symbol < 10)
                                    {
                                        if (bigSymbol == 0)
                                            bigSymbol = symbol;
                                        if (bigSymbol != symbol)
                                        {
                                            canPay = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!canPay)
                                break;
                        }
                        if (canPay)
                            canPayList.Add(spinDatas[i].ID);

                    }
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (int id in canPayList)
                        {
                            strCommand = "UPDATE spins SET canpay=1 WHERE id=@id";
                            command = new SQLiteCommand(strCommand, connection, transaction);
                            command.Parameters.AddWithValue("@id", id);
                            await command.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
