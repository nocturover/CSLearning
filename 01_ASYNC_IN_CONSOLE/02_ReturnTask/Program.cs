using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace _02_ReturnTask
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //int result = await SumAsync(1, 2);
            int result = SumAsync(1, 2).GetAwaiter().GetResult();

            Console.WriteLine("\tresult = " + result);

            await DownloadImageAsync(1, 2);

            DownloadImageAsyncVoid(1, 2);

            while (true)
            {}
        }

        static async Task<int> SumAsync(int a, int b)
        {
            await Task.Delay(1000);
            string method = MethodBase.GetCurrentMethod().ReflectedType.Name;
            Console.Write($"call [{method}]");
            return a + b;
        }
        static async Task DownloadImageAsync(int width, int height)
        {
            await Task.Delay(1000);
            string method = MethodBase.GetCurrentMethod().ReflectedType.Name;
            Console.WriteLine($"call [{method}]");
        }
        static async void DownloadImageAsyncVoid(int width, int height)
        {
            await Task.Delay(1000);
            string method = MethodBase.GetCurrentMethod().ReflectedType.Name;
            Console.WriteLine($"call [{method}]");
        }
    }
}
