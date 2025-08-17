using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    class WildBoosterPreprocess : SpinDBPreprocess
    {
        public WildBoosterPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {
#if INCHON
        _minRanges = new double[] { 15.0, 10.0, 50.0, 100.0, 300.0, 500.0, 1000.0, 1000.0, 1500.0 };
        _maxRanges = new double[] { 37.5, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0, 1500.0, 3000.0 };
#else
        _minRanges = new double[] { 15.0, 10.0, 50.0,  100.0, 300.0, 500.0,  1000.0 };
        _maxRanges = new double[] { 37.5, 50.0, 100.0, 300.0, 500.0, 1000.0, 3000.0 };
#endif

        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override int FreeSpinOptions
        {
            get { return 8; }
        }
        protected override int[] AvailableSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201 };
                case 1:
                    return new int[] { 202, 203 };
                case 2:
                    return new int[] { 204, 205 };
                case 3:
                    return new int[] { 206, 207 };
            }
            return null;
        }
    }
}
