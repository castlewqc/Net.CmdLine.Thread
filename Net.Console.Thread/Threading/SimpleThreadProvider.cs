using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.Threading
{
    public class SimpleThreadProvider : IThreadProvider, IThreadPoolInformationProvider
    {
        private static bool IsTraceBusinessObject = false;

        public void RunUIThread(Control control, MethodInvoker invoker)
        {
            try
            {
                MethodInvoker invokerWrapped = SafeMethodWapper.Wrapper(invoker);
                if (control.InvokeRequired)
                {
                    control.Invoke(invokerWrapped);
                    return;
                }
                invokerWrapped.Invoke();
            }
            catch (Exception ex)
            {
                IAFExceptionManagerFactory.CreateIAFExceptionManager()
                    .ManagerException(0, ExceptionPriority.Medium, ex);
            }
        }

        public IStopSignal RunWhileTrueNewThread(Action action, int intervalInMS)
        {
            StopSignal signal = new StopSignal();

            Thread thread = new Thread(() => WhileTureMethodWrapper.Wrapper(SafeMethodWapper.Wrapper(action), intervalInMS, signal)());
            thread.IsBackground = true;
            thread.Start();
            return signal;
        }

        public void RunNewThread(Action action)
        {
            Thread thread = new Thread(() => SafeMethodWapper.Wrapper(action)());
            thread.IsBackground = true;
            thread.Start();
        }

        public void RunNewThread<T>(Action<T> action, T arg)
        {
            Thread thread = new Thread(() => SafeMethodWapper.Wrapper<T>(action)(arg));
            thread.IsBackground = true;
            thread.Start();
        }

        public void RunNewThread<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            Thread thread = new Thread(() => SafeMethodWapper.Wrapper<T1, T2>(action)(arg1, arg2));
            thread.IsBackground = true;
            thread.Start();
        }

        public void RunNewThread<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            Thread thread = new Thread(() => SafeMethodWapper.Wrapper<T1, T2, T3>(action)(arg1, arg2, arg3));
            thread.IsBackground = true;
            thread.Start();
        }

        public void RunNewThread<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Thread thread = new Thread(() => SafeMethodWapper.Wrapper<T1, T2, T3, T4>(action)(arg1, arg2, arg3, arg4));
            thread.IsBackground = true;
            thread.Start();
        }

        public TResult RunNewThread<TResult>(Func<TResult> func)
        {
            Func<TResult> funcWrapper = null;

            if (IsTraceBusinessObject)
            {
                funcWrapper = BisunessObjectTraceWapper.Wrapper<TResult>(SafeMethodWapper.Wrapper<TResult>(func));
            }
            else
            {
                funcWrapper = SafeMethodWapper.Wrapper<TResult>(func);
            }

            TResult result = default(TResult);
            IAsyncResult ir = funcWrapper.BeginInvoke(null, null);
            ir.AsyncWaitHandle.WaitOne();
            result = funcWrapper.EndInvoke(ir);
            return result;
        }

        public TResult RunNewThread<T1, TResult>(Func<T1, TResult> func, T1 arg1)
        {
            Func<T1, TResult> funcWrapper = null;

            if (IsTraceBusinessObject)
            {
                funcWrapper = BisunessObjectTraceWapper.Wrapper<T1, TResult>(SafeMethodWapper.Wrapper<T1, TResult>(func));
            }
            else
            {
                funcWrapper = SafeMethodWapper.Wrapper<T1, TResult>(func);
            }

            TResult result = default(TResult);
            IAsyncResult ir = funcWrapper.BeginInvoke(arg1, null, null);
            ir.AsyncWaitHandle.WaitOne();
            result = funcWrapper.EndInvoke(ir);
            return result;
        }

        public TResult RunNewThread<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            Func<T1, T2, TResult> funcWrapper = null;

            if (IsTraceBusinessObject)
            {
                funcWrapper = BisunessObjectTraceWapper.Wrapper<T1, T2, TResult>(SafeMethodWapper.Wrapper<T1, T2, TResult>(func));
            }
            else
            {
                funcWrapper = SafeMethodWapper.Wrapper<T1, T2, TResult>(func);
            }

            TResult result = default(TResult);
            IAsyncResult ir = funcWrapper.BeginInvoke(arg1, arg2, null, null);
            ir.AsyncWaitHandle.WaitOne();
            result = funcWrapper.EndInvoke(ir);
            return result;
        }

        public TResult RunNewThread<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            Func<T1, T2, T3, TResult> funcWrapper = null;

            if (IsTraceBusinessObject)
            {
                funcWrapper = BisunessObjectTraceWapper.Wrapper<T1, T2, T3, TResult>(SafeMethodWapper.Wrapper<T1, T2, T3, TResult>(func));
            }
            else
            {
                funcWrapper = SafeMethodWapper.Wrapper<T1, T2, T3, TResult>(func);
            }

            TResult result = default(TResult);
            IAsyncResult ir = funcWrapper.BeginInvoke(arg1, arg2, arg3, null, null);
            ir.AsyncWaitHandle.WaitOne();
            result = funcWrapper.EndInvoke(ir);
            return result;
        }

        public TResult RunNewThread<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Func<T1, T2, T3, T4, TResult> funcWrapper = null;

            if (IsTraceBusinessObject)
            {
                funcWrapper = BisunessObjectTraceWapper.Wrapper<T1, T2, T3, T4, TResult>(SafeMethodWapper.Wrapper<T1, T2, T3, T4, TResult>(func));
            }
            else
            {
                funcWrapper = SafeMethodWapper.Wrapper<T1, T2, T3, T4, TResult>(func);
            }

            TResult result = default(TResult);
            IAsyncResult ir = funcWrapper.BeginInvoke(arg1, arg2, arg3, arg4, null, null);
            ir.AsyncWaitHandle.WaitOne();
            result = funcWrapper.EndInvoke(ir);
            return result;
        }

        public void Pipe<T>(T pipeState, IEnumerable<Action<T>> actions)
        {
            Thread thread = new Thread(() =>
            {
                SafeMethodWapper.Wrapper<T>((m) =>
                {
                    foreach (Action<T> action in actions)
                    {
                        action(pipeState);
                    }
                }
                   );
            }
            );

            thread.IsBackground = true;
            thread.Start();
        }

        public void Pipe<T>(T pipeState, params Action<T>[] actions)
        {
            Pipe<T>(pipeState, (IEnumerable<Action<T>>)actions);
        }

        public IProducerConsumerPattern ProducerConsumer<T>(Func<T> producerFunc, Action<T> consumerAction)
        {
            IConcurrentQueue<T> queue = new SynchronizedQueue<T>();
            IProducerConsumerPattern pattern = new ProducerConsumerPattern<T>(this, queue, producerFunc, consumerAction);
            return pattern;
        }

        public IProducerConsumerPattern ProducerConsumer<T>(Func<T> producerFunc, Action<T> consumerAction, byte consumerCount)
        {
            IConcurrentQueue<T> queue = new SynchronizedQueue<T>();
            IProducerConsumerPattern pattern = new ProducerConsumerPattern<T>(this, queue, producerFunc, consumerAction, consumerCount);
            return pattern;
        }

        public IProducerConsumerPattern ProducerConsumer<T>(Func<T> producerFunc, byte producerCount, Action<T> consumerAction, byte consumerCount)
        {
            IConcurrentQueue<T> queue = new SynchronizedQueue<T>();
            IProducerConsumerPattern pattern = new ProducerConsumerPattern<T>(this, queue, producerFunc, producerCount, consumerAction, consumerCount);
            return pattern;
        }

        public void Cancel()
        {
        }

        public void Shutdown()
        {
        }

        public void RefreshConfiguration()
        {
        }

        public IStartStopSignal ProviderTcpListener(int port, Action<ITcpConnectionInfo, byte[]> dataReceived, Action<ITcpConnectionInfo> clientConnected, Action<ITcpConnectionInfo> clientDisconnected)
        {
            TcpServer server = new TcpServer(port, dataReceived, clientConnected, clientDisconnected);

            IStartStopSignal signal = new TcpServerSignal(server);

            return signal;
        }

        public int GetMinThreadCount()
        {
            return 0;
        }

        public int GetMaxThreadCount()
        {
            return 0;
        }

        public int GetWorkingThreadCount()
        {
            return 0;
        }

        public int GetWorkItemCountInQueue()
        {
            return 0;
        }

        public List<ThreadInfo> GetAllThreadInformations()
        {
            return null;
        }
    }
}
