using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASYNC_IN_CONSOLE
{
    class Program
    {
        const int LoopCnt = 10;

        static void Main(string[] args)
        {
            for (int idx = 0; idx < 3; idx++)
            {
                StartSubThread();
            }
            for (int cnt = 0; cnt < LoopCnt; cnt++)
            {
                Thread.Sleep(100);
                Console.WriteLine("Main Thread : {0}", cnt);
            }
        }
        private static async void StartSubThread()
        {
            for (int cnt = 0; cnt < LoopCnt; cnt++)
            {
                await Task.Delay(100);
                Console.WriteLine("Sub Thread[{0}] : {1}", Thread.CurrentThread.ManagedThreadId, cnt);

            }
        }
    }
}
