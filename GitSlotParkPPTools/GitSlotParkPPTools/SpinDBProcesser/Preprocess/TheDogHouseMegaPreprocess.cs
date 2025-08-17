using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    class TheDogHouseMegaPreprocess : SpinDBPreprocess
    {
        public TheDogHouseMegaPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

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
