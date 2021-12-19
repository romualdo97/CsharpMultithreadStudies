using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    class ProducerConsumerDemo
    {
        private readonly BlockingCollection<string> m_cutleriesToWash = new BlockingCollection<string>(new ConcurrentStack<string>(), 10);

        private readonly List<string> m_cutleries = new List<string>()
        {
            "Fork",
            "Spoon",
            "Plate",
            "Knife"
        };

        private readonly Random m_rng = new Random();

        // Producer
        public void Eat(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string nextCutlery = m_cutleries[m_rng.Next(4)];
                m_cutleriesToWash.Add(nextCutlery);
                Console.WriteLine($"+ {nextCutlery}");
                Thread.Sleep(500);
            }
        }

        // Consumer
        public void Wash(CancellationToken cancellationToken)
        {
            foreach (var item in m_cutleriesToWash.GetConsumingEnumerable())
            {
                cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine($"- {item}");
                Thread.Sleep(3000);
            }
        }

        public void Run(CancellationToken ct)
        {
            Task t1 = Task.Run(() => Eat(ct), ct);
            Task t2 = Task.Run(() => Wash(ct), ct);

            try
            {
                Task.WaitAll(t1, t2);
            }
            catch (AggregateException ex)
            {
            }
        }

        public static void Start()
        {
            var cts = new CancellationTokenSource();
            var producerConsumerDemo = new ProducerConsumerDemo();
            Task.Run(() => producerConsumerDemo.Run(cts.Token), cts.Token);
            Console.ReadLine();
            cts.Cancel();
            Console.WriteLine("End of processing");
        }
    }
}
