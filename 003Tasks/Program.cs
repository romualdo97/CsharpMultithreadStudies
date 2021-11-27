// Please define only one of the symbols below at a time, EX means Exarcise

// Creating a task and task states
//#define EX001
//#define EX002
//#define EX003
//#define EX004

// Cancelling a task
//#define EX005
//#define EX006
//#define EX007

// Continuations
//#define EX008 // continue after a prev task... chaining tasks
//#define EX009 // continue after a prev task... chaining tasks

// Wait
//#define EX010

// I/O Tasks
//#define EX011

// Nested vs Child tasks
//#define EX012

// Exceptions handling
#define EX013

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _003Tasks
{
#if EX007
    // Simple demostration of the register to cancellation.... this not works just is a demostration
    class WebClientWrapper
    {
        private WebClient wc = new WebClient();

        private async Task LongRunningOperation(CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                // Registering for a cancellation event is way better than polling the token by calling its
                // cancellation requested flag.
                // So this is a very neat way to call some arbitrary code when cancell is requested
                // and remember that not all long running operation have a loop inside.
                // in those cases use the power of the register method.  
                using (CancellationTokenRegistration ctr = token.Register(() => { wc.CancelAsync() }))
                {
                    wc.DownloadStringAsync(new Uri("https://romualdo97.github.io/"));
                }
            }
        }
    }
#endif

    class Program
    {
        static void Main(string[] args)
        {
#if EX001
            Task.Run(Print);
            Task.Run(Print);
#endif

#if EX002
            // This is equivalent to code in EX001, note this gives you more control on
            // the creation of the task, also the Run method creates a task that can't be cancelled
            // so there'S an overload of the Run method that receives a cancellationToken
            Task.Factory.StartNew(Print, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            Task.Factory.StartNew(Print);
#endif

#if EX003
            Task.Factory.StartNew(() => PrintWithArguments("Romualdo"));
            Task.Factory.StartNew(() => PrintWithArguments("José"));
#endif

#if EX004
            Task<int> t1 = Task.Factory.StartNew(() => Print(true), CancellationToken.None, TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task<int> t2 = Task.Factory.StartNew(() => Print(false));

            // Accessing the Result property blocks the calling thread until the asynchronous operation is complete; it is equivalent to calling the Wait method.
            // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1.result?redirectedfrom=MSDN&view=net-6.0#System_Threading_Tasks_Task_1_Result
            Console.WriteLine($"t1.Result: {t1.Result}");
            Console.WriteLine($"t2.Result: {t2.Result}");
#endif

#if EX005
            var cts = new CancellationTokenSource();
            Task<int> t1 = Task.Run(() => Print(true, cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(false, cts.Token), cts.Token);
            
            Thread.Sleep(10);

            cts.Cancel();

            // Some hack to see the status after the exception is thrown
            try
            {
                Console.WriteLine($"t1.Status: {t1.Result}");
                Console.WriteLine($"t2.Status: {t2.Result}");
            }
            catch {}

            Console.WriteLine($"t1.Status: {t1.Status}");
            Console.WriteLine($"t2.Status: {t2.Status}");
#endif

#if EX006
            var parentCts = new CancellationTokenSource();
            var childCts = CancellationTokenSource.CreateLinkedTokenSource(parentCts.Token);

            Task<int> t1 = Task.Run(() => Print(true, parentCts.Token), parentCts.Token);
            Task<int> t2 = Task.Run(() => Print(false, childCts.Token), childCts.Token);

            // Select one of these options
            // parentCts.CancelAfter(10); // equivalent to Thread.Sleep(10); parentCts.Cancel();

            // Select one of these options
            Thread.Sleep(10);
            //parentCts.Cancel(); // Select this
            //childCts.Cancel(); // or this

            // Some hack to see the status after the exception is thrown
            try
            {
                Console.WriteLine($"t1.Status: {t1.Result}");
                Console.WriteLine($"t2.Status: {t2.Result}");
            }
            catch { }

            Console.WriteLine($"t1.Status: {t1.Status}");
            Console.WriteLine($"t2.Status: {t2.Status}");
#endif

#if EX008
            var cts = new CancellationTokenSource();
            Task<int> t1 = Task.Run(() => Print(true, cts.Token), cts.Token);
            
            var task = t1.ContinueWith(prevTask =>
            {
                Console.WriteLine($"How many numbers where processed by previous task: {prevTask.Result}");
                var t = Task.Run(() => Print(false, cts.Token), cts.Token);
                t.Wait();
                //throw new InvalidOperationException();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            
            task.ContinueWith(prevTask =>
            {
                Console.WriteLine($"Finished...");
            }, TaskContinuationOptions.OnlyOnFaulted);

            Console.WriteLine($"Main thread is not blocked");
#endif

#if EX009
            var cts = new CancellationTokenSource();
            Task<int> t1 = Task.Run(() => Print(true, cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(true, cts.Token), cts.Token);

            Task.Factory.ContinueWhenAll(new[] { t1, t2 }, tasks =>
            {
                Console.WriteLine($"Finished...");
            });

            Console.WriteLine($"Main thread is not blocked");
#endif

#if EX010
            var cts = new CancellationTokenSource();
            Task<int> t1 = Task.Run(() => Print(true, cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(true, cts.Token), cts.Token);

            Console.WriteLine("Started t1");
            Console.WriteLine("Started t2");
            Task.WaitAll(t1, t2);
            //t1.Wait();
            Console.WriteLine("Finished t1");
            Console.WriteLine("Finished t2");
#endif

#if EX011
            TestTaskWrite();
#endif

#if EX012
            Task t1 = Task.Run(() => { throw new InvalidOperationException(); });

            try
            {
                t1.Wait();
            } 
            catch (AggregateException ex)
            {
                var flattenList = ex.Flatten().InnerExceptions;
                foreach (var exception in flattenList)
                {
                    Console.WriteLine(exception);
                }
            }
#endif

#if EX013
            Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() => { Console.WriteLine("Something"); }, TaskCreationOptions.AttachedToParent);
                
            }).Wait();
#endif
            Thread.Sleep(10);
            // <<<<<<<<<<<< END OF MAIN
            //Console.ReadLine();
        }

#if EX001
        private static void Print()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
        }
#endif

#if EX002
        private static void Print()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
            }
        }
#endif

#if EX003
        private static void PrintWithArguments(string name)
        {
            Console.WriteLine($"Hello {name}!!!");
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
            }
        }
#endif

#if EX004
        private static int Print(bool showEven)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif

#if EX005
        private static int Print(bool showEven, CancellationToken cancellationToken)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif

#if EX006
        private static int Print(bool showEven, CancellationToken cancellationToken)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif

#if EX008
        private static int Print(bool showEven, CancellationToken cancellationToken)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif

#if EX009
        private static int Print(bool showEven, CancellationToken cancellationToken)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif


#if EX010
        private static int Print(bool showEven, CancellationToken cancellationToken)
        {
            Console.WriteLine($"isThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            if (showEven)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 == 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Requested cancellation");
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    if (i % 2 != 0)
                    {
                        total++;
                        Console.WriteLine($"currentTaskId: {Task.CurrentId}, value: {i}");
                    }
                }
            }

            return total;
        }
#endif

#if EX011
        private static void TestTaskWrite()
        {
            string filePath = "helloWorld.txt";
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 8, true);
            
            byte[] data = Encoding.Unicode.GetBytes("A quick brown fox jumps over the lazy dog.");

            Task task = fs.WriteAsync(data, 0, data.Length);

            // Register a continuation action which will called when the write is completed
            task.ContinueWith(writeTask =>
            {
                fs.Close();
                TestTaskRead();
                Console.WriteLine("Write completed"); // Note that read task is not blocking this to occur
            });

            // Not blocked the main thread
            Console.WriteLine("Not blocked the main thread");
        }

        private static void TestTaskRead()
        {
            string filePath = "helloWorld.txt";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 8, true);

            byte[] data = new byte[1024];

            Task<int> readTask = fs.ReadAsync(data, 0, data.Length);
            readTask.ContinueWith(prevTask =>
            {
                fs.Close();
                Console.WriteLine("Read completed \"" + Encoding.Unicode.GetString(data, 0, prevTask.Result) + "\"");
            });
        }
#endif
    }
}
 