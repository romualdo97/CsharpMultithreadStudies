// #define EX01 // Monitor enter and monitor exit without taking care of exceptions
// #define EX02 // Monitor enter and monitor exit taking care of exceptions
// #define EX03 // Implementing something similar to lock keyword but with timeout capabilities
#define EX04 // Using ReaderWriterLockSlim so we allow parallel reads and synchronized writes

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Synchronization
{
    public class BankCard
    {
        private decimal m_moneyAmount;
        private decimal m_credit;
        private readonly object m_sync = new object();
        private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();

        public decimal TotalMoneyAmount
        {
            get
            {
                m_rwLock.EnterReadLock();
                var result = m_moneyAmount + m_credit;
                m_rwLock.ExitReadLock();

                return result;                
            }
        }

        public BankCard(decimal moneyAmount)
        {
            m_moneyAmount = moneyAmount;
        }

        public void ReceivePayment(decimal amount)
        {
#if EX01
            Monitor.Enter(m_sync);
            m_moneyAmount += amount;
            Monitor.Exit(m_sync);
#endif

#if EX02
            bool isLockTaken = false;
            try
            {
                Monitor.Enter(m_sync, ref isLockTaken);
                m_moneyAmount += amount;
            }
            finally
            {
                if (isLockTaken)
                {
                    Monitor.Exit(m_sync);
                }
            }
#endif

#if EX03
            // If your aree 100% that you dont need the timeout... then use the lock keyword
            lock (m_sync)
            {
                m_moneyAmount += amount;
            }
#endif

#if EX04
            // Note you should never use this without try finally blocks... leaving this way for simplicity sake
            m_rwLock.EnterWriteLock();
            m_moneyAmount += amount;
            m_rwLock.ExitWriteLock();
#endif
        }

        public void TransferToCard(decimal amount, BankCard recipient)
        {
#if EX01
            Monitor.Enter(m_sync);
            m_moneyAmount -= amount;
            recipient.m_moneyAmount += amount;
            Monitor.Exit(m_sync);
#endif

#if EX02
            bool isLockTaken = false;
            try
            {
                // Monitor.Enter(m_sync, ref isLockTaken);
                Monitor.TryEnter(m_sync, TimeSpan.FromSeconds(10), ref isLockTaken);
                m_moneyAmount -= amount;
                recipient.m_moneyAmount += amount;
            }
            finally
            {
                if (isLockTaken)
                {
                    Monitor.Exit(m_sync);
                }
            }
#endif

#if EX03
            using (m_sync.LockTimeout(TimeSpan.FromSeconds(3)))
            {
                // Thread.Sleep(5000); // Generate a timeout exception on other threads waitting to acquire this lock
                m_moneyAmount -= amount;
                recipient.m_moneyAmount += amount;
            }
#endif

#if EX04
            // Note you should never use this without try finally blocks... leaving this way for simplicity sake
            m_rwLock.EnterWriteLock();
            m_moneyAmount -= amount;
            recipient.m_moneyAmount += amount;
            m_rwLock.ExitWriteLock();
#endif
        }

        public override string ToString()
        {
            return m_moneyAmount + "";
        }
    }

    class MonitorExamples
    {
        public static void Start ()
        {
            var a = new BankCard(10);
            var b = new BankCard(20);

            List<Task> allTasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var t = Task.Factory.StartNew(() =>
                {
                    a.ReceivePayment(1);
                });
                allTasks.Add(t);
            }

            for (int i = 0; i < 10; i++)
            {
                var t = Task.Factory.StartNew(() =>
                {
                    a.TransferToCard(1, b);
                });
                allTasks.Add(t);
            }

            Task.WaitAll(allTasks.ToArray());
            Console.WriteLine(a);
        }

    }
}
