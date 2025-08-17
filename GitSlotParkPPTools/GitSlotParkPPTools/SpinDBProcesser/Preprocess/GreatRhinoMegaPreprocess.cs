using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    class MoneyStacksPreprocess : SpinDBPreprocess
    {
        public MoneyStacksPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
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
            if (freeSpinGroup == 0)
                return new int[] { 200, 201 };
            else if (freeSpinGroup == 1)
                return new int[] { 202, 203 };
            else if (freeSpinGroup == 2)
                return new int[] { 204, 205 };
            else if (freeSpinGroup == 3)
                return new int[] { 206, 207 };
            return null;
        }
    }
}
