using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PragmaticPlayAutomationTool
{
    public class SlotDiamondAPI
    {
        private static SlotDiamondAPI _sInstace = new SlotDiamondAPI();
        public static SlotDiamondAPI Instance
        {
            get { return _sInstace; }
        }
        public string ClientID  { get; set; }
        public string SecretKey { get; set; }
        public string ApiURL    { get; set; }

        private string createMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public async Task<ApiUserLoginResult> loginAccount(string strUserID, string strPassword)
        {
            try
            {
                CreateAccountRequest request = new CreateAccountRequest();
                request.username        = strUserID;
                request.password        = strPassword;
                string strRequestJson   = JsonConvert.SerializeObject(request);
                string strAuthKey       = createMD5(SecretKey + strRequestJson);
                string strURL           = string.Format("{0}/createaccount", ApiURL);

                HttpClient httpClient   = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string                  strResponse = await response.Content.ReadAsStringAsync();
                CreateAccountResponse   resp        = JsonConvert.DeserializeObject<CreateAccountResponse>(strResponse);
                if (resp == null || resp.status != "0")
                    return null;

                return new ApiUserLoginResult(resp.data.usercode, resp.data.token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ApiUserWithdrawResult> withdrawAllAccount(string strUserCode, string strToken)
        {
            try
            {
                SubtractAllRequest request = new SubtractAllRequest();
                request.usercode        = strUserCode;
                request.token           = strToken;
                string strRequestJson   = JsonConvert.SerializeObject(request);
                string strAuthKey       = createMD5(SecretKey + strRequestJson);
                string strURL           = string.Format("{0}/subtractallmemberpoint", ApiURL);

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();
                SubtractAllResponse resp = JsonConvert.DeserializeObject<SubtractAllResponse>(strResponse);
                if (resp == null || resp.status != "0")
                    return null;

                return new ApiUserWithdrawResult(double.Parse(resp.data.transaction_amount), resp.data.transaction_id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ApiUserDepositResult> depositAccount(string strUserCode, string strToken, double money)
        {
            try
            {
                AddMemberPointRequest request = new AddMemberPointRequest();
                request.usercode                = strUserCode;
                request.token                   = strToken;
                request.transaction_amount      = Math.Round(money, 2).ToString();
                string strRequestJson           = JsonConvert.SerializeObject(request);
                string strAuthKey               = createMD5(SecretKey + strRequestJson);
                string strURL                   = string.Format("{0}/addmemberpoint", ApiURL);

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string                  strResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(strResponse);
                AddMemberPointResponse  resp        = JsonConvert.DeserializeObject<AddMemberPointResponse>(strResponse);
                if (resp == null || resp.status != "0")
                    return null;

                return new ApiUserDepositResult(double.Parse(resp.data.transaction_amount), resp.data.transaction_id);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<ApiUserBalanceResult> getAccountBalance(string strUserCode, string strToken)
        {
            try
            {
                AccountBalanceRequest request = new AccountBalanceRequest();
                request.usercode = strUserCode;
                request.token = strToken;

                string strRequestJson   = JsonConvert.SerializeObject(request);
                string strAuthKey       = createMD5(SecretKey + strRequestJson);
                string strURL           = string.Format("{0}/getaccountbalance", ApiURL);

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();

                AccountBalanceResponse resp = JsonConvert.DeserializeObject<AccountBalanceResponse>(strResponse);
                if (resp == null || resp.status != "0")
                    return null;

                double availabeBalance = double.Parse(resp.data.available_balance);
                double externalBalance = double.Parse(resp.data.external_balance);
                return new ApiUserBalanceResult(availabeBalance + externalBalance);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> getGameLaunchURL(string strUserCode, string strToken, string strGameID, string strReturnURL)
        {
            try
            {
                GetGameURLRequest request = new GetGameURLRequest();
                request.mode        = "real";
                request.usercode    = strUserCode;
                request.token       = strToken;
                request.game        = strGameID;
                request.lang        = "ko";
                request.return_url  = strReturnURL;

                string strRequestJson   = JsonConvert.SerializeObject(request);
                strRequestJson = strRequestJson.Replace("/", "\\/");

                string strAuthKey       = createMD5(SecretKey + strRequestJson);
                string strURL           = string.Format("{0}/getgameurl", ApiURL);

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));

                string strResponse = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                GetGameURLResponse resp = JsonConvert.DeserializeObject<GetGameURLResponse>(strResponse);
                if (resp == null || resp.status != "0")
                    return null;

                return resp.data.return_url;
            }
            catch(Exception)
            {
                return null;
            }
        }
        public async Task<Dictionary<string, string>> getGameList()
        {
            try
            {
                GetGameListRequest gameListRequest = new GetGameListRequest();
                gameListRequest.provider_id = "9";
                gameListRequest.language_id = "2";
                gameListRequest.limit = "1000";
                gameListRequest.offset = "0";

                var strRequestJson = JsonConvert.SerializeObject(gameListRequest);
                var strAuthKey = createMD5(SecretKey + strRequestJson);
                var strURL = string.Format("{0}/getgames", ApiURL);

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", ClientID);
                var response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                var strResponse = await response.Content.ReadAsStringAsync();

                Dictionary<string, string> gameIds = new Dictionary<string, string>();
                JToken jToken = JToken.Parse(strResponse);
                var gameDataArray = jToken["data"] as JArray;
                for (int i = 0; i < gameDataArray.Count; i++)
                {
                    var gameCode = (string)gameDataArray[i]["game_code"];
                    var gameId = (string)gameDataArray[i]["id"];
                    gameIds[gameCode] = gameId;
                }
                return gameIds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        class CreateAccountResponse
        {
            public string status    { get; set; }
            public string code      { get; set; }
            public string message   { get; set; }
            public Data   data      { get; set; }
            public class Data
            {
                public int      user_id     { get; set; }
                public string   username    { get; set; }
                public string   usercode    { get; set; }
                public string   token       { get; set; }
            }
        }
        class CreateAccountRequest
        {
            public string username { get; set; }
            public string password { get; set; }

        }
        class AddMemberPointRequest
        {
            public string usercode              { get; set; }
            public string transaction_amount    { get; set; }
            public string token                 { get; set; }
        }
        class AddMemberPointResponse
        {
            public string status    { get; set; }
            public string code      { get; set; }
            public string message   { get; set; }
            public Data   data      { get; set; }
            public class Data
            {
                public string transaction_amount { get; set; }
                public string transaction_id     { get; set; }
            }
        }
        class SubtractAllRequest
        {
            public string usercode { get; set; }
            public string token { get; set; }
        }
        class SubtractAllResponse
        {
            public string status { get; set; }
            public string code   { get; set; }
            public string message { get; set; }
            public Data data      { get; set; }
            public class Data
            {
                public string transaction_amount { get; set; }
                public string transaction_id { get; set; }
            }
        }
        class AccountBalanceRequest
        {
            public string usercode  { get; set; }
            public string token     { get; set; }
        }
        class AccountBalanceResponse
        {
            public string status    { get; set; }
            public string code      { get; set; }
            public string message   { get; set; }
            public Data   data      { get; set; }
            public class Data
            {
                public string available_balance { get; set; }
                public string external_balance  { get; set; }
            }
        }
        class GetGameURLRequest
        {
            public string mode          { get; set; }
            public string usercode      { get; set; }
            public string game          { get; set; }
            public string lang          { get; set; }
            public string return_url    { get; set; }
            public string token         { get; set; }
        }
        class GetGameURLResponse
        {
            public string status    { get; set; }
            public string code      { get; set; }
            public string message   { get; set; }
            public Data   data      { get; set; }
            public class Data
            {
                public string return_url { get; set; }
            }
        }
        class GetProvidersRequest
        {
            public string language_id { get; set; }
        }
        class GetGameListRequest
        {
            public string language_id { get; set; }
            public string provider_id { get; set; }
            public string limit        { get; set; }
            public string offset       { get; set; }
        }
    }
    public class ApiUserLoginResult
    {
        public string UserCode  { get; set; }
        public string Token     { get; set; }

        public ApiUserLoginResult(string strUserCode, string strToken)
        {
            this.UserCode   = strUserCode;
            this.Token      = strToken;
        }
    }
    public class ApiUserDepositResult
    {
        public double Amount        { get; set; }
        public string TransactionID { get; set; }

        public ApiUserDepositResult(double amount, string strTransactionID)
        {
            this.Amount         = amount;
            this.TransactionID  = strTransactionID;
        }
    }
    public class ApiUserWithdrawResult
    {
        public double Amount { get; set; }
        public string TransactionID { get; set; }
        public ApiUserWithdrawResult(double amount, string strTransactionID)
        {
            this.Amount         = amount;
            this.TransactionID  = strTransactionID;
        }
    }
    public class ApiUserBalanceResult
    {
        public double Balance { get; set; }
        public ApiUserBalanceResult(double balance)
        {
            this.Balance = balance;
        }
    }
}
