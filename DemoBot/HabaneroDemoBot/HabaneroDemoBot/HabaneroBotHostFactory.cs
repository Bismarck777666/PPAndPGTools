using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;

namespace HabaneroDemoBot
{
    public class HabaneroBotHostFactory
    {
        public static ActorSystem LauchCommNode(Config clusterConfig)
        {
            //액터시스템을 창조한다.
            return ActorSystem.Create("habaneroboting", clusterConfig);
        }
    }
}
