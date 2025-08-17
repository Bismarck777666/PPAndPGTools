using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBConvert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //MongoDBConvert.Instance.correctMedusa2DB().Wait();
            MongoDBConvert.Instance.convertTask().Wait();
        }
    }
}
