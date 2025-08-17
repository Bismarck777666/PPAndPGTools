using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;

namespace HabaneroDemoBot.HabaneroFetcher
{
    public class SpinDataFetcher: ReceiveActor
    {
        protected int                       _proxyIndex             = -1;
        protected IList<string>             _httpProxyList          = new List<string>();
        protected IList<string>             _socks5ProxyList        = new List<string>();
        protected string                    _proxyUserID            = null;
        protected string                    _proxyPassword          = null;
        protected string                    _gameName               = "";
        protected string                    _gameID                 = "";
        protected string                    _userID                 = null;
        protected int                       _playbet                = 2;
        protected int                       _playdenom              = 100;
        protected int                       _playmini               = 30;
        protected int                       _playline               = 1;
        protected double                    _realBet                = 0.0;
        protected double                    _meanOdd                = 0.0;
        protected string                    _gameUrl                = null;

        protected DateTime                  _LastMessageTime        = new DateTime(1970, 1, 1);
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);


        protected ICancelable               _heartbeatCancelable    = null;
        protected IActorRef                 _self                   = null;
        protected bool                      _nowFreeSpin            = false;
        protected List<string>              _freeSpinStack          = null;
        protected List<string>              _tembleSpinStack        = null;
        protected int                       _cnt                    = 0;    //한세션동안 넣는 스핀번호(최대 99,999,999이상)

        public SpinDataFetcher(int proxyIndex, Config config)
        {
            _proxyIndex         = proxyIndex;
            _httpProxyList      = config.GetStringList("httpProxyList");
            _socks5ProxyList    = config.GetStringList("socks5ProxyList");
            _proxyUserID        = config.GetString("proxyUserID");
            _proxyPassword      = config.GetString("proxyPassword");
            _gameName           = config.GetString("gameName");
            _gameID             = config.GetString("gameID");
            _playbet            = config.GetInt("playbet");   
            _playdenom          = config.GetInt("playdenom"); 
            _playmini           = config.GetInt("playmini");  
            _playline           = config.GetInt("playline");  
            _realBet            = config.GetDouble("realBet");
            _userID             = config.GetString("token");
            _gameUrl            = config.GetString("gameURL");
            _meanOdd            = config.GetDouble("minOdd");
            _self               = Self;
        }

        public static Props Props(int proxyIndex, Config config)
        {
            return Akka.Actor.Props.Create(() => new SpinDataFetcher(proxyIndex, config));
        }

    }
}
