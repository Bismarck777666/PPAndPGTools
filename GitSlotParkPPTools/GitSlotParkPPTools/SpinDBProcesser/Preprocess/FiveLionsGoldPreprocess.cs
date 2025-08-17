using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    class FiveLionsGoldPreprocess : SpinDBPreprocess
    {
        public FiveLionsGoldPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override int FreeSpinOptions
        {
            get { return 7; }
        }
    }
}
