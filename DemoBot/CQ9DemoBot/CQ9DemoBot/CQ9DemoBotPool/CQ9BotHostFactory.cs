using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ9DemoBot
{
    public class CQ9BotHostFactory
    {
        public static ActorSystem LauchCommNode(Config clusterConfig)
        {
            //액터시스템을 창조한다.
            return ActorSystem.Create("cq9boting", clusterConfig);
        }
    }
}
