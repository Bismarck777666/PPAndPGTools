using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmaticDemoBot
{
    public enum GameCategory
    {
        NormalGame          = 0,
        OptionGame          = 1,
        PurGame             = 2,
    }

    public enum MessageType {
        pan             = 0,
        urq             = 1,
        ilb             = 2,
        NormalSpin      = 3,
        Collect         = 4,
        FreeStart       = 5,
        FreeSpin        = 6,
        GamblePick      = 7,
        GambleHalf      = 8,
        NoCollectSpin   = 9,
        ExtendFree      = 10,
        FreeReopen      = 11,
        LastFree        = 12,
        jtr             = 13,
        tbr             = 14,
        pnj             = 15,
        FreeOption      = 16,
        WheelTrigger    = 20,
        Wheel           = 21,
        LastWheel       = 22,
        ixi             = 23,
        ltg             = 24,
        ksz             = 25,
        RespinTrigger   = 30,
        Respin          = 31,
        LastRespin      = 32,
        fej             = 33,
        FreeRespinStart = 34,   //프리스핀안에서 리스핀 시작
        FreeRespin      = 35,   //프리스핀안에서 리스핀
        FreeRespinEnd   = 36,   //프리스핀안에서 리스핀 끝
        PowerTrigger    = 37,
        PowerRespin     = 38,
        PowerLast       = 39,
        BonusTrigger    = 45,
        BonusSpin       = 46,
        BonusEnd        = 47,
        FreeCashStart   = 52,
        FreeCashSpin    = 53,
        FreeCashEnd     = 54,
        DiamondStart    = 57,
        DiamondSpin     = 58,
        DiamondEnd      = 59,
        PurBonus        = 64,
        PurFree         = 66,
        ktm             = 68
    }

    #region 스핀보관
    public class SpinResponse
    {
        public double   TotalWin    { get; set; }
        public int      SpinType    { get; set; }
        public int      LineType    { get; set; }
        public string   Response    { get; set; }
    }

    public class OptionGameResponse : SpinResponse
    {
        public double   RealWin     { get; set; }
    }

    public class PurSpinResponse : SpinResponse
    {
        public int PurIndex { get; set; }
    }

    #endregion
}
