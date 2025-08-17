using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ9DemoBot
{
    public enum GameCategory
    {
        NormalGame          = 0,
        OptionGame          = 1,
        ExtraNormalGame     = 2
    }


    #region 소켓메시지들
    public class RequestReqPacket
    {
        public int      req     { get; set; }
        public string   vals    { get; set; }
    }
    public class ResponseResPacket
    {
        public int      err     { get; set; } 
        public string   msg     { get; set; }
        public int      res     { get; set; }
        public string[] vals    { get; set; }
    }
    public class RequestIrqPacket
    {
        public int      irq     { get; set; }
        public long[]   vals    { get; set; }
    }
    public class ResponseIrsPacket
    {
        public int      err     { get; set; }
        public int      irs     { get; set; }
        public string   msg     { get; set; }
        public long[]   vals    { get; set; }
    }
    public class ResponseEvtPacket
    {
        public int      evt     { get; set; }
        public double[] vals    { get; set; }
    }
    #endregion

    #region 소켓파람Object
    public class RequestInitUI
    {
        public int      Type     { get; set; }
        public int      ID       { get; set; }
        public string   GameID   { get; set; }
    }
    public class RequestInitReelSet
    {
        public int Type     { get; set; }
        public int ID       { get; set; }
        public int State    { get; set; }
    }
    public class RequestCheck
    {
        public int Type     { get; set; }
        public int ID       { get; set; }
    }
    //45번메시지 파람
    public class RequestFreeOptSelect
    {
        public int Type                 { get; set; }
        public int ID                   { get; set; }
        public int PlayerSelectState    { get; set; }
        public int PlayerSelectIndex    { get; set; }
    }
    #endregion

    #region HTTP메시지들
    public class ClientInfo
    {
        public string ip            { get; set; }
        public string code          { get; set; }
        public string datatime      { get; set; }
    }
    public class ClientInfoItem
    {
        public ClientInfo data { get; set; }
    }
    #endregion

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

    public class ExtraGameResponse: SpinResponse
    {
        public int      Extra       { get; set; }
    }

    #endregion

    #region 메시지코드
    enum MessageCode
    {
        NormalSpinRequest           = 31,
        NormalSpinResultRequest     = 32,
        NormalTembleRequest         = 33,
        FreeSpinStartRequest        = 41,
        FreeSpinRequest             = 42,
        FreeSpinSumRequest          = 43,
        FreeSpinOptionRequest       = 44,
        FreeSpinOptSelectRequest    = 45,
        FreeSpinOptResultRequest    = 46,

        NormalSpinResponse          = 131,
        NormalSpinResultResponse    = 132,
        NormalTembleResponse        = 133,
        FreeSpinStartResponse       = 141,
        FreeSpinResponse            = 142,
        FreeSpinSumResponse         = 143,
        FreeSpinOptionResponse      = 144,
        FreeSpinOptSelectResponse   = 145,
        FreeSpinOptResultResponse   = 146,
    }

    enum NextModule
    {
        Normal      = 0,
        FreeStart   = 20,
        BaseOption  = 30,
        FreeOption  = 40,
    }
    #endregion
}
