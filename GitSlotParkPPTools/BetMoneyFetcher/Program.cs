using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BetMoneyFetcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //FetchWorker.Instance.fetchBetMoneyInfo("USD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("TND", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("INR", 0.5).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MAD", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BRL", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("IDR", 10.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("AUD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("CHF", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("NOK", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("THB", 0.3).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MYR", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("XOF", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("NGN", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("TVD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("RUB", 0.01).Wait();     //루불2(마우로서버)
            //FetchWorker.Instance.fetchBetMoneyInfo("ILS", 0.05).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("COP", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("RT", 0.3).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BWP", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("AED", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("UAH", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("UYU", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("IQD", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("EGP", 0.5).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("AZN", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("PKR", 1.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("KZT", 1.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("UZS", 100.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("CAD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("CNY", 0.05).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("PLN", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("HUF", 1.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("NZD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("CLP", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("CZK", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GEL", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("RON", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BAM", 0.02).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("TOPIA", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("SYP", 10.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("SEK", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("ZAR", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GBP", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GOF", 10.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("KES", 0.5).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("ZWL", 50.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("ZMW", 0.05).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("AOA", 0.5).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MZN", 0.05).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("NAD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("PYG", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BOB", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BDT", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("DKK", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("PEN", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("ISK", 0.5).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("IRR", 5000.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MNT", 5.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("TMT", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("IRT", 500.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("HKD", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("JPY", 1.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("SGD", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("VND", 100.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("UGX", 10.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("TZS", 10.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GHS", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("RSD", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("HRK", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("XAF", 500).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GNF", 500).Wait();
            FetchWorker.Instance.fetchBetMoneyInfo("TRY", 0.1).Wait();
            FetchWorker.Instance.fetchBetMoneyInfo("ARS", 0.5).Wait();      //아젠티나 페소1
            //FetchWorker.Instance.fetchBetMoneyInfo("PHP", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MXN", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("DZD", 0.1).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("EUR", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("LBP", 500.0).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("MMK", 1.0).Wait();

            //FetchWorker.Instance.fetchBetMoneyInfo("VES", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("BYN", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("LKR", 0.05).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("JC", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("GC", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("SC", 0.01).Wait();
            //FetchWorker.Instance.fetchBetMoneyInfo("ENT", 0.1).Wait();



            //FetchWorker.Instance.fetchBetMoneyInfo("RUB", 0.5).Wait();      //루불1
            ////FetchWorker.Instance.fetchTRYBetMoneyInfo("TRY").Wait();
            ////FetchWorker.Instance.fetchARSBetMoneyInfo("ARS").Wait();      //아젠티나 페소2(마우로서버)
            ////FetchWorker.Instance.fetchPHPBetMoneyInfo("PHP").Wait();
            ////FetchWorker.Instance.fetchPHPBetMoneyInfo("MXN").Wait();
            ////FetchWorker.Instance.fetchDZDBetMoneyInfo("DZD").Wait();        //PHP * 10
            //FetchWorker.Instance.fetchNewEURBetMoneyInfo("EUR").Wait();
            ////FetchWorker.Instance.fetchNewLBPBetMoneyInfo("LBP").Wait();
            //FetchWorker.Instance.fetchNewMMKBetMoneyInfo("MMK").Wait();


            FetchWorker.Instance.fetchRTPInfo("USD").Wait();
            Console.ReadLine();
        }
    }
}
