using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAndAwait
{
    class Program
    {
        static void Main(string[] args)
        {
            // DoSomethingAsync();

            HelloWorldAsync();

            Console.WriteLine("Reached end of Main method.");
            Console.ReadLine();
        }

        static Task DoSomethingAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1500); // Simulate lot of work
                Console.WriteLine("Hello world from another thread!!!");
            });
        }

        static Task<string> DoSomethingElseAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                return "Result from async operation";
            });
        }

        static async void HelloWorldAsync()
        {
            Console.WriteLine("Started something");
            await DoSomethingAsync();

            Console.WriteLine("Continuation after prev async workload is completed");
            string result = await DoSomethingElseAsync();
            Console.WriteLine(result);
        }
    }
}
