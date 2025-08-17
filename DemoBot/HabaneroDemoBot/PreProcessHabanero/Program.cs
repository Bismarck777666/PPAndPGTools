using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessHabanero
{
    class Program
    {
        static void Main(string[] args)
        {
            SpinDBPreprocess dbProcessor = createPreprocess("LaughingBuddha");

            dbProcessor.preprocessDB().Wait();
            //dbProcessor.decreaseFreeStartWin().Wait();//옵션
            //dbProcessor.increaseFreeStartOdd().Wait();//부처님

            Console.WriteLine("{0} finished preprocess", "LaughingBuddha");
            Console.Read();
        }

        static SpinDBPreprocess createPreprocess(string strGame)
        {
            //string strFolder = "D:\\Workshop\\PragmaticPlayGames\\Server\\GITGameServerSolution2021(Slotdiamond)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\";
            string strFolder = "E:\\work\\work_by_category\\Slot\\PPServer\\GITGameServerSolution2021(Sqlite)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\";
            switch (strGame)
            {
                case "Jump":
                    return new JumpPreprocess(strFolder, strGame);
                case "Ninetails":
                    return new NinetailsPreprocess(strFolder, strGame);
                case "FiveLuckyLions":
                    return new FiveLuckyLionsProcess(strFolder, strGame);
                case "LaughingBuddha":
                    return new LaughingBuddhaProcess(strFolder, strGame);
                case "LuckyDurian":
                    return new LuckyDurianProcess(strFolder, strGame);
                case "LanternLuck":
                    return new LanternLuckProcess(strFolder, strGame);
                case "Prost":
                    return new ProstProcess(strFolder, strGame);
                case "LuckyFortuneCat":
                    return new LuckyFortuneCatProcess(strFolder, strGame);
            }
            return null;
        }
    }
}
