using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPReplayLinkFix
{
    class RSAPublicKeyUpdate
    {
        private static RSAPublicKeyUpdate _instance   = new RSAPublicKeyUpdate();
        public static RSAPublicKeyUpdate Instance    => _instance;

        public void updateBuildJsFile(List<string> gameSymbols)
        {
            foreach (var gameSymbol in gameSymbols)
            {
                try
                {
                    string[] platForms = new string[2] { "desktop", "mobile" };
                    for (int i = 0; i < platForms.Length; i++)
                    {
                        string strFilePath      = string.Format("vs/{0}/{1}/build.js", gameSymbol, platForms[i]);
                        string strFileContent   = File.ReadAllText(strFilePath);

                        //strFileContent = doChangeRSAPublicKey(gameSymbol, platForms[i], strFileContent);
                        //if (!strFileContent.Contains("RSAPublicKey"))
                        //    Console.WriteLine("{0} <-----> {1}", gameSymbol, platForms[i]);

                        strFileContent = doChangeFrbAvaliable(gameSymbol, platForms[i], strFileContent);

                        File.WriteAllText(strFilePath, strFileContent);
                    }
                    Console.WriteLine("{0} ------ build.js file update finished!", gameSymbol);
                }
                catch (Exception ex) 
                { 
                    
                }
            }
            Console.ReadLine();
        }

        public string doChangeRSAPublicKey(string strSymbol, string strPlatform, string strFileContent)
        {
            try
            {
                if (strFileContent.Contains("RSAPublicKey"))
                    return strFileContent;

                int bufIndex        = strFileContent.IndexOf("=!![]);try{return ");
                int rsakeyEndIndex  = strFileContent.LastIndexOf(");},", bufIndex);

                string oldRSAKey = strFileContent.Substring(rsakeyEndIndex - 9, 9);
                strFileContent = ReplaceAt(strFileContent, rsakeyEndIndex - 9, 9, "RSAPublicKey");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has been occured in {0} {1}", strSymbol, strPlatform);
            }

            return strFileContent;
        }

        public string doChangeFrbAvaliable(string strSymbol, string strPlatform, string strFileContent) 
        {
            try
            {
                string strKey       = "/gs2c/promo/frb/available/";
                string strReplace   = "/gitapi/pp/promo/frb/available/";
                if (strFileContent.Contains(strReplace))
                    return strFileContent;

                if (!strFileContent.Contains(strKey))
                    return strFileContent;

                int frIndex     = strFileContent.IndexOf(strKey);
                strFileContent  = ReplaceAt(strFileContent, frIndex, strKey.Length, strReplace);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has been occured in {0} {1}", strSymbol, strPlatform);
            }

            return strFileContent;
        }

        public string ReplaceAt(string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }
    }
}
