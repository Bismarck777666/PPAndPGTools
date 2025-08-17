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

namespace CQ9DemoBot.CQ9Fetchers
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
                case "GuGuGu3":
                case "MoveNJump":
                    newActor = Context.ActorOf(Normal9Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "GuGuGu":
                case "RaveJump2":
                case "WingChun":
                case "777":
                case "SoSweet":
                case "Super5":
                case "Apsaras":
                case "ZhongKui":
                case "FootballBaby":
                case "TreasureHouse":
                case "YuanBao":
                case "BigWolf":
                case "MonkeyOfficeLegend":
                case "DoubleFly":
                    newActor = Context.ActorOf(MiniFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "GodOfWar":
                case "JumpHigh2":
                case "ShouXin":
                case "GreekGods":
                case "AllStarTeam":
                case "SherlockHolmes":
                    newActor = Context.ActorOf(OptionGameFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "GuGuGu2":
                case "TheBeastWar":
                case "Wonderland":
                case "skrskr":
                case "HappyMagpies":
                case "DiamondTreasure":
                case "PyramidRaider":
                    newActor = Context.ActorOf(Option8Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "HotSpin":
                case "EcstaticCircus":
                case "PharaohsGold":
                    newActor = Context.ActorOf(BaseOptionMiniFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "HotPinatas":
                    newActor = Context.ActorOf(BaseOptionTembleFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "WheelMoney":
                    newActor = Context.ActorOf(WheelMoneyFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "Eight88":
                    newActor = Context.ActorOf(BaseOption8Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "GophersWar":
                case "LonelyPlanet":
                    newActor = Context.ActorOf(OptionMiniFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "CricketFever":
                case "Thor":
                case "Thor2":
                case "InvincibleElephant":
                case "FlowerFortunes":
                case "FireChibi":
                case "FireChibi2":
                case "Meow":
                case "SnowQueen":
                case "FunnyAlpaca":
                case "RaveHigh":
                case "FireQueen":
                case "FireQueen2":
                case "DiscoNight":
                case "SixCandy":
                case "TreasureBowl":
                case "WukongAndPeaches":
                case "HappyRichYear":
                case "Chameleon":
                case "WolfDisco":
                case "TreasureIsland":
                case "FortuneDragon":
                case "Poseidon":
                case "SuperDiamonds":
                case "FortuneTotem":
                case "Hephaestus":
                case "MagicWorld":
                case "GoldenEggs":
                case "Apollo":
                case "LordGanesha":
                case "GreatLion":
                case "RedPhoenix":
                case "WanbaoDino":
                case "DragonTreasure":
                case "SummerMood":
                case "SkyLanterns":
                case "DetectiveDee":
                case "AllWild":
                case "Acrobatics":
                case "Fire777":
                case "GoldStealer":
                case "SongkranFestival":
                case "SixGacha":
                case "DetectiveDee2":
                case "AladdinsLamp":
                case "RunningToro":
                case "TheCupids":
                case "OoGaChaKa":
                case "BurningXiYou":
                case "NinjaRaccoon":
                case "DollarBomb":
                case "KingKongShake":
                case "KingofAtlantis":
                case "Kronos":
                case "WolfMoon":
                case "HeroOfThreeKingdomsCaoCao":
                case "GaneshaJr":
                    newActor = Context.ActorOf(Normal8Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "FaCaiFuWa":
                    newActor = Context.ActorOf(FaCaiFuWaFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "VampireKiss":
                case "FootballBoots":
                    newActor = Context.ActorOf(Extra8Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "WaterWorld":
                case "WaterBalloons":
                case "ZumaWild":
                case "Hercules":
                case "DragonHeart":
                case "MoneyTree":
                case "SweetPop":
                case "HotDJ":
                case "SixToros":
                    newActor = Context.ActorOf(Temble8Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "SakuraLegend":
                case "RichWitch":
                    newActor = Context.ActorOf(TembleMiniFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "MyeongRyang":
                case "FootballFever":
                    newActor = Context.ActorOf(MyeongRyangFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "MirrorMirror":
                    newActor = Context.ActorOf(Temble10Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "FloatingMarket":
                    newActor = Context.ActorOf(FloatingMarketFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "LoyKrathong":
                    newActor = Context.ActorOf(LoyKrathongFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "MrMiser":
                case "NightCity":
                    newActor = Context.ActorOf(Temble9TicketFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "LuckyTigers":
                case "GB9":
                    newActor = Context.ActorOf(Normal10Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "StrikerWild":
                    newActor = Context.ActorOf(Normal10TicketFetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "GoodFortune":
                case "FaCaiShen2":
                    newActor = Context.ActorOf(Extra10Fetcher.Props(proxyIndex, _configuration), actorName);
                    _spinFetcherHashMap.Add(newActor.Path.Name);
                    break;
                case "JumpHigher":
                case "Zeus":
                case "ZeusM":
                case "LuckyBats":
                case "FaCaiShen":
                case "JumpingMobile":
                case "Eight88CaiShen":
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
