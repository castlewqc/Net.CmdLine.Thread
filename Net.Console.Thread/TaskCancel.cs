using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    //通过执行Task.WaitAll(task),Task.WaitAny(task),task.Result,task.Wait()出现了异常抛出的是一个System.AggregateException;

    //通过执行task.Wait(CancellationToken)出现了异常抛出的是一个OperationCanceledException;
   public class TaskCancel
    {
        public static void TaskCancelDemo()
        {
            //向应该被取消的 System.Threading.CancellationToken 发送信号
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            //将在线程池上运行的指定工作排队，并返回代表该工作的 Task(TResult) 对象。 借助取消标记，可取消工作。
            //以异步方式执行的工作量。应用以取消工作的取消标记。
            var token = cancellationTokenSource.Token;
            Task<int> task = Task.Run(() => Sum(cancellationTokenSource.Token, 100000000), token);
            token.Register(() => { Console.WriteLine("The delegate is triggered."); }); //比异常捕获先触发

            Thread.Sleep(10);
            //Thread.Sleep(1);

            //传达取消请求
            //这是异步请求，Task可能未完成也可能已经完成
            cancellationTokenSource.Cancel();

            try
            {
                //若任务已取消，则Result会抛出AggregateException
                Console.WriteLine("Sum is " + task.Result);
            }
            catch (AggregateException aggregateException)
            {
              
                //捕获方式一
                //如果是OperationCanceledException，则返回true，表示已处理该异常
                //如果不是，则返回false，表示该异常未处理，会抛出一个新的AggregateException
                //aggregateException.Handle(handle => handle is OperationCanceledException);

                //捕获方式二
                //首先设置永远返回true，表示所有异常已处理，不再新抛出
                aggregateException.Handle(handle =>
                { 
                    //将任何OperationCanceledException对象都视为已处理（打印输出）
                    if (handle is OperationCanceledException)
                    {
                        Console.WriteLine("Sum was cancelled.");
                    }
                    else//如果不是OperationCanceledException，则也已处理（打印输出）
                    {
                        Console.WriteLine(aggregateException.Message);
                        foreach (var e in aggregateException.InnerExceptions)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    //注：若返回false，则其它（OperationCanceledException除外，因为上面已经处理过）
                    //任何异常都造成抛出一个新的AggregateException
                    //若返回true，则已处理异常，不再抛出（即使包含未处理的异常）
                    return true;
                });

                //所有异常都处理完成后，执行下面的代码
                Console.WriteLine("All exceptions are handled.");
            }
        }

        /// <summary>
        /// n以内正整数求和
        /// </summary>
        /// <param name="cancellationToken">取消操作的通知</param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static int Sum(CancellationToken cancellationToken, int n)
        {
            int sum = 0;
            for (; n > 0; n--)
            {
                //在取消标识引用的CancellationTokenSource上调用Cancel
                //如果已请求取消此标记，则引发 System.OperationCanceledException
                //TaskCanceledException : OperationCanceledException
                cancellationToken.ThrowIfCancellationRequested();
                if (cancellationToken.IsCancellationRequested)
                {
                    // 释放资源操作等等...
                    throw new OperationCanceledException(cancellationToken);
                }

                //检查n值，若太大，则抛出OverflowException
                checked { sum = sum + n; }
            }
            return sum;
        }

    }
}
