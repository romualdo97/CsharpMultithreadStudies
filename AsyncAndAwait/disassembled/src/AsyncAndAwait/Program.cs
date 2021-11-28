namespace AsyncAndAwait
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Program
    {
        private static Task DoSomethingAsync()
        {
            Action action1 = <>c.<>9__1_0;
            if (<>c.<>9__1_0 == null)
            {
                Action local1 = <>c.<>9__1_0;
                action1 = <>c.<>9__1_0 = delegate {
                    Thread.Sleep(0x5dc);
                    Console.WriteLine("Hello world from another thread!!!");
                };
            }
            return Task.get_Factory().StartNew(action1);
        }

        private static Task<string> DoSomethingElseAsync()
        {
            Func<string> func1 = <>c.<>9__2_0;
            if (<>c.<>9__2_0 == null)
            {
                Func<string> local1 = <>c.<>9__2_0;
                func1 = <>c.<>9__2_0 = delegate {
                    Thread.Sleep(0xbb8);
                    return "Result from async operation";
                };
            }
            return Task.get_Factory().StartNew<string>(func1);
        }

        [AsyncStateMachine((Type) typeof(<HelloWorldAsync>d__3)), DebuggerStepThrough]
        private static void HelloWorldAsync()
        {
            <HelloWorldAsync>d__3 stateMachine = new <HelloWorldAsync>d__3 {
                <>t__builder = AsyncVoidMethodBuilder.Create(),
                <>1__state = -1
            };
            stateMachine.<>t__builder.Start<<HelloWorldAsync>d__3>(ref stateMachine);
        }

        private static void Main(string[] args)
        {
            HelloWorldAsync();
            Console.WriteLine("Reached end of Main method.");
            Console.ReadLine();
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly Program.<>c <>9 = new Program.<>c();
            public static Action <>9__1_0;
            public static Func<string> <>9__2_0;

            internal void <DoSomethingAsync>b__1_0()
            {
                Thread.Sleep(0x5dc);
                Console.WriteLine("Hello world from another thread!!!");
            }

            internal string <DoSomethingElseAsync>b__2_0()
            {
                Thread.Sleep(0xbb8);
                return "Result from async operation";
            }
        }

        [CompilerGenerated]
        private sealed class <HelloWorldAsync>d__3 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncVoidMethodBuilder <>t__builder;
            private string <result>5__1;
            private string <>s__2;
            private TaskAwaiter <>u__1;
            private TaskAwaiter<string> <>u__2;

            private void MoveNext()
            {
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter awaiter;
                    Program.<HelloWorldAsync>d__3 d__;
                    TaskAwaiter<string> awaiter2;
                    if (num == 0)
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter();
                        this.<>1__state = num = -1;
                    }
                    else if (num == 1)
                    {
                        awaiter2 = this.<>u__2;
                        this.<>u__2 = new TaskAwaiter<string>();
                        this.<>1__state = num = -1;
                        goto TR_0005;
                    }
                    else
                    {
                        Console.WriteLine("Started something");
                        awaiter = Program.DoSomethingAsync().GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            d__ = this;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, Program.<HelloWorldAsync>d__3>(ref awaiter, ref d__);
                            return;
                        }
                    }
                    awaiter.GetResult();
                    Console.WriteLine("Continuation after prev async workload is completed");
                    awaiter2 = Program.DoSomethingElseAsync().GetAwaiter();
                    if (awaiter2.IsCompleted)
                    {
                        goto TR_0005;
                    }
                    else
                    {
                        this.<>1__state = num = 1;
                        this.<>u__2 = awaiter2;
                        d__ = this;
                        this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, Program.<HelloWorldAsync>d__3>(ref awaiter2, ref d__);
                    }
                    return;
                TR_0005:
                    this.<>s__2 = awaiter2.GetResult();
                    this.<result>5__1 = this.<>s__2;
                    this.<>s__2 = null;
                    Console.WriteLine(this.<result>5__1);
                    this.<>1__state = -2;
                    this.<result>5__1 = null;
                    this.<>t__builder.SetResult();
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<result>5__1 = null;
                    this.<>t__builder.SetException(exception);
                }
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
            }
        }
    }
}

