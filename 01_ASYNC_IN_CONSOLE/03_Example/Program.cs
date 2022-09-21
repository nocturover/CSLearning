using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace _03_Example
{
    class Price : IEnumerable
    {
        private ArrayList Prices;
        private double OPEN { get; set; }
        private double HIGH { get; set; }
        private double LOW { get; set; }
        private double CLOSE { get; set; }
        private int VOL { get; set; }

        public Price(List<double> prices)
        {
            OPEN = prices[0];
            HIGH = prices[1];
            LOW = prices[2];
            CLOSE = prices[3];
            Prices = new ArrayList { OPEN, HIGH, LOW, CLOSE };
        }
        public IEnumerator GetEnumerator()
        {
            return Prices.GetEnumerator();
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            //SetConsoleLog();

            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Thread : [{0}] method : {1} *main", Thread.CurrentThread.ManagedThreadId, MethodBase.GetCurrentMethod().ReflectedType.Name);
            UpdateToTable(LoadPriceDataAsync("priceTable"));
            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Thread : [{0}] method : {1} *main", Thread.CurrentThread.ManagedThreadId, MethodBase.GetCurrentMethod().ReflectedType.Name);

            while (true) { }
        }
        static async void UpdateToTable(Task<Price> price)
        {
            Price priceData = await price;
            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Thread : [{0}] method : {1}", Thread.CurrentThread.ManagedThreadId, MethodBase.GetCurrentMethod().ReflectedType.Name);

            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Prices : O,H,L,C - {0}", priceData.Cast<double>().Select(p => p.ToString()).Aggregate((x, y) => x + ", " + y));
            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Done to update");
        }
        static async Task<Price> LoadPriceDataAsync(string table)
        {
            List<object> prices = await DB.GetDataFromDB(table);
            Console.WriteLine(DateTime.Now.ToString("s")+"::" + "Thread : [{0}] method : {1}", Thread.CurrentThread.ManagedThreadId, MethodBase.GetCurrentMethod().ReflectedType.Name);

            Price res = new Price(prices.Cast<double>().ToList());
            return res;
        }

        static FileStream fs;
        static StreamWriter sw;
        static void SetConsoleLog()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                sw.Close();
                fs.Close();
            };
            fs = new FileStream("Log.txt", FileMode.Create);
            sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            Console.SetOut(sw);
            Console.SetError(sw);
        }
    }
    class DB
    {
        public async static Task<List<object>> GetDataFromDB(string table)
        {
            await Task.Delay(2000);
            return new List<object> { 2000d, 4000d, 1000d, 3000d };
        }
    }
}

