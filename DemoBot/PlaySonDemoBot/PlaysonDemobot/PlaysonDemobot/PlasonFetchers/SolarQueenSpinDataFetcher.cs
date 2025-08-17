using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PlaysonDemobot
{
    public class SolarQueenSpinDataFetcher : PlaysonSpinDataFetcher
    {
        public SolarQueenSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,bool isBuy) : base(strProxyInfo, strProxyUserID, strProxyPassword,isBuy)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _isBuy          = isBuy;
        }

        protected override async Task<string> sendSpinRequest(HttpClient httpClient, string serverUrl, string nextAction, bool isBuy = false)
        {
            //if (_prnd == "")
            //    return await base.sendSpinRequest(httpClient, serverUrl, nextAction);

            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session",        _session);
                root.SetAttribute("prnd",           _prnd);
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "bet");

                XmlElement bet      = xml.CreateElement("bet");
                bet.SetAttribute("cash",            _defaultBet.ToString());

                XmlElement debug    = xml.CreateElement("debug");
                debug.SetAttribute("user_cash",     (_userBalance - _realBet).ToString());
                debug.SetAttribute("bet_cash",      _realBet.ToString());

                root.AppendChild(bet);
                root.AppendChild(debug);
                xml.AppendChild(root);

                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse  = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();

                return strResponse;
            }
            catch (Exception ex)
            {
                _session = string.Empty;
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        protected override async Task<List<SpinData>> doSpin(HttpClient httpClient, string serverUrl)
        {
            List<string> strResponseHistory = new List<string>();
            List<SpinData> responseList     = new List<SpinData>();
            string  strResponse     = "";
            string  nextAction      = null;
            int     spinType        = 0;
            double  spinWin         = 0.0;
            bool    nowFreeSpin     = false;
            try
            {
                do
                {
                    strResponse = await sendSpinRequest(httpClient, serverUrl, nextAction);

                    XmlDocument resultDoc = new XmlDocument();
                    resultDoc.LoadXml(strResponse);

                    XmlNode serverNode  = resultDoc.SelectSingleNode("/server");
                    _prnd               = serverNode.Attributes["rnd"].Value;
                    
                    XmlNode stateNode   = resultDoc.SelectSingleNode("/server/state");
                    string state        = stateNode.Attributes["current_state"].Value;
                    
                    if(resultDoc.SelectSingleNode("/server/user_new") != null)
                    {
                        XmlNode userNode = resultDoc.SelectSingleNode("/server/user_new");
                        _userBalance = Convert.ToInt64(userNode.Attributes["cash"].Value);
                        serverNode.RemoveChild(userNode);
                    }

                    if (state == "fs")
                    {
                        nowFreeSpin = true;
                    }

                    SpinData spinData = new SpinData();
                    strResponseHistory.Add(resultDoc.InnerXml);

                    XmlNode gameNode = resultDoc.SelectSingleNode("/server/game");
                    spinWin += Convert.ToDouble(gameNode.Attributes["cash-win"].Value);
                    XmlNode featureNode         = resultDoc.SelectSingleNode("/server/game/feature_rounds_progress");
                    int featureRoundsProgress   = Convert.ToInt32(featureNode.InnerText);
                    
                    if (featureRoundsProgress != 10)
                        continue;
                    
                    if(gameNode.Attributes["original_bonus_games"].Value == "0" || (gameNode.Attributes["current_freespin_number"] != null &&
                        gameNode.Attributes["original_bonus_games"].Value == gameNode.Attributes["current_freespin_number"].Value))
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = nowFreeSpin ? 1 : 0;
                        spinResponse.SpinOdd    = spinWin / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        nowFreeSpin = false;
                        return responseList;
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(string.Join("\n", strResponseHistory.ToArray()));
                _session = string.Empty;
            }
            return null;
        }
    }
}
