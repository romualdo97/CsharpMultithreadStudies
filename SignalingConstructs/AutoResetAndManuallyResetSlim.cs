// #define USE_MANUALRESET
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace SignalingConstructs
{
    // Imagine this code is provided by vendos and you can't change the source code
    public class Protocol
    {
        public enum OperationStatus
        {
            Finished,
            Faulted
        }

        public class ProtocolMessage
        {
            public OperationStatus Status { get; }

            public ProtocolMessage(OperationStatus status)
            {
                Status = status;
            }
        }

        IPEndPoint m_enpoint;
        public event EventHandler<ProtocolMessage> OnMessageReceived;

        public Protocol(IPEndPoint endpoint)
        {
            m_enpoint = endpoint;
        }

        public void Send(int opCode, object parameters)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"\t\t\t\t\tEmulating operation on thread {Thread.CurrentThread.ManagedThreadId}");
                // Emulating interoperation with a bank terminal device
                Thread.Sleep(3000);

                Console.WriteLine($"\t\t\t\t\tInvoking delegate {Thread.CurrentThread.ManagedThreadId}");
                OnMessageReceived?.Invoke(this, new ProtocolMessage(OperationStatus.Finished));
            });
        }
    }

    // Client API
    public class BankTerminal
    {
        private readonly Protocol m_protocol;
#if USE_MANUALRESET
        private readonly ManualResetEventSlim m_operationSignal = new ManualResetEventSlim(false);
#else
        private readonly AutoResetEvent m_operationSignal = new AutoResetEvent(false);
#endif

        public BankTerminal(IPEndPoint endPoint)
        {
            m_protocol = new Protocol(endPoint);
            m_protocol.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, Protocol.ProtocolMessage e)
        {
            Console.WriteLine($"Message received {Thread.CurrentThread.ManagedThreadId}");
            if (e.Status == Protocol.OperationStatus.Finished)
#if USE_MANUALRESET
                m_operationSignal.Set();
#else
                m_operationSignal.Set();
#endif
        }

        public Task Purchase(decimal amount)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Purshasing on {Thread.CurrentThread.ManagedThreadId}");
                const int purchaseOpCode = 1;
                m_protocol.Send(purchaseOpCode, amount);

                Console.WriteLine($"Waiting on {Thread.CurrentThread.ManagedThreadId}");
#if USE_MANUALRESET
                m_operationSignal.Reset(); // Manual reset the trigger
                m_operationSignal.Wait();
#else
                m_operationSignal.WaitOne();
#endif
                Console.WriteLine($"After waiting {Thread.CurrentThread.ManagedThreadId}");
            });
        }
    }

    class AutoResetAndManuallyResetSlim
    {
        public static void Start()
        {
            var bankTerminal = new BankTerminal(new IPEndPoint(new IPAddress(0x0), 8080));

            // Perform a first purchase
            Task firstTask = bankTerminal.Purchase(1500);
            firstTask.ContinueWith(task =>
            {
                Console.WriteLine("Operation done!!!");
            });

            // Wait until pre work is finished
            firstTask.Wait();
            Console.WriteLine("===================================");

            // Perform a second purchase
            Task secondTask = bankTerminal.Purchase(5000);
            secondTask.ContinueWith(task =>
            {
                Console.WriteLine("Second purchase done!!!");
            });


            Console.ReadLine();
        }
    }
}
