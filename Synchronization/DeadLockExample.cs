using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
    class DeadLockExample
    {
        private static object firstLock = new object();
        private static object secondLock = new object();

        public static void Start()
        {
            Task.Run(DoSomeWork);
            
            Console.WriteLine("Acquiring second lock");
            lock (secondLock)
            {
                Console.WriteLine("Acquired second lock");

                #region Acquiring first lock
                Console.WriteLine("Acquiring first lock");
                lock (firstLock)
                {
                    Console.WriteLine("Acquired first lock");
                }
                Console.WriteLine("Released first lock");
                #endregion
            }
            Console.WriteLine("Released second lock");
        }

        private static void DoSomeWork()
        {
            Console.WriteLine("\t\t\t\t\tAcquiring first lock");
            lock (firstLock)
            {
                Console.WriteLine("\t\t\t\t\tAcquired first lock");
                
                // Do some hard work
                Thread.Sleep(1000);

                #region Acquiring second lock
                Console.WriteLine("\t\t\t\t\tAcquiring second lock");
                lock (secondLock)
                {
                    Console.WriteLine("\t\t\t\t\tAcquired second lock");
                }
                Console.WriteLine("\t\t\t\t\tReleased second lock");
                #endregion
            }
            Console.WriteLine("\t\t\t\t\tReleased first lock");
        }
    }
}
