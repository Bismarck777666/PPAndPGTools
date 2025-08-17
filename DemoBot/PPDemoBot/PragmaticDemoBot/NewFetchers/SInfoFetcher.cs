using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PragmaticDemoBot;

namespace PragmaticDemoBot
{
    internal class SInfoFetcher : EuroNoWinRespinFetcher
    {
        public SInfoFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet, bool isV4) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet, false, false)
        {
            _isV4 = isV4;
        }

        protected override KeyValuePair<string, string>[] buildDoSpinRequest(string strToken)
        {
            if (!_hasAnteBet)
            {
                KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
                        {
                    new KeyValuePair<string, string>("action",  "doSpin"),
                    new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                    new KeyValuePair<string, string>("c",       Math.Round(_defaultC, 2).ToString()),
                    new KeyValuePair<string, string>("l",       _lineCount.ToString()),
                    new KeyValuePair<string, string>("sInfo",   "n"),
                    new KeyValuePair<string, string>("index",   _index.ToString()),
                    new KeyValuePair<string, string>("counter", _counter.ToString()),
                    new KeyValuePair<string, string>("repeat",  "0"),
                    new KeyValuePair<string, string>("mgckey",  strToken),
                        };
                return postValues;
            }
            else
            {
                KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
                        {
                    new KeyValuePair<string, string>("action",  "doSpin"),
                    new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                    new KeyValuePair<string, string>("c",       Math.Round(_defaultC, 2).ToString()),
                    new KeyValuePair<string, string>("l",       _lineCount.ToString()),
                    new KeyValuePair<string, string>("sInfo",   "n"),
                    new KeyValuePair<string, string>("index",   _index.ToString()),
                    new KeyValuePair<string, string>("counter", _counter.ToString()),
                    new KeyValuePair<string, string>("repeat",  "0"),
                    new KeyValuePair<string, string>("bl",      "0"),
                    new KeyValuePair<string, string>("mgckey",  strToken),
                        };
                return postValues;
            }
        }

        protected override KeyValuePair<string, string>[] buildDoPurSpinRequest(string strToken)
        {
            KeyValuePair<string, string>[] postValues = new KeyValuePair<string, string>[]
                    {
                    new KeyValuePair<string, string>("action",  "doSpin"),
                    new KeyValuePair<string, string>("symbol",  _strGameSymbol),
                    new KeyValuePair<string, string>("c",       Math.Round(_defaultC, 2).ToString()),
                    new KeyValuePair<string, string>("l",       _lineCount.ToString()),
                    new KeyValuePair<string, string>("sInfo",   "n"),
                    new KeyValuePair<string, string>("index",   _index.ToString()),
                    new KeyValuePair<string, string>("counter", _counter.ToString()),
                    new KeyValuePair<string, string>("repeat",  "0"),
                    new KeyValuePair<string, string>("pur",     "2"),
                    new KeyValuePair<string, string>("mgckey",  strToken),
                    };

            List<KeyValuePair<string, string>> postList = new List<KeyValuePair<string, string>>(postValues);
            if (_hasAnteBet)
                postList.Add(new KeyValuePair<string, string>("bl", "0"));
            return postList.ToArray();
        }
    }
}
