namespace Net.CmdLine.ThreadStudy
{
    using Net.CmdLine.ThreadStudy.Timer.Quzrtz;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using 反射;
    public class Fruit { }
    public class Apple : Fruit { }

    public interface IPlate<in T> 
    {
        //T get();
        void set(T item);

    }
    public class Plate<T> : IPlate<T> where T:Fruit
    {
        private T item;

        public Plate(T t)
        {
            this.item = t;
        }

        //public T get()
        //{
        //    return item;
        //}

        public void set(T item)
        {
            this.item = item;
        }

        public static implicit operator Plate<T>(Plate<Apple> v)
        {
            throw new NotImplementedException();
        }
    }
    internal class Program
    {

        static void Print(object objToken)
        {
            Action<object> act = delegate (object obj)
             {
                 Console.WriteLine(obj.ToString());
             };
            act("321");

            //todo 有问题
            Apple apple = new Apple();
            Plate<Fruit> p = new Plate<Apple>(apple);


            CancellationToken token = (CancellationToken)objToken;
            Console.WriteLine("Print , 开始等待{0}...", System.Threading.Thread.CurrentThread.ManagedThreadId);
            if (token.WaitHandle.WaitOne())
            {
                Console.WriteLine("直到 调用 Cancel() 执行此处 ");
            }



            //while (!token.IsCancellationRequested) {
            //    //调用Cancel() 后 退出循环
            //}

            Console.WriteLine("执行退出 {0}!", System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        //       buffer的大小始终是buffersize
        //       但读的时候如果count比buffersize大
        //       1、buffer里没有数据，直接读入指定的array
        //       2、buffer里有数据，先把数据复制到array，然后再读取剩余的字节
        //           不管怎样只会读1次
        //

        //文件流的缓冲区大小.
        //设的过小,可能导致不能读出预期长度的数据
        //设的过大, 浪费内存
        //所以使用的时候一般都会事先预估数据块的大小来设置buffersize
        //或者采取多次读取的方法来回避这个问题  -->  多次读取应该设置byte[] 比默认4096 小？

        private static  void  OnShow(string name)
        {
            Console.WriteLine("OnShow Thread ID "+Thread.CurrentThread.ManagedThreadId.ToString());
            Thread.Sleep(3000);
            Console.WriteLine("->"+name);
        }
        private  static void Callback(IAsyncResult ar)
        {
            Console.WriteLine("Callback Thread ID " + Thread.CurrentThread.ManagedThreadId.ToString());
            object o = ar.AsyncState;
            Console.WriteLine("Call back  " + o);
        }
        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            foreach (System.Exception item in e.Exception.InnerExceptions)
            {
                Console.WriteLine("异常类型：{0}{1}来自：{2}{3}异常内容：{4}",
                    item.GetType(), Environment.NewLine, item.Source,
                    Environment.NewLine, item.Message);
            }
            e.SetObserved();
        }

      
        public static void Main(String[] args)
        {
            new Sample().Build();
            Console.ReadKey();
            LockDemo lockDemo = new LockDemo();
            lockDemo.Test(20);
          
            //++++++++++++++++++++++++++++++++++++++++++++++++++
          
            LockDemo lockDemo2 = new LockDemo();

            // lockthis -> 同一线程互斥，不同线程不互斥
            //互斥同一String引用 不同线程互斥
            Thread thread1 = new Thread(lockDemo.LockString);
            Thread thread2 = new Thread(lockDemo2.LockString);
            thread1.Start();thread2.Start();
            Console.ReadKey();
            //++++++++++++++++++++++++++++++++++++++++++++++++++


            var ttt = AsyncAwaitSample.Method05();
            ttt.Wait();
            Console.WriteLine("something1");
            Console.WriteLine("something2" + ttt.Result);
            Console.WriteLine("something3");
            Console.ReadKey();

            

            Thread.SpinWait(1000);

            //一种多线程取消任务开关对象
            CancellationTokenSource s1 = new CancellationTokenSource();
            CancellationTokenSource s2 = new CancellationTokenSource();

            //s1 , 或者 s2 取消，导致 s3 取消
            CancellationTokenSource s3 = CancellationTokenSource.CreateLinkedTokenSource(s1.Token, s2.Token);

            //异步执行结束 回调
            s1.Token.Register(new Action(() => {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            s2.Token.Register(new Action(() =>
            {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            s3.Token.Register(new Action(() =>
            {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            //异步执行
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s1.Token);  //Token 中包含回调信息，执行结束 触发 Register 关联方法。
                                                                              // System.Threading.Thread.Sleep(2000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s2.Token);
            //  System.Threading.Thread.Sleep(2000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s3.Token);



            Console.WriteLine();
            s2.CancelAfter(3000); //若各线程传递各自tocken ,执行回调线程：  s2 , s3
                                  //s1.CancelAfter(3000); //若各线程传递各自tocken ,执行回调线程:  s1 , s3

            //s3.Cancel(); //若各线程传递各自tocken ,只触发 s3 , 不会触发 s1 , s2 回调。

            //注意: 若各线程传递s3.token ,  s1 , s2 任意Cancle , s1 , s2 , s3 均会回调。

            Console.ReadKey();


            //+++++++++++++++++++++++++++
            TaskDemo2.TaskAsyncAndAwait();
            Console.Read();
            //DelegateSample.Main();

            var a = new Program();
            var b = new Program();
            var c = new Program();
            //+++++++++
            //测试实例对象加上互斥锁后，是否可以访问+修改
            //lcok 对象修改后还是否有效 
            Action<object> act = delegate(object v)
            {
                new LockStatic.LockStaticVariable().Run(v.ToString());
            };
            var task11 = new Task(act, a);
            
            var task22 = new Task(act,b);
            var task33 = new Task(act,c);
            Console.WriteLine(a.GetHashCode());
            Console.WriteLine(b.GetHashCode());
            task11.Start();
            Thread.Sleep(100);
            task22.Start();
                  Thread.Sleep(100);
            task33.Start();
            //+++++++++

            //ParalleDemo.Run2();
            Console.Read();

            string[] text1 = { "Today is 2018-06-06", "weather is sunny", "I am happy" };
                         var tokens1 = text1.SelectMany(s => s.Split(' '));
                         foreach (string token in tokens1)
                            Console.Write("{0}.", token);
                      Console.ReadKey();

            //var t23 = new Exception.Test();
            //new Thread(t23.Run).Start("我是第一个线程");
            ////Thread.Sleep(500);

            //var t24 = new Exception.Test();
            //new Thread(t24.Run).Start("我是第二个线程");
            //Thread.Sleep(500);

            //Exception.Test.obj = "new_obj";

            //var t25 = new Exception.Test();
            //new Thread(t25.Run).Start("我是第三个线程");
            ////Thread.Sleep(500);

            //var t26 = new Exception.Test();
            //new Thread(t26.Run).Start("我是第四个线程");
            ////Thread.Sleep(500);


            new Thread(Exception.Test.Runstatic).Start("我是第一个线程");
            new Thread(Exception.Test.Runstatic).Start("我是第二个线程");
            Thread.Sleep(500);
            //var old = Exception.Test.obj;
          
            //Console.WriteLine(Object.ReferenceEquals(Exception.Test.obj, old));
            new Thread(Exception.Test.Runstatic).Start("我是第三个线程");
           // Exception.Test.obj = new Exception.Obj { ticket = new Exception.Ticket(), str = "new_obj", i = 0 };
            new Thread(Exception.Test.Runstatic).Start("我是第四个线程");
            new Thread(Exception.Test.Runstatic).Start("我是第五个线程");
            new Thread(Exception.Test.Runstatic).Start("我是第六个线程");
            //var t27 = new Exception.Test();


            Console.Read();


            new Exception.TicketRun().Run();
            new MonitorDemo().Run();
            new ConsoleRead().TryReadLine();
            //TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(TaskScheduler_UnobservedTaskException);
            //TaskException.UnobservedTaskException();
            // TaskCancel.TaskCancelDemo();
            //Spin.Two();
            // new MonitorDemo().Run();
            Console.Read();

            byte[] buffer = new byte[10]; //重要 buffer.Initialize(); str.TrimEnd('\0');//将后面冗余的'\0'去掉
            string path = @"d:\\2.txt";
            //using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 10))//??
            //{

            //    using (var sr = new StreamReader(fs))
            //    {
            //        while (true)
            //        {
            //            string s = sr.ReadLine();
            //            if (!string.IsNullOrWhiteSpace(s))
            //            {
            //                Console.WriteLine(s);
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }
            //    }
            //}

            //流有一个缓存区，满了就写入磁盘 默认4096
            //不用using and flush 不会写入磁盘
            FileStream fs = new FileStream("d:\\Log2019-06-27.log", FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 10);
          var sw = new StreamWriter(fs);
                
                    Console.WriteLine(fs.Length);
                    sw.WriteLine("房间大司法局撒地方每节课的祭祀哦发大水");
                    Console.WriteLine(fs.Length);
                    //sw.Flush(); //注释掉不会写入文件
                    Console.WriteLine(fs.Length);
                
            
                Console.Read();
            //TaskDemo2.Delay(5000).Wait();
            //Console.WriteLine("Just For Test.");


            var tr=TaskDemo2.Run(()=>Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString()));
            Console.WriteLine(tr.GetHashCode());
            Console.Read();
            return;
            //Simple ds = new Simple();
            //ds.DoRun();
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            //Console.Read();

            CancellationTokenSource source = new CancellationTokenSource();
            source.Cancel();

            Task task1 = new Task(() =>
            {
                Console.WriteLine("task1:" + DateTime.Now.ToString("O"));
                Thread.Sleep(5000);
                Console.WriteLine("task1 tid={0}， dt={1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
            });

            var task2 = task1.ContinueWith(t =>
            {
                Console.WriteLine("task2 tid={0}， dt={1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
            }, source.Token, TaskContinuationOptions.LazyCancellation, TaskScheduler.Current);
            //使用LazyCancellation  task3依旧等待task1完成
            //不使用LazyCancellation而取消task2  这三个task的先后顺序无效

            var task3 = task2.ContinueWith(t =>  
            {
                Console.WriteLine("task3:"+DateTime.Now.ToString("O"));
                Thread.Sleep(3000);
                Console.WriteLine("task3 tid={0}， dt={1}  {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, task2.Status);
            });

            task1.Start();
            Console.ReadKey();
            return;
            Task t2 = new Task((state)=>Console.WriteLine("->"+state),"someState");
            t2.Start();
            Task<int> task = Task.Factory.StartNew(() => {
                Console.WriteLine("task"+Thread.CurrentThread.ManagedThreadId.ToString());
                Thread.Sleep(3000);
                return 1;
            }, TaskCreationOptions.LongRunning);//会使用thread来运行task

            DoWork();

            //不阻塞线程
            task.ContinueWith<string>((r, s) =>
            {
                Console.WriteLine("s:" + s);
                Console.WriteLine("task continuewith" + Thread.CurrentThread.ManagedThreadId.ToString());
                Console.WriteLine("continuewith:" + r.Result + 100);// 这个r.Result 肯定是已得到的
                return "我是一个返回值";
            }, "我是一个输入参数", TaskContinuationOptions.ExecuteSynchronously);

            //阻塞主线程 因为使用到了RESULT
            //string rs=task.ContinueWith<string>((r,s) => {
            //    Console.WriteLine("s:" + s);
            //    Console.WriteLine("task continuewith" + Thread.CurrentThread.ManagedThreadId.ToString());
            //    Console.WriteLine("continuewith:"+r.Result + 100);
            //    return "我是一个返回值";
            //},"我是一个输入参数",TaskContinuationOptions.ExecuteSynchronously).Result;
            //Console.WriteLine("ContinueWith Result" + rs);


            //不阻塞主线程  和task同一线程执行？
            task.GetAwaiter().OnCompleted(() => {
                Console.WriteLine("GetAwaiter" + Thread.CurrentThread.ManagedThreadId.ToString());
                Console.WriteLine("Completed: " + task.Result);
                });
            //Console.WriteLine(task.Result);
            Console.WriteLine("主线程开始");

            Task<int> kk=async(); //不阻塞
            kk.GetAwaiter().OnCompleted(() => { Console.WriteLine(kk.Result); });
            //result 阻塞
            //Console.WriteLine(async().Result);
            Console.WriteLine("主线程结束");
            try
            {
                //    TaskDemo2.TaskAsyncAndAwait();
                // new AutoResetEventDemo().Run();
            }
            finally { Console.ReadLine(); }
        }

        private static async Task<int> async()
        {
            var k=  await asyncMethod();
            Console.WriteLine("...");
            return k;
        }
        private static void DoWork()
        {
            Console.WriteLine("do work");
        }

        public static async Task<int> asyncMethod()
        {
            var t = new Task<int>(() => { Thread.Sleep(3000); for (int i = 0; i < 3; i++) Console.WriteLine(i + 1); return Calculate(); });
            t.Start();
         
            Console.WriteLine("->");
            //https://www.cnblogs.com/jesse2013/p/async-and-await.html
            //t.GetAwaiter().GetResult();
            var task = await t;
            Console.WriteLine("-->");
            return task;
        }

        internal static int Calculate()
        {
            return 1 + 1;
        }

        class Simple
        {
            Stopwatch sw = new Stopwatch();
            public void DoRun()
            {
                Console.WriteLine("Caller: Before call");
                ShowDealyAsync();
                Console.WriteLine("Caller: After call");
            }

           
            private async void ShowDealyAsync()
            {
                //运行在主线程上
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
                sw.Start();
                Console.WriteLine("Before Delay: {0}", sw.ElapsedMilliseconds + "_" + Thread.CurrentThread.ManagedThreadId.ToString());
                await Task.Delay(5000);      //执行到await表达式时，立即返回到调用方法，等待5秒后执行后续部分
                //运行在其他线程上
                Console.WriteLine("After Delay : {0}", sw.ElapsedMilliseconds+"_"+Thread.CurrentThread.ManagedThreadId.ToString());//后续部分
            }
        }

    }
}
