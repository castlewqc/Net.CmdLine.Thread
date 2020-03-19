using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    //类默认 本assembly中可以访问
    class ThreadPoolDemo
    {
        /*
         * RegisterWaitForSingleObject
         * 
         * 向线程池添加一个可以定时执行的方法，
         * 第四个参数millisecondsTimeOutInterval 就是用来设置间隔执行的时间，
         * 但是这里第五个参数executeOnlyOnce 会对第四个参数起作用，当它为true时，表示任务仅会执行一次，
         * 就是说它不会，像Timer一样，每隔一定时间执行一次
         * ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false), new WaitOrTimerCallback(??), state,5000, false);
         * 
         * 
         * 
         * ThreadPool 可以和AutoResetEvent,ManualResetEvent一起用来通知主线程线程池什么时候都完成了
         * 
         * 
         * CLR线程池分为工作者线程(workerThreads)与I/O线程(completionPortThreads)两种:
         * 使用CLR线程池的工作者线程一般有两种方式：
         *
         * 通过ThreadPool.QueueUserWorkItem()方法；
         * 通过委托；
         * 
         * GetAvailableThreads
         * GetMaxThreads
         * QueueUserWorkItem
         * 
         * 最佳线程数目 = （（线程等待时间+线程cpu时间）/ 线程cpu时间 ） * cpu核心数目
         * io密集型 需要越多线程
         * cpu密集型 越少线程，减少切换上下文时间
         * 
         *  高并发，任务执行时间短 cpu核心数+1
         *  
         * 
         *  CancellationTokenSource ctsToken = new CancellationTokenSource();
         *  ThreadPool.QueueUserWorkItem(new WaitCallback(??),ctsToken)
         *  ctsToken.Cancel(true/false) true表示捕获第一个异常就返回 false捕捉所有异常
         *  
         *  if (token.IsCancellationRequested) {return;}
         *  
         *  CancellationToken.None 创建的线程不能被取消 那么token.CanBeCanceled永远为false
         *  
         *  //主线程中添加线程取消回调函数
         *  CancellationTokenSource.Token.Register(()=>???) 回调函数
         */
        public void Work()
        {
            string s = "321";
            ThreadPool.QueueUserWorkItem(new WaitCallback(CountProcess),s);
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetEnvironmentVariables));
        }
        /// <summary>  
        /// 统计当前正在运行的系统进程信息  
        /// </summary>  
        /// <param name="state"></param>  
        private void CountProcess(object state)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                try
                {
                    Console.WriteLine("进程信息:Id:{0},ProcessName:{1},StartTime:{2}", p.Id, p.ProcessName, p.StartTime);
                }
                catch (Win32Exception e)
                {
                    Console.WriteLine("ProcessName:{0}", p.ProcessName);
                }
                finally
                {
                }
            }
            Console.WriteLine("获取进程信息完毕。");
        }
        /// <summary>  
        /// 获取当前机器系统变量设置  
        /// </summary>  
        /// <param name="state"></param>  
        public void GetEnvironmentVariables(object state)
        {
            IDictionary list = System.Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry item in list)
            {
                Console.WriteLine("系统变量信息:key={0},value={1}", item.Key, item.Value);
            }
            Console.WriteLine("获取系统变量信息完毕。");
        }
    }
}
