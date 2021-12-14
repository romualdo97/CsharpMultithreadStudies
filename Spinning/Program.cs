using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spinning
{
    class Program
    {
        public static bool done = false;

        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Task started");
                    Thread.Sleep(1000);
                    Console.WriteLine("Task ended");
                }
                finally
                {
                    done = true;
                }
            });

            SpinWait.SpinUntil(() => done);
            // StupidSpinning();

            Console.WriteLine("Ended");
        }

        private static void StupidSpinning ()
        {
            while (!done)
            {
                Thread.Sleep(10);
                // Wait by polling
            }
        }
    }
}
