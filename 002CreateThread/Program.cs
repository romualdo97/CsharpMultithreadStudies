#region Exercise indices
// Please define only one of the symbols below at a time, EX means Exarcise
//#define EX001
//#define EX002
//#define EX003
#define EX004
//#define EX005
#endregion

using System;
using System.Threading;
using System.Collections.Generic;

namespace CreateThread
{
#if EX004
    class PrintingInfo
    {
        public int ProcessedNumbers { get; set; }
    }
#endif

    class Program
    {
        static void Main(string[] args)
        {
#if EX001
            Thread t1 = new Thread(PrintEven);
            t1.Start();

            PrintOdd();
#endif

#if EX002
            Thread t1 = new Thread(PrintWithParameter);
            t1.Start(false);

            PrintWithParameter(true);
#endif


#if EX003
            Thread t1 = new Thread(() => PrintWithParameter(false));
            t1.Start();

            Thread.Sleep(10);
            t1.Abort();
            Console.WriteLine("After abort");

            PrintWithParameter(true);
#endif

#if EX004
            var printInfo = new PrintingInfo();
            var t1 = new Thread(() => Print(false, printInfo));
            t1.Start();
            
            // Blocks the calling thread until the thread represented by this instance terminates,
            // while continuing to perform standard COM and SendMessage pumping.
            if (t1.Join(TimeSpan.FromMilliseconds(5000)))
            {
                Console.WriteLine($"I'm sure that spawned thread processed that many: {printInfo.ProcessedNumbers}");
            }
            else
            {
                Console.WriteLine("Timed out. can't process results");
            }
#endif

#if EX005
            // Demonstration that Thread class creates a foreground thread not a background thread
            // foreground threads need to be completed before allowing the process to finish.
            var t1 = new Thread(Print);
            t1.IsBackground = true; // Make the thread a background thread... so the application finishes when the main thread gets finished (usually we work with background threads)
            t1.Priority = ThreadPriority.Highest; // You can assing a thread priority (99%% tyous hsould not set priority of thread... since you work with threads pools and thread pool is more intelligent calculating priority of threads)
            t1.Start(); // Starts a thread with an infinite loop
            Console.WriteLine("Press enter...");
#endif

            Console.ReadLine();
        }

#if EX001
        private static void PrintEven()
        {
            for (int i = 0; i < 100; i++)
            {
                if (i % 2 == 0)
                {
                    Console.WriteLine(i);
                }
            }
        }

        private static void PrintOdd()
        {
            for (int i = 0; i < 100; i++)
            {
                if (i % 2 != 0)
                {
                    Console.WriteLine(i);
                }
            }
        }
#endif

#if EX002
        private static void PrintWithParameter(object showEven)
        {
            bool showEvenArg = (bool)showEven;
            for (int i = 0; i < 100; i++)
            {
                bool shouldShow = showEvenArg ? i % 2 == 0 : i % 2 != 0;
                if (shouldShow)
                {
                    Console.WriteLine(i);
                }
            }
        }
#endif

#if EX003
        private static void PrintWithParameter(bool showEven)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    bool shouldShow = showEven ? i % 2 == 0 : i % 2 != 0;
                    if (shouldShow)
                    {
                        string type = showEven ? "even" : "odd";
                        Console.WriteLine($"Showing {type} number; Current thread ID: {Thread.CurrentThread.ManagedThreadId}");
                        Console.WriteLine(i + "\n");
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine(e);
            }
        }
#endif

#if EX004
        private static void Print(bool showEven, PrintingInfo printInfo)
        {
            if (showEven)
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (i % 2 == 0)
                    {
                        printInfo.ProcessedNumbers++;
                        Console.WriteLine($"Showing even number; Current thread ID: {Thread.CurrentThread.ManagedThreadId}");
                        Console.WriteLine(i + "\n");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10000; i++)
                {
                    if (i % 2 != 0)
                    {
                        printInfo.ProcessedNumbers++;
                        Console.WriteLine($"Showing odd number; Current thread ID: {Thread.CurrentThread.ManagedThreadId}");
                        Console.WriteLine(i + "\n");
                    }
                }
            }
        }
#endif

#if EX005
        private static void Print()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
#endif
    }
}
