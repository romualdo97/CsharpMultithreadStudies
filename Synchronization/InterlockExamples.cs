using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Synchronization
{
    class Character
    {
        private int m_armor = 0;
        private int m_health = 100;

        public int Health { get => m_health; private set => m_health = value; }
        public int Armor { get => m_armor; private set => m_armor = value; }

        public void Hit(int damage)
        {
            // Health -= damage - Armor;
            int actualDamage = Interlocked.Add(ref damage, -Armor);
            Interlocked.Add(ref m_health, -actualDamage);
        }

        public void Heal(int heal)
        {
            // Health += heal;
            Interlocked.Add(ref m_health, heal);
        }

        public void CastArmorSpell(bool isPositive)
        {
            if (isPositive)
            {
                Interlocked.Increment(ref m_armor);
                // Armor++;
            }
            else
            {
                Interlocked.Decrement(ref m_armor);
                // Armor--;
            }
        }
    }

    class InterlockExamples
    {
        static void ExchangeSample()
        {
            // Remember the simple example of turning a flag on and trying to determine which code actually turned it on ?
            // bool y = x;
            // x = true;
            // return y;

            int x = 0;
            int y = Interlocked.Exchange(ref x, 1);
        }

        static void CompareExchangeExample()
        {
            // Remember the simple example of trying to set the value to 200 if-and-only-if the current value is 100?
            // if (x == 100)
            // {
            //     x = 200;
            // }

            double x = 100;
            Interlocked.CompareExchange(ref x, 200, 100);
        }

        static void ThreadSafeSwap()
        {
            double a = 100;
            double b = 200;

            // Swap
            //double tmp = a;
            //a = b;
            //b = tmp;

            double tmp = Interlocked.Exchange(ref a, b);
            Interlocked.Exchange(ref b, tmp);
        }

        public static void Start()
        {
            // The heap memory is a resource shared between all threads (every class is allocated in heap in C#)
            Character c = new Character();

            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var t1 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        c.Hit(10);
                        //c.CastArmorSpell(true);
                    }
                });
                tasks.Add(t1);

                var t2 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        c.Heal(10);
                        //c.CastArmorSpell(false);
                    }
                });
                tasks.Add(t2);
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Resulting health {c.Health}");
            Console.WriteLine($"Resulting armor {c.Armor}");
            Console.ReadLine();
        }
    }
}
