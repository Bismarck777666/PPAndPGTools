using PragmaticDemoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticDemoBot
{
    class ClassicGameFetcher : GameSpinDataFetcher
    {
        public ClassicGameFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, string strClientVersion, double realBet, bool hasAnteBet) :
                base(strProxyInfo, strProxyUserID, strProxyPassword, strClientVersion, realBet, hasAnteBet)
        {
            _isV3 = false;
            _isV4 = false;
        }
        protected override bool isFreeOrBonus(SortedDictionary<string, string> dicParams)
        {
            bool isFreeBonus = base.isFreeOrBonus(dicParams);
            if (isFreeBonus)
                return true;

            if (dicParams.ContainsKey("rs_p"))
                return true;
            return false;
        }
        protected override async Task<List<SpinResponse>> doSpin(HttpClient httpClient, string strToken)
        {
            List<string>        strResponseHistory  = new List<string>();
            List<SpinResponse>  responseList        = new List<SpinResponse>();
            string              strResponse         = "";

            try
            {
                strResponse = await sendSpinRequest(httpClient, strToken);
                SortedDictionary<string, string> dicParamValues = splitAndRemoveCommonResponse(strResponse);
                strResponseHistory.Add(combineResponse(dicParamValues));
                SpinResponse response = new SpinResponse();
                response.SpinType = 0;
                response.TotalWin = double.Parse(dicParamValues["w"]);
                response.Response = string.Join("\n", strResponseHistory);
                responseList.Add(response);
                return responseList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                return null;
            }
        }
    }
}
