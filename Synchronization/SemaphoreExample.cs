using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Synchronization
{
    class SemaphoreExample
    {
        public static SemaphoreSlim Bouncer { get; set; }

        public static void Start()
        {
            // Create the semaphore with 3 slots, where 3 are available.
            Bouncer = new SemaphoreSlim(3, 3);

            OpenNightClub();
        }

        private static void OpenNightClub()
        {
            for (int i = 1; i <= 50; i++)
            {
                int number = i;
                Task.Factory.StartNew(() =>
                {
                    Guest(number);
                });
            }
        }

        private static void Guest(int number)
        {
            // Wait to enter the nighclup (a semaphore to be released)
            Console.WriteLine("Guest {0:00} is waiting to enter nightclup.", number);
            Bouncer.Wait();

            // Do some dancing
            Console.WriteLine("Guest {0:00} is doing some dancing.", number);
            Thread.Sleep(5000);

            // Let one guest out (release one semaphore)
            Console.WriteLine("Guest {0:00} is releasing the nightclub.", number);
            Bouncer.Release();
        }
    }
}
