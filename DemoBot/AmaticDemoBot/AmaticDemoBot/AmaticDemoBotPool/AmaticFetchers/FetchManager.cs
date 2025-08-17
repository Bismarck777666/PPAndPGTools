using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AmaticDemoBot.CQ9Fetchers
{
    public class FetchManager : ReceiveActor
    {
        private HashSet<string>             _spinFetcherHashMap     = new HashSet<string>();
        private Config                      _configuration          = null;
        private string                      _gameName               = "";
        private int                         _gameCategory           = -1;
        private bool                        _isShuttingDown         = false;
        private List<string>                _waitingRestartFetchers = new List<string>();

        public FetchManager(Config config)
        {
            _configuration  = config;
            _gameName       = _configuration.GetString("gameName");
            _gameCategory   = config.GetInt("gameCategory");

            Receive<FetcherStopMessage>(message => {
                if (message.IsRestart && !_waitingRestartFetchers.Contains(message.ActorName))
                    _waitingRestartFetchers.Add(message.ActorName);
                
                IActorRef actor = Context.Child(message.ActorName);
                Context.Stop(actor);
            });

            Receive<Terminated>(terminated => {
                if (_isShuttingDown)
                {
                    _spinFetcherHashMap.Remove(terminated.ActorRef.Path.Name);
                    if (_spinFetcherHashMap.Count == 0)
                        Context.Stop(Self);
                }
                else
                {
                    string actorName            = terminated.ActorRef.Path.Name;
                    if (_waitingRestartFetchers.Contains(terminated.ActorRef.Path.Name))
                    {
#if PROXY
                    int proxyIndex              = Convert.ToInt32(actorName.Substring(actorName.IndexOf("_") + 1));
                    createChildFetchActor(proxyIndex,_gameName, actorName);
#else
                        //createChildFetchActor(-1, _gameName, actorName);
                        int proxyIndex = Convert.ToInt32(actorName.Substring(actorName.IndexOf("_") + 1));
                        createChildFetchActor(proxyIndex, _gameName, actorName);
#endif
                        Context.Child(actorName).Tell("start");
                    }
                }
            });

            Receive<string>(processCommand);

            //프록시개수만큼 훼쳐액토를 만든다
            IList<string> strProxyList = _configuration.GetStringList("socks5ProxyList");
#if PROXY
                //프록시가 있는경우에 프록시개수만큼 훼쳐를 만든다
                for(int i = 0; i < strProxyList.Count; i++)
                    createChildFetchActor(i,_gameName, "SpinDataFetcher_" + i.ToString());
#else
            //프록시가 없을때는 1개만든다
            for(int i = 0; i < 2; i++)
                createChildFetchActor(i,_gameName, "SpinDataFetcher_" + i.ToString());
#endif
            foreach (string fetcherActorName in _spinFetcherHashMap)
            {
                var actor = Context.Child(fetcherActorName);
                actor.Tell("start");
            }
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new FetchManager(config));
        }

        private void processCommand(string command)
        {
            switch (command)
            {
                case "terminate":
                    Context.ActorSelection("*").Tell(PoisonPill.Instance);
                    _isShuttingDown = true;
                    break;
            }
        }

        private void createChildFetchActor(int proxyIndex, string gameName, string actorName)
        {
            IActorRef newActor = null;
            if(_gameCategory == 1)
            {
                newActor = Context.ActorOf(OptionFetcher.Props(proxyIndex, _configuration), actorName);
                _spinFetcherHashMap.Add(newActor.Path.Name);
            }
            else if (_gameCategory == 2)
            {
                //if(_gameName == "AllwaysCandy")
                //    newActor = Context.ActorOf(AllWaysCandyFetcher.Props(proxyIndex, _configuration), actorName);
                //else
                //    newActor = Context.ActorOf(AnteFetcher.Props(proxyIndex, _configuration), actorName);
                if(_gameName == "LuckyPiggies2" || _gameName == "BillysGang")
                    newActor = Context.ActorOf(LuckyPiggies2Fetcher.Props(proxyIndex, _configuration), actorName);
                else
                    newActor = Context.ActorOf(AllWaysCandyFetcher.Props(proxyIndex, _configuration), actorName);
                
                _spinFetcherHashMap.Add(newActor.Path.Name);
            }
            else
            {
                if(_gameName == "SuperCats" || _gameName == "BookOfPharao" || _gameName == "BookOfMontezuma"
                    || _gameName == "MistressOfMonsters" || _gameName == "MrMagic")
                    newActor = Context.ActorOf(PowerRespinFetcher.Props(proxyIndex, _configuration), actorName);
                else if(_gameName == "Fantastico")
                    newActor = Context.ActorOf(OldFetcher.Props(proxyIndex, _configuration), actorName);
                else if (_gameName == "BigPanda" || _gameName == "BookOfAztecSelect")
                    newActor = Context.ActorOf(Option1Fetcher.Props(proxyIndex, _configuration), actorName);
                else
                    newActor = Context.ActorOf(SpinDataFetcher.Props(proxyIndex, _configuration), actorName);
                
                _spinFetcherHashMap.Add(newActor.Path.Name);
            }
            Context.Watch(newActor);
        }
    }

    public class FetcherStopMessage
    {
        public string   ActorName       { get; set; }
        public bool     IsRestart       { get; set; }
        public FetcherStopMessage(string actorName,bool isRestart = true)
        {
            this.ActorName      = actorName;
            this.IsRestart      = isRestart;
        }
    }
}
