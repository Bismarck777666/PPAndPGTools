using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;

namespace HabaneroDemoBot.HabaneroFetcher
{
    public class FetchManager : ReceiveActor
    {
        private HashSet<string>             _spinFetcherHashMap     = new HashSet<string>();
        private Config                      _configuration          = null;
        private string                      _gameName               = "";
        private bool                        _isShuttingDown         = false;
        private List<string>                _waitingRestartFetchers = new List<string>();

        public FetchManager(Config config)
        {
            _configuration  = config;
            _gameName       = _configuration.GetString("gameName");
            
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
                        createChildFetchActor(-1, _gameName, actorName);
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
            createChildFetchActor(-1,_gameName, "SpinDataFetcher");
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
            switch (gameName)
            {
                case "LuckyBats":
                default:
                    newActor = Context.ActorOf(SpinDataFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    return;
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
