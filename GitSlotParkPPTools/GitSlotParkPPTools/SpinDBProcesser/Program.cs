using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace SpinDBProcesser
{
    class Program
    {

        static void Main(string[] args)
        {
            SpinDBPreprocess dbProcessor = createPreprocess("VolcanoGoddess");
            dbProcessor.readInfo().Wait();           
            //dbProcessor.calculateNormalFreeSpinRate().Wait();
            dbProcessor.calculateAllFreeSpinRate().Wait();
            dbProcessor.preprocessDB().Wait();
            //(dbProcessor as PowerOfThorMegaPreprocess).setRanges().Wait();

            //FruitPartyPreprocess dbProcessor = createPreprocess("FruitParty") as FruitPartyPreprocess;
            //(dbProcessor as OlympusGatePreprocess).setupPurEnabled().Wait();
            //dbProcessor.setupPurEnabled().Wait();

            //SpinDataChecker.doCheck().Wait();
        }

        static SpinDBPreprocess createPreprocess(string strGame)
        {
            //string strFolder = "D:\\Workshop\\PragmaticPlayGames\\Server\\GITGameServerSolution2021(Slotdiamond)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\";
            //string strFolder = "E:\\work\\work_by_category\\Slot\\PPServer\\GITGameServerSolution2021(Sqlite)\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\";
            string strFolder = "";
            switch (strGame)
            {
                case "AztecKingMega":
                    return new AztecKingMegaPreprocess(strFolder, strGame);
                case "BroncoSpirit":
                    return new BroncoSpiritPreprocess(strFolder, strGame);
                case "EightDragons":
                    return new EightDragonsPreprocess(strFolder, strGame);
                case "HandOfMidas":
                    return new HandOfMidasPreprocess(strFolder, strGame);
                case "FruitParty":
                    return new FruitPartyPreprocess(strFolder, strGame);
                case "PowerOfThorMega":
                    return new PowerOfThorMegaPreprocess(strFolder, strGame);
                case "OlympusGates":
                case "PyramidBonanza":
                    return new OlympusGatePreprocess(strFolder, strGame);
                case "FiveLionsGold":
                    return new FiveLionsGoldPreprocess(strFolder, strGame);
                case "FiveLionsMega":
                    return new FiveLionsMegaPreprocess(strFolder, strGame);
                case "TheDogHouseMega":
                    return new TheDogHouseMegaPreprocess(strFolder, strGame);
                case "FiveLions":
                    return new FiveLionsPreprocess(strFolder, strGame);
                case "GreatRhinoMega":
                    return new GreatRhinoMegaPreprocess(strFolder, strGame);
                case "WildBooster":
                    return new WildBoosterPreprocess(strFolder, strGame);
                case "GuGuGu2":
                    return new GuGuGu2Preprocess(strFolder, strGame);
                case "Thor2":
                    return new Thor2Preprocess(strFolder, strGame);
                case "BigBassMissionFishin":
                case "VolcanoGoddess":
                    return new BigBassMissionFishinPreprocess(strFolder, strGame);

                case "MoneyStacks":
                    return new MoneyStacksPreprocess(strFolder, strGame);

            }
            return null;
        }       
    }
}
