using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmaticDemoBot.BitReader
{
    public class AmaDecrypt
    {
        public int      _pointIndex = 0;
        public string   _strMessage = "";

        public AmaDecrypt()
        {
        }

        public void setMessage(string message)
        {
            _pointIndex = 0;
            _strMessage = message;
        }

        private long hexStringToLong(string strHex)
        {
            long n = Int64.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
            return n;
        }

        public long Read1BitHexToDec()
        {
            string strHex = _strMessage.Substring(_pointIndex, 1);
            _pointIndex++;

            return hexStringToLong(strHex);
        }

        public long Read2BitHexToDec()
        {
            string strHex = _strMessage.Substring(_pointIndex, 2);
            _pointIndex += 2;

            return hexStringToLong(strHex);
        }

        public long ReadLengthAndDec()
        {
            string strLengthHex = _strMessage.Substring(_pointIndex, 1);
            int length = (int)hexStringToLong(strLengthHex);
            _pointIndex++;

            string strHex = _strMessage.Substring(_pointIndex, length);
            _pointIndex += length;

            return hexStringToLong(strHex);
        }

        public List<long> ReadNumArray()
        {
            List<long> numArray = new List<long>();
            long arrayLength = ReadLengthAndDec();

            for(long i = 0; i < arrayLength; i++)
            {
                long num = Read1BitHexToDec();

                numArray.Add(num);
            }

            return numArray;
        }

        public string ReadLastLeftString()
        {
            return _strMessage.Substring(_pointIndex);
        }
    }

    public class AmaEncrypt
    {
        private string longToHexString(long num)
        {
            return string.Format("{0:X}", num);
        }
        
        public string WriteDecHex(string message, long num)
        {
            return string.Format("{0}{1}", message, longToHexString(num));
        }

        public string WriteDec2Hex(string message, long num)
        {
            string strNum = longToHexString(num);
            if (strNum.Length == 1)
                strNum = string.Format("0{0}", strNum);

            return string.Format("{0}{1}", message, strNum);
        }

        public string WriteLengthAndDec(string message, long num)
        {
            string numStr = longToHexString(num);
            string lenStr = longToHexString(numStr.Length);

            return string.Format("{0}{1}{2}", message, lenStr, numStr);
        }

        public string Write1BitNumArray(string message, List<long> numArray)
        {
            int numArrayLength = numArray.Count;
            message = WriteLengthAndDec(message, numArrayLength);
            
            for (int i = 0; i < numArrayLength; i++)
            {
                message = WriteDecHex(message, numArray[i]);
            }

            return message;
        }

        public string Write2BitNumArray(string message, List<long> numArray)
        {
            int numArrayLength = numArray.Count;
            message = WriteLengthAndDec(message, numArrayLength);

            for (int i = 0; i < numArrayLength; i++)
            {
                message = WriteDec2Hex(message, numArray[i]);
            }

            return message;
        }

        public string WriteLeftHexString(string message, string strHex)
        {
            return string.Format("{0}{1}", message, strHex);
        }
    }

    public class AmaPacket
    {
        public long         messageheader   { get; set; }
        public long         messagetype     { get; set; }
        public long         sessionclose    { get; set; }
        public long         messageid       { get; set; }
        public long         balance         { get; set; }
        public long         win             { get; set; }
        public List<long>   reelstops       { get; set; }
        public long         betstep         { get; set; }
        public long         betline         { get; set; }
        public List<long>   unknowparam3    { get; set; }
        public List<long>   freereelstops   { get; set; }
        public List<long>   linewins        { get; set; }
        public List<long>   gamblelogs      { get; set; }

        public AmaPacket(string message, int reelCnt, int freeReelCnt)
        {
            AmaDecrypt amaConverter = new AmaDecrypt();
            amaConverter.setMessage(message);

            messageheader   = amaConverter.Read1BitHexToDec();
            messagetype     = amaConverter.Read2BitHexToDec();
            sessionclose    = amaConverter.Read1BitHexToDec();
            messageid       = amaConverter.ReadLengthAndDec();
            balance         = amaConverter.ReadLengthAndDec();
            win             = amaConverter.ReadLengthAndDec();

            reelstops = new List<long>();
            for(int i = 0; i < reelCnt; i++)
            {
                reelstops.Add(amaConverter.ReadLengthAndDec());
            }
            betstep    = amaConverter.Read2BitHexToDec();
            betline         = amaConverter.Read2BitHexToDec();

            unknowparam3 = new List<long>();
            for(int i = 0; i < 6; i++)
            {
                unknowparam3.Add(amaConverter.ReadLengthAndDec());
            }

            freereelstops = new List<long>();
            for (int i = 0; i < freeReelCnt; i++)
            {
                freereelstops.Add(amaConverter.ReadLengthAndDec());
            }

            linewins = new List<long>();
            long lineCnt = amaConverter.Read2BitHexToDec();
            for(int i = 0; i < lineCnt; i++)
            {
                linewins.Add(amaConverter.ReadLengthAndDec());
            }

            gamblelogs = new List<long>();
            for (int i = 0; i < 8; i++)
            {
                //gamblelogs.Add(amaConverter.Read2BitHexToDec());
                gamblelogs.Add(0);
            }
        }
    }
}
