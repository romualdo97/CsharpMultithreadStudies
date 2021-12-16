using System;

namespace ConcurrentCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            IStack<int> myStack = Stack<int>.Empty;
            var first = myStack.Push(1);
            var second = first.Push(2);
            var third = second.Push(3);

            foreach (var num in third)
            {
                Console.WriteLine(num);
            }

            Console.ReadLine();
        }
    }
}
