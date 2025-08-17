using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class LeprechaunSongFetcher : GameSpinDataFetcher
    {
        public LeprechaunSongFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV4 = false;
        }

        protected override async Task<string> doBonus(HttpClient httpClient, string strToken, int doBonusID)
        {
            int bonusID = 0;
            if (doBonusID >= 2)
                bonusID = doBonusID - 2;
            var postValues = new List<KeyValuePair<string, string>>(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("action", "doBonus"),
                            new KeyValuePair<string, string>("symbol", _strGameSymbol),
                            new KeyValuePair<string, string>("ind",     doBonusID.ToString()),
                            new KeyValuePair<string, string>("index",  _index.ToString()),
                            new KeyValuePair<string, string>("counter",_counter.ToString()),
                            new KeyValuePair<string, string>("repeat", "0"),
                            new KeyValuePair<string, string>("mgckey", strToken),
            });

            if (doBonusID == 1)
                postValues.RemoveAt(2);

            FormUrlEncodedContent postContent = new FormUrlEncodedContent(postValues);
            HttpResponseMessage message = null;

            if (_isV4)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/ge/v4/gameService", _strHostName), postContent);
            else if (_isV3)
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/v3/gameService", _strHostName), postContent);
            else
                message = await httpClient.PostAsync(string.Format("https://{0}/gs2c/gameService", _strHostName), postContent);

            message.EnsureSuccessStatusCode();

            _index++;
            _counter += 2;
            return await message.Content.ReadAsStringAsync();
        }

    }
}
