using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ConcurrentCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoMyImmutableStackDemo();
            //ImmutableStackDemo();
            //ImmutableQueueDemo();
            //ImmutableListDemo();
            //ImmutableSetDemo();
            //ImmutableDictionaryDemo();
            //ImmutableBuilderDemo();
            //ConcurrentQueueDemo();
            //ConcurrentStackDemo();
            //ConcurrentBagDemo();
            //ConcurrentDictionaryDemo();
            //ProducerConsumerDemo.Start();
            ProducerConsumerDemo();

            Console.ReadLine();
        }

        static void DoMyImmutableStackDemo()
        {
            IStack<int> myStack = Stack<int>.Empty;
            var first = myStack.Push(1);
            var second = first.Push(2);
            var third = second.Push(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        static void ImmutableStackDemo()
        {
            var myStack = ImmutableStack.Create<int>();
            var first = myStack.Push(1);
            var second = first.Push(2);
            var third = second.Push(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        static void ImmutableQueueDemo()
        {
            var myQueue = ImmutableQueue<int>.Empty;
            var first = myQueue.Enqueue(1);
            var second = first.Enqueue(2);
            var third = second.Enqueue(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        static void ImmutableListDemo()
        {
            var myList = ImmutableList<int>.Empty;
            var first = myList.Add(1);
            var second = first.Add(2);
            var third = second.Add(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        static void ImmutableSetDemo()
        {
            var mySet = ImmutableHashSet<int>.Empty;
            var first = mySet.Add(1);
            var second = first.Add(2);
            var third = second.Add(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        static void ImmutableDictionaryDemo()
        {
            var mySet = ImmutableDictionary<int, float>.Empty;
            var first = mySet.Add(1, 1.5f);
            var second = first.Add(2, 2.5f);
            var third = second.Add(3, 3.5f);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }
        }

        private static void ImmutableBuilderDemo()
        {
            var list = new List<int>();
            for (int i = 0; i < 120000; i++)
            {
                list.Add(i);
            }

            // Option A: Converting mutable list into an immutable one
            //var builder = ImmutableList.CreateBuilder<int>();
            //builder.AddRange(list);
            //var immutableList = builder.ToImmutable();

            // Option B: Converting mutable list into an immutable one (using extension methods defined in the immutable namespace)
            var immutableList = list.ToImmutableList();
        }

        private static void ConcurrentQueueDemo()
        {
            var names = new ConcurrentQueue<string>();
            names.Enqueue("romu");
            names.Enqueue("jose");

            Task.Run(() =>
            { 
                while (!names.IsEmpty)
                {
                    string name;
                    if (names.TryDequeue(out name))
                    {
                        Console.WriteLine($"Dequeued {name} on other thread");
                    }
                }
            });

            // Play changing this value
            Thread.Sleep(100);

            string name;
            bool success = names.TryDequeue(out name);

            if (success)
            {
                Console.WriteLine($"Dequeued {name} on main thread");
            }
            else
            {
                Console.WriteLine("Nothing to dequeue on main thread");
            }
        }

        private static void ConcurrentStackDemo()
        {
            var names = new ConcurrentStack<string>();
            names.Push("romu");
            names.Push("jose");

            Task.Run(() =>
            {
                while (!names.IsEmpty)
                {
                    string name;
                    if (names.TryPop(out name))
                    {
                        Console.WriteLine($"Popped {name} on other thread");
                    }
                }
            });

            // Play changing this value
            Thread.Sleep(10);

            string name;
            bool success = names.TryPop(out name);

            if (success)
            {
                Console.WriteLine($"Popped {name} on main thread");
            }
            else
            {
                Console.WriteLine("Nothing to pop on main thread");
            }
        }


        private static void ConcurrentBagDemo()
        {
            // http://dotnetpattern.com/csharp-concurrentbag
            // As I have explained earlier, ConcurrentBag is preferable in scenarios where same
            // thread is both producer and the consumer. ConcurrentBag maintains a local queue
            // for each thread that access it, and when the same thread is retrieving items,
            // it gives priority to those items that are in same thread queue.
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            AutoResetEvent autoEvent1 = new AutoResetEvent(false);

            Task t1 = Task.Factory.StartNew(() =>
            {
                for (int i = 1; i <= 4; ++i)
                {
                    bag.Add(i);
                }

                // Wait for second thread to add its items
                autoEvent1.WaitOne();

                while (bag.IsEmpty == false)
                {
                    int item;
                    if (bag.TryTake(out item))
                    {
                        Console.WriteLine(item);
                    }
                }
            });


            Task t2 = Task.Factory.StartNew(() =>
            {
                for (int i = 5; i <= 7; ++i)
                {
                    bag.Add(i);
                }
                autoEvent1.Set();
            });

            t1.Wait();
        }

        private static void ConcurrentDictionaryDemo()
        {
            // Construct a ConcurrentDictionary
            ConcurrentDictionary<int, int> cd = new ConcurrentDictionary<int, int>();

            // Bombard the ConcurrentDictionary with 10000 competing AddOrUpdates
            Parallel.For(0, 10000, i =>
            {
                // Initial call will set cd[1] = 1.
                // Ensuing calls will set cd[1] = cd[1] + 1
                cd.AddOrUpdate(1, 1, (key, oldValue) => oldValue + 1);
            });

            Console.WriteLine("After 10000 AddOrUpdates, cd[1] = {0}, should be 10000", cd[1]);

            // Should return 100, as key 2 is not yet in the dictionary
            int value = cd.GetOrAdd(2, (key) => 100);
            Console.WriteLine("After initial GetOrAdd, cd[2] = {0} (should be 100)", value);

            // Should return 100, as key 2 is already set to that value
            value = cd.GetOrAdd(2, 10000);
            Console.WriteLine("After second GetOrAdd, cd[2] = {0} (should be 100)", value)

            //cd.AddOrUpdate
        }


        private static void ProducerConsumerDemo()
        {
            // https://www.infoworld.com/article/3090215/how-to-work-with-blockingcollection-in-c.html
            BlockingCollection<int> data = new BlockingCollection<int>(boundedCapacity: 3);

            Task.Run(() =>
            {
                Thread.Sleep(2000);

                // Take using the enumerator
                //var consumingEnumerator = data.GetConsumingEnumerable().GetEnumerator();
                //consumingEnumerator.MoveNext();
                //Console.WriteLine($"Consumed {consumingEnumerator.Current}");

                // Take using the Take method
                var value = data.Take();
                Console.WriteLine($"Consumed {value}");

                Console.WriteLine("Sleeping");
                Thread.Sleep(2000);

                value = data.Take();
                Console.WriteLine($"Consumed {value}");
                value = data.Take();
                Console.WriteLine($"Consumed {value}");
                value = data.Take();
                Console.WriteLine($"Consumed {value}");
                value = data.Take();
                Console.WriteLine($"Consumed {value}");

                Console.WriteLine("Finished");

            });

            data.Add(1);
            Console.WriteLine("Added 1");

            data.Add(2);
            Console.WriteLine("Added 2");

            data.Add(3);
            Console.WriteLine("Added 3");

            data.Add(4); // This would block until an item is removed from the collection.
            Console.WriteLine("Added 4");

            Thread.Sleep(3000);
            data.Add(5);
            Console.WriteLine("Added 5");
        }

    }
}
