using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabaneroDemoBot
{
    public enum GameCategory
    {
        NormalGame          = 0,
        OptionGame          = 1,
    }
    #region 스핀보관
    public class SpinResponse
    {
        public double   TotalWin    { get; set; }
        public int      SpinType    { get; set; }
        public string   Response    { get; set; }
    }

    public class OptionGameResponse : SpinResponse
    {
        public double   RealWin     { get; set; }
    }
    #endregion


}
