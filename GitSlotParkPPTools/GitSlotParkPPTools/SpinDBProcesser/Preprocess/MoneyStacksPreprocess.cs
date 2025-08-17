using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    class GreatRhinoMegaPreprocess : SpinDBPreprocess
    {
        public GreatRhinoMegaPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override int FreeSpinOptions
        {
            get { return 12; }
        }
        protected override int[] AvailableSpinTypes(int freeSpinGroup)
        {
            if (freeSpinGroup == 0)
                return new int[] { 200, 201, 202, 203 };
            else if (freeSpinGroup == 1)
                return new int[] { 204, 205, 206, 207 };
            else if (freeSpinGroup == 2)
                return new int[] { 208, 209, 210, 211 };
            return null;
        }
    }
}
