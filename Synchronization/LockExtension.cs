using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Synchronization
{
    public static class LockExtension
    {
        public static Lock LockTimeout(this object obj, TimeSpan timeout)
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(obj, timeout, ref lockTaken);
                if (lockTaken)
                {
                    return new Lock(obj);
                }

                Console.WriteLine("Timeout error");
                throw new TimeoutException("Failed to acquire sync object.");
            }
            catch
            {
                if (lockTaken)
                {
                    Monitor.Exit(obj);
                }

                throw; // Re-thorw exceptions
            }
        }

        public struct Lock : IDisposable
        {
            private readonly object m_sync;

            public Lock(object obj)
            {
                m_sync = obj;
            }

            public void Dispose()
            {
                Monitor.Exit(m_sync);
            }
        }
    }
}
