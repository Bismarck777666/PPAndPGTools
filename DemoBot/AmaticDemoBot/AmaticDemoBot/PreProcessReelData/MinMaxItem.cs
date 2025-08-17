using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessReelData
{
    public class MinMaxItem
    {
        public double MinOdd { get; set; }
        public double MaxOdd { get; set; }
        public MinMaxItem(double min, double max)
        {
            this.MinOdd = min;
            this.MaxOdd = max;
        }
    }
}
