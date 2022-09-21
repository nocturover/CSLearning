using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace _04_Task_ContinueWith
{
    class Program
    {
        static void Main(string[] args)
        {
            Web.DownloadHTMLStr("https://noc.com").ContinueWith((task) => Console.WriteLine($"HTML : {task.Result}"));

            Utils.LogSumAsync((res) => { Console.WriteLine($"RandNum : {res}"); });

            Utils.TimeWriter((time, IsSuccess) => { Console.WriteLine(time); });

            Utils.FormatString("a b c d e f g", (text) => { return string.Concat(text.Where(t => !Char.IsWhiteSpace(t))); });

            Func<int[], int[]> sortNumber = (nums) => nums.OrderBy((s) => { return s; }).ToArray<int>();

            while (true) { }
        }
    }
    class Web
    {
        public static Task<string> DownloadHTMLStr(string URL)
        {
            Task<string> downloadOnWeb = Task.Run(() =>
            {
                Thread.Sleep(3000);
                return "<h1> Title Example </h1>";
            });
            return downloadOnWeb;
        }
    }
    class Utils
    {
        public delegate void SumCallback(int result);
        public static async void LogSumAsync(SumCallback Callback)
        {
            while (true)
            {
                Callback(new Random().Next(1, 100));
                await Task.Delay(3000);
            }
        }
        public static async void TimeWriter(Action<string, bool> Callback)
        {
            while (true)
            {
                Callback(DateTime.Now.ToString(), true);
                await Task.Delay(1000);
            }
        }
        public static async void FormatString(string Text, Func<string, string> Formatter)
        {
            while (true)
            {
                await Task.Delay(1000);
                Console.WriteLine(Formatter(Text));
            }
        }
    }
}
