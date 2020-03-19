using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class TaskException
    {
     

        //通过执行Task.WaitAll(task),Task.WaitAny(task),task.Result,task.Wait()出现了异常抛出的是一个System.AggregateException;

        //通过执行task.Wait(CancellationToken)出现了异常抛出的是一个OperationCanceledException;
        public void WaitCatchException()
        {
           
                Task t =new Task( () =>
                {
                    try
                    {
                        Thread.SpinWait(1000);
                        var ex1 = new System.Exception("ex1");
                        var ex2 = new System.Exception("ex2");
                        throw new AggregateException("WaitCathException",ex1,ex2);
                    }
                    catch (AggregateException  ex)
                    {
                        ex.Handle((handle) => 
                        {
                             Console.WriteLine(handle.Message);
                             return true;
                        });
                        Console.WriteLine(ex.Message);
                       
                    }
                });

                t.Start();
          
                t.Wait();
           
        }

        //主线程委托（比较好）
        public event EventHandler<AggregateExceptionEventArgs> AggregateExceptionCatched=
            new EventHandler<AggregateExceptionEventArgs>(TaskException_AggregateExceptionCatched);
        public void EventHandlerCatchException()
        {
            Task t = new Task(() =>
            {
                try
                {
                    throw new InvalidOperationException("任务并行编码 中产生未知错误");
                }
                catch (System.Exception ex)
                {
                    AggregateExceptionEventArgs args = new AggregateExceptionEventArgs()
                    {
                        AggregateException = new AggregateException(ex)
                    };
                    //使用主线程委托代理，处理子线程 异常
                    //这种方式没有阻塞 主线程或其他线程
                    AggregateExceptionCatched?.Invoke(null, args);
                }
            });
            t.Start();
        }

        private static void TaskException_AggregateExceptionCatched(object sender, AggregateExceptionEventArgs e)
        {
           
            foreach (var item in e.AggregateException.InnerExceptions)
            {
                Console.WriteLine("异常类型：{0}{1}来自：{2}{3}异常内容：{4}",
                    item.GetType(), Environment.NewLine, item.Source,
                    Environment.NewLine, item.Message);
            }
        }

        //TaskScheduler.UnobservedTaskException
        //https://www.cnblogs.com/tianma3798/p/7003862.html?utm_source=itdadao&utm_medium=referral
        public static void UnobservedTaskException()
        {
            Task t1 = new Task(() =>
            {
                throw new System.Exception("任务并行编码中 产生 未知错误");
            });
            t1.Start();
            Console.ReadKey();
            t1 = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.ReadKey();
           
            //GC.Collect(0); //只有在回收t1对象 的时候才会触发UnobservedTaskException

        }
    }
    public class AggregateExceptionEventArgs : EventArgs
    {
        public AggregateException AggregateException { get; set; }
    }

}
