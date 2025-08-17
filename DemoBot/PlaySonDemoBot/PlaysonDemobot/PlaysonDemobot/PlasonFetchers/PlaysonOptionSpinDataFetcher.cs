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
    public class PlaysonOptionSpinDataFetcher : PlaysonSpinDataFetcher
    {
        protected int       FreeSpinOptionCount = 0;
        protected Random    _random             = new Random((int)DateTime.Now.Ticks);
        protected List<int> _freeOptions        = new List<int>();
        protected int       _freeOptionIndex     = -1;
        public PlaysonOptionSpinDataFetcher(string strProxyInfo, string strProxyUserID, string strProxyPassword, bool isBuy,IList<int> optionList) : base(strProxyInfo, strProxyUserID, strProxyPassword, isBuy)
        {
            _proxyInfo      = strProxyInfo;
            _proxyUserID    = strProxyUserID;
            _proxyPassword  = strProxyPassword;
            _isBuy          = isBuy;
            foreach(int option in optionList)
                _freeOptions.Add(option);
        }

        protected virtual async Task<string> sendOptionRequest(HttpClient httpClient, string serverUrl, string nextAction)
        {
            try
            {
                XmlDocument xml = new XmlDocument();

                XmlElement root = xml.CreateElement("client");
                root.SetAttribute("session", _session);
                root.SetAttribute("rnd", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                root.SetAttribute("command", "freespin");

                XmlElement freespin = xml.CreateElement("freespin");
                freespin.SetAttribute("type", (600 + _freeOptions[_freeOptionIndex]).ToString());

                root.AppendChild(freespin);
                xml.AppendChild(root);

                var responseMessage = await httpClient.PostAsync(serverUrl, new StringContent(xml.InnerXml, Encoding.UTF8, "application/x-www-form-urlencoded"));
                string strResponse = await responseMessage.Content.ReadAsStringAsync();
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
            List<SpinData> responseList = new List<SpinData>();
            string strResponse      = "";
            string nextAction       = null;
            double spinWin          = 0.0;
            bool nowFreeSpin        = false;
            double freeTriggerWin   = 0;
            
            try
            {
                do
                {
                    if (nextAction == "fs_choice")
                    {
                        _freeOptionIndex    = _random.Next(0, _freeOptions.Count);
                        freeTriggerWin      = spinWin;
                        spinWin             = 0;
                        strResponse = await sendOptionRequest(httpClient, serverUrl, nextAction);
                    }
                    else
                        strResponse = await sendSpinRequest(httpClient, serverUrl, nextAction);

                    XmlDocument resultDoc = new XmlDocument();
                    resultDoc.LoadXml(strResponse);

                    XmlNode serverNode = resultDoc.SelectSingleNode("/server");
                    _prnd = serverNode.Attributes["rnd"].Value;

                    XmlNode stateNode = resultDoc.SelectSingleNode("/server/state");
                    string state = stateNode.Attributes["current_state"].Value;
                    nextAction = state;

                    if (resultDoc.SelectSingleNode("/server/user_new") != null)
                    {
                        XmlNode userNode = resultDoc.SelectSingleNode("/server/user_new");
                        _userBalance = Convert.ToInt64(userNode.Attributes["cash"].Value);
                        serverNode.RemoveChild(userNode);
                    }

                    if (state == "fs_choice")
                        nowFreeSpin = true;

                    SpinData spinData = new SpinData();
                    strResponseHistory.Add(resultDoc.InnerXml);

                    XmlNode gameNode    = resultDoc.SelectSingleNode("/server/game");
                    XmlNode freeNode    = resultDoc.SelectSingleNode("/server/freespin");

                    if (gameNode != null && gameNode.Attributes["cash-win"] != null)
                    {
                        if(!(freeNode != null && freeNode.Attributes["type"] != null))
                            spinWin += Convert.ToDouble(gameNode.Attributes["cash-win"].Value);
                    }

                    if(state == "fs_choice")
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = 100;
                        spinResponse.SpinOdd    = spinWin / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);

                        strResponseHistory = new List<string>();
                    }

                    if (state == "idle")
                    {
                        SpinData spinResponse = new SpinData();
                        spinResponse.SpinType   = nowFreeSpin ? 200 + _freeOptions[_freeOptionIndex] : 0;
                        spinResponse.SpinOdd    = spinWin / _realBet;
                        spinResponse.RealOdd    = spinResponse.SpinOdd;
                        spinResponse.Response   = string.Join("\n", strResponseHistory);
                        responseList.Add(spinResponse);
                        nowFreeSpin         = false;
                        _freeOptionIndex    = -1;
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
