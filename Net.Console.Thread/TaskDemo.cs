using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * 当使用Task进行多线程任务开发时,不建议使用Wait方法或者Result属性,去阻塞主线程,原因如下:

       i、会卡界面

       ii、伸缩性好的软件,不会这么做,除非迫不得已

       iii、很有可能创建新的线程,浪费资源(如果主线程执行的足够快,它可能自己去完成子线程的任务,而不是创建新的线程)

       iiii.async await 
     * 
     * TaskCreationOptions 
     * 1.AttachedToParent 父任务等待所有子任务完成
     * 2.DenyChildAttach 不允许子任务附加到父任务
     * 3.HideScheduler 子任务不使用父类Task的Scheduler 而是使用默认的
     * 4.LongRunning  Task运行在Thread上
     * 5.PreferFairness 尽可能公平的方式安排任务
     * 
     * TaskContinuationOptions https://www.cnblogs.com/yaopengfei/p/8213263.html
     * 　　①. LazyCancellation：在延续取消的情况下，防止延续的完成直到完成先前的任务。-> 作用：保持task之间的先后顺序
　　       ②. ExecuteSynchronously：希望执行前面那个task的thread也在执行本延续任务
　　       ③. NotOnRanToCompletion和OnlyOnRanToCompletion
　　　　      NotOnRanToCompletion：延续任务必须在前面task非完成状态才能执行
　　　　      OnlyOnRanToCompletion：延续任务必须在前面task完成状态才能执行
     * 如果调用多任务延续（即：调用TaskFactory或TaskFactory<TResult>的静态ContinueWhenAll和ContinueWhenAny方法）时，
     * NotOn和On六个标识或标识的组合都是无效的。也就是说，无论先驱任务是如何完成的，ContinueWhenAll和ContinueWhenAny都会执行延续任务。
     * 
     * Task 任务
     * wait result会阻塞线程
     * result有返回值
     * 
     * token.IsCancellationRequested token.ThrowIfCancellationRequested 取消
     * token.Register(callback function)
     * 
     * continueWith 
     * TaskCreationOption 选项 https://docs.microsoft.com/zh-cn/dotnet/api/system.threading.tasks.taskcontinuationoptions?redirectedfrom=MSDN&view=netframework-4.7.2
     *
     * 
     * 
     * Task.FromResult(result) Task.FromResult用来创建一个带返回值的、已完成的Task。 返回一个包含返回值的task
     *
     *
    
     */
    class TaskDemo
    {
        public static  void Run()
        {
           
               CancellationTokenSource cts = new CancellationTokenSource();
            Console.WriteLine("主线程开始执行");
           
            Console.WriteLine("主线程"+Thread.CurrentThread.ManagedThreadId);
            Task task = new Task(() =>
                  Console.WriteLine("task 开始执行->" + Thread.CurrentThread.ManagedThreadId)
              );
           
            task.Start();
            task.Wait();
      

            var taskWithResult = Task.Run(() => TaskWithParameter(10));
            Console.WriteLine(taskWithResult.Result.ToString());//相当于调用wait方法
            //taskWithResult.Wait();


            Task.Run(() => WorkLong(cts.Token));
            Thread.Sleep(1000);
            cts.Token.Register(()=>Console.WriteLine("取消回调"));
            cts.Cancel();//当我们调用了Cancel()方法之后，.NET Framework不会强制性的去关闭运行的Task。


            taskWithResult.ContinueWith(
                t=>Console.WriteLine("ContinueWith->"+t.Result.ToString()),
                TaskContinuationOptions.OnlyOnRanToCompletion);


            var parentTask = new Task<string[]>(() =>
              {
                  var result = new string[2];
                  new Task(() => result[0] = ChildThreadOne(), TaskCreationOptions.AttachedToParent).Start();
                  new Task(() => result[1] = ChildThreadOne(), TaskCreationOptions.AttachedToParent).Start();
                  return result;
              });
            parentTask.Start();
            parentTask.ContinueWith(x =>
            {
                Console.WriteLine("当父任务执行完毕时,CLR会唤起一个新线程,将父任务的返回值(子任务的返回值)输出,所以这里不会有任何的线程发生阻塞");
                foreach (var re in parentTask.Result)
                {
                    Console.WriteLine("子任务的返回值分别为:{0}", re);
                }
            });
            Console.WriteLine("主线程不会阻塞,它会继续执行");
            Console.WriteLine("主线程执行结束");
        }
        private  static int TaskWithParameter(int i)
        {
            Console.WriteLine("TaskWithParameter"+Thread.CurrentThread.ManagedThreadId);
            return --i;
        }
        private static  void WorkLong(CancellationToken token)
        {

            try
            {
                while (true)
                {
                    //token.ThrowIfCancellationRequested();

                    if (token.IsCancellationRequested) { Console.WriteLine("token 取消"); return; }
                  
                    Console.WriteLine("work doing"); Thread.Sleep(500);
                }

                
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static string ChildThreadOne() { return DateTime.Now.ToString(); }
       
    }
}
