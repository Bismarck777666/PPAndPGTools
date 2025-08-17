using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinDBProcesser
{
    public class AztecKingMegaPreprocess : SpinDBPreprocess
    {
        public AztecKingMegaPreprocess(string strFolderName, string strGameName) : base(strFolderName, strGameName)
        {

        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override int FreeSpinOptions
        {
            get { return 4; }
        }
    }
}
