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
    public class WolfPowerMegaDataFetcher : PlaysonSpinDataFetcher
    {
        public WolfPowerMegaDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword,bool isBuy) : base(strProxyInfo, strProxyUserID, strProxyPassword,isBuy)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _isBuy          = isBuy;
        }

        protected override async Task<string> sendSpinRequest(HttpClient httpClient, string serverUrl, string nextAction,bool isBuy = false)
        {
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

                if (isBuy)
                {
                    XmlElement buyNode  = xml.CreateElement("buy");
                    buyNode.SetAttribute("bonus_game", "true");
                    root.AppendChild(buyNode);
                }

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

        protected virtual async Task<string> sendBonusRequest(HttpClient httpClient, string serverUrl, string nextAction, int roundNum)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                
                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session",        _session);
                root.SetAttribute("prnd",           _prnd);
                root.SetAttribute("rnd",            DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command",        "bonus");

                XmlElement action      = xml.CreateElement("action");
                action.SetAttribute("round",        roundNum.ToString());

                root.AppendChild(action);
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
            int     roundNumber     = 0;
            double  spinWin         = 0.0;
            bool    nowFreeSpin     = false;
            try
            {
                do
                {
                    if (nextAction == "bonus")
                    {
                        strResponse = await sendBonusRequest(httpClient, serverUrl, nextAction, roundNumber);
                        roundNumber++;
                    }
                    else
                        strResponse = await sendSpinRequest(httpClient, serverUrl, nextAction,_isBuy);

                    XmlDocument resultDoc = new XmlDocument();
                    resultDoc.LoadXml(strResponse);

                    XmlNode serverNode  = resultDoc.SelectSingleNode("/server");
                    _prnd               = serverNode.Attributes["rnd"].Value;
                    
                    XmlNode stateNode   = resultDoc.SelectSingleNode("/server/state");
                    string state        = stateNode.Attributes["current_state"].Value;
                    nextAction          = state;

                    if(resultDoc.SelectSingleNode("/server/user_new") != null)
                    {
                        XmlNode userNode = resultDoc.SelectSingleNode("/server/user_new");
                        _userBalance = Convert.ToInt64(userNode.Attributes["cash"].Value);
                        serverNode.RemoveChild(userNode);
                    }

                    SpinData spinData = new SpinData();
                    strResponseHistory.Add(resultDoc.InnerXml);

                    XmlNode gameNode        = resultDoc.SelectSingleNode("/server/game");
                    XmlNode gameBonusNode   = resultDoc.SelectSingleNode("/server/game/bonus");
                    XmlNode bonusNode       = resultDoc.SelectSingleNode("/server/bonus");
                    if(gameNode != null && gameNode.Attributes["cash-win"] != null)
                        spinWin += Convert.ToDouble(gameNode.Attributes["cash-win"].Value);
                    if (gameBonusNode != null && gameBonusNode.Attributes["win"] != null)
                        if (nextAction != "bonus")
                            spinWin += Convert.ToDouble(gameBonusNode.Attributes["win"].Value);
                    if (bonusNode != null && bonusNode.Attributes["win"] != null)
                        if (nextAction != "bonus")
                            spinWin += Convert.ToDouble(bonusNode.Attributes["win"].Value);
                    
                    if (state == "idle")
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = _isBuy ? 1 : 0;
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
