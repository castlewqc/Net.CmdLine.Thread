using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /* 运行
     * Wait wait = new Wait(); wait.Run(); 
     *  
     * 结果
     * Wait test start...
     * ->Running
     * WaitSleepJoin
     * do work(***)->Running
     * Stopped
     * 
     * 
     * (4) 位置 让子线程先于主线程执行
     * (1) 位置 子线程释放锁，直到它再一次获取锁时接着运行
     * (2) 位置 永远也不会执行
     * (3) 位置 子线程再次获取锁后接着运行到这
     * (5) 位置 子线程运行完后显示其线程状态
     * 
     * wait一般用于资源共享，同步代码中
     * 
     * Monitor.Enter Monitor.Exit
     **/
    public class Wait
    {
        static readonly object _locker = new object();
        static bool go;
        private void TheadMethod()
        {
            //lock (_locker) //悲观锁
            if(Monitor.TryEnter(_locker))//与上面代码相似
            {
                Console.WriteLine("->"+Thread.CurrentThread.ThreadState);
                if (!go)
                {
                    Monitor.Wait(_locker); //(1)
                }
                else
                {
                    Console.WriteLine("do work->" + Thread.CurrentThread.ThreadState); //(2)
                }
                Console.WriteLine("do work(***)->" + Thread.CurrentThread.ThreadState); //(3)
            }
        }

        public void Run()
        {
            Console.WriteLine("Wait test start...");

            Thread thread = new Thread(new ThreadStart(TheadMethod));
            thread.Start();

            Thread.Sleep(1000); //(4)
            Console.WriteLine(thread.ThreadState);


            lock (_locker)
            {
                go = true;
                Monitor.Pulse(_locker);//通知等待_locker资源的线程,在这个代码同步块代码全部执行完毕后，被通知的线程才执行
            }

            Thread.Sleep(1000); //(5)
            Console.WriteLine(thread.ThreadState);

        }
    }
}
