#define Args
using System.Threading;
using System.Threading.Tasks;

namespace REVIEW_CSHARP_BASIC
{
    public partial class AsyncExample : Form
    {
        public AsyncExample()
        {
            InitializeComponent();
            string str = "hello, world!";
            Console.WriteLine(str.Substring(7));
            Console.WriteLine(str.Substring(7, 6));
#if Async
            StartWriteLineAsync();
#endif
#if Ordinary
            StartWriteLineOrdinaryTask();
#endif
#if Args
            StartWriteLineWitheArgs();
#endif
            Console.WriteLine("[Log] Done to AsyncTask");
        }
        private async void StartWriteLineAsync()
        {
            for (int idx = 0; idx < 10; idx++)
            {
                Console.WriteLine("Hello, World!");
                await Task.Delay(100);
            }
        }
        private void StartWriteLineOrdinaryTask()
        {
            int[] retArr = new int[10];
            Action action = () =>
            {
                foreach (int i in retArr)
                {
                    Console.WriteLine("Ordinary Task!");
                    Thread.Sleep(100);
                }
            };
            Task task = new Task(action);
            task.Start();
            task.Wait();
        }
        private void StartWriteLineWitheArgs()
        {
            Action action = () =>
            {
                for (int idx = 0; idx < 10; idx++)
                {
                    WriteLineWithArgs("New Message");
                    Thread.Sleep(100);
                }
            };
            Task task = new Task(action);
            task.Start();
            task.Wait();

            void WriteLineWithArgs(string msg)
            {
                Console.WriteLine(msg);
            }

            // Another way
            Task.Run(() =>
            {
                for (int idx = 0; idx < 10; idx++)
                {
                    WriteLineWithArgs("Direct Input args");
                    Thread.Sleep(100);
                }
            });
        }
    }
}