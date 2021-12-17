using System;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace ConcurrentCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            // DoMyStackDemo();
            // StackDemo();
            // QueueDemo();
            // ListDemo();
            // SetDemo();
            // DictionaryDemo();
            ImmutableBuilderDemo();

            Console.ReadLine();
        }

        static void DoMyStackDemo()
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

        static void StackDemo()
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

        static void QueueDemo()
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

        static void ListDemo()
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

        static void SetDemo()
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

        static void DictionaryDemo()
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
    }
}
