using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class TaskDemo2
    {
        public static void TaskRun()
        {
            Task t01 = new Task(() => { Thread.Sleep(2000); Console.WriteLine(Thread.CurrentThread.ManagedThreadId); });
            t01.Start();
            Task t02 = Task.Factory.StartNew(() => { Console.WriteLine(Thread.CurrentThread.ManagedThreadId); });
            //----------------------------------Task任务很简单，基本不耗时间的时候，一个线程执行完了就又被分配了一个新的任务
            Task t03 = Task.Run(() => { Console.WriteLine("t03"); });
            //Task 不会阻塞主线程
            //Task<TResult>
            Task t04 = new Task<string>(() => { Console.WriteLine(Thread.CurrentThread.ManagedThreadId); return "t04"; });
            t04.Start();
            Task t05 = Task.Factory.StartNew<string>(() => { Console.WriteLine(Thread.CurrentThread.ManagedThreadId); return "05"; });
            Task t06 = Task.Run<string>(() => { Console.WriteLine("t03"); return "t06"; });

            //task 同步
            Task t07 = new Task(() => { Thread.Sleep(1000); Console.WriteLine("t07 end."); });
            t07.RunSynchronously();
            Console.WriteLine("this is mian thread.");

            //阻塞方法 wait waitall waitany  (返回值都是void) 比thread的join功能更强大
            //task.Wait()  表示等待task执行完毕，功能类似于thead.Join()； 
            //Task.WaitAll(Task[] tasks)  表示只有所有的task都执行完成了再解除阻塞； 
            //Task.WaitAny(Task[] tasks) 表示只要有一个task执行完毕就解除阻塞

            Task task1 = new Task(() => {
                Thread.Sleep(500);
                Console.WriteLine("线程1执行完毕！");
            });
            //task1.Wait();
            task1.Start();
            Task task2 = new Task(() => {
                Thread.Sleep(1000);
                Console.WriteLine("线程2执行完毕！");
            });
            //task2.Wait();--------------------->error wait()出现在start()之前 task任务不会执行
            task2.Start();
            //阻塞主线程。task1,task2都执行完毕再执行主线程
            //执行【task1.Wait();task2.Wait();】可以实现相同功能
            Task.WaitAll(new Task[] { task1, task2 });
            Console.WriteLine("主线程执行完毕！");


            //Task的延续操作
            //whenany whenall 不会阻塞主线程 
            //continuewith ----》Task.Factory.ContinueWhenAll(Task[] tasks,Actioon countinuationAction),
            //Task.Factory.ContinueWhenAny(Task[] tasks, Actioon countinuationAction)


            //Task 任务取消 CancellationTokenSource
            //CancellationTokenSource source = new CancellationTokenSource();
            //source.Cancel();
            //source.Token.Register(Action action);
            //source.CancelAfter(5000);


           

             

        }

        //加上await 返回的是TResult

        /* https://www.cnblogs.com/CreateMyself/p/5983208.html
         * 无论方法是同步还是异步都可以用async关键字来进行标识，
         * 因为用async标识只是显示表明在该方法内可能会用到await关键字使其变为异步方法，
         * 而且将该异步方法进行了明确的划分，只有用了await关键字时才是异步操作，其余一并为同步操作。
         * 参数不能用ref out
         */
        public static  async Task<int> asyncMethod()
        {
            var task = await Task.Run(() => Calculate());
            return task;
        }
        static int Calculate()
        {
            return 1 + 1;
        }
        //异步方法 async/await c#5.0 
        public static void TaskAsyncAndAwait()
        {
            /*
             * 
             * 
             */


        Console.WriteLine(">>>>>>>>>>>>>>>>主线程启动");
            Task<string> task = GetStringAsync1();
            Console.WriteLine("<<<<<<<<<<<<<<<<主线程结束");
            Console.WriteLine($"GetStringAsync1执行结果：{task.Result}");




        }

        //异步方法中方法签名返回值为Task<TResult>，代码中的返回值为TResult。
        static async Task<string> GetStringAsync1()
        {
            Console.WriteLine(">>>>>>>>GetStringAsync1方法启动");
            string str = await GetStringAsync2();
            Console.WriteLine("<<<<<<<<GetStringAsync1方法结束");
            return str;
        }
        static async Task<string> GetStringAsync2()
        {
            Console.WriteLine(">>>>>>>>GetStringAsync2方法启动");
            string str = await GetStringFromTask();
            Console.WriteLine("<<<<<<<<GetStringAsync2方法结束");
            return str;
        }

        static Task<string> GetStringFromTask()
        {
            Console.WriteLine(">>>>GetStringFromTask方法启动");
            Task<string> task = new Task<string>(() =>
            {
                Console.WriteLine(">>任务线程启动");
                Thread.Sleep(3000);
                Console.WriteLine("<<任务线程结束");
                return "hello world";
            });
            task.Start();
            Console.WriteLine("<<<<GetStringFromTask方法结束");
            return task;
        }


        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(function());
                }
                catch (System.Exception ex)
                {
                    tcs.SetException(ex);
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }
        public static Task Run(Action act) 
        {
            var tcs = new TaskCompletionSource<object>();
            Task task = new Task(() => {

                try
                {
                     act();
                    tcs.SetResult(null);
                }
                catch (System.Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            Console.WriteLine(task.GetHashCode());
            task.Start();
            return tcs.Task; //返回的Task和执行的Task HashCode 不一样
        }
        public static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }

        public static Task MyDelay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            Thread.Sleep(milliseconds);
            tcs.SetResult(null); 
            return tcs.Task;
        }

    }
}
