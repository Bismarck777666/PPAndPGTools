using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PGSpinDBBuilder.PGFetcher
{
    class DragonTigerLuckFetcher : NewPGSpinDataFetcher
    {
        public DragonTigerLuckFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, int gameID, float betSize, int betLevel) :
            base(strProxyInfo, strProxyUserID, strProxyPassword, gameID, betSize, betLevel)
    {

    }
        protected override async Task<string> sendSpinRequest(HttpClient httpClient)
        {
            string strURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/Spin?traceId={1}", _strGameSymbol, genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("id", _strLastID),
                new KeyValuePair<string, string>("cs", _betSize.ToString()),
                new KeyValuePair<string, string>("ml", _betLevel.ToString()),
                new KeyValuePair<string, string>("wk", "0_C"),
                new KeyValuePair<string, string>("btt", "2"),
                new KeyValuePair<string, string>("atk", _strToken),
                new KeyValuePair<string, string>("pf",  "1"),
                new KeyValuePair<string, string>("gpt", "3"),
            });
            HttpResponseMessage message = await httpClient.PostAsync(strURL, postContent);
            message.EnsureSuccessStatusCode();
            string strContent = await message.Content.ReadAsStringAsync();
            return strContent;
        }
    }
}
