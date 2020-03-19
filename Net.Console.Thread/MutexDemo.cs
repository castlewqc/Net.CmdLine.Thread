using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /* 互斥对象
     * 
     * 互斥量跟临界区中提到的Monitor很相似，只有拥有互斥对象的线程才具有访问资源的权限，
     * 由于互斥对象只有一个，因此就决定了任何情况下此共享资源都不会同时被多个线程所访问。
     * 当前占据资源的线程在任务处理完后应将拥有的互斥对象交出，以便其他线程在获得后得以访问资源。
     * 互斥量比临界区复杂，因为使用互斥不仅仅能够在同一应用程序不同线程中实现资源的安全共享，
     * 而且可以在不同应用程序的线程之间实现对资源的安全共享
     * 
     * Metux 确保一块代码块同一时间只有一个线程在执行，一般用于跨进程的线程同步，
     * 而Monitor用于进程内的线程同步
     * 
     * WaitOne() 类似于Monitor的TryEnter/Enter 而不是Wait,Monitor.Wait()会先释放已获得的资源再等待资源
     * 而WaitOne() 阻塞自己等待信号set
     * ReleaseMutex() 释放当前Metux一次
     * 
     * 
     * 使用Mutex需要注意的两个细节
     *
     *可能你已经注意到了，例子中在给Mutex命名的字符串里给出了一个“Global\”的前缀。这是因为在运行终端服务（或者远程桌面）的服务器上，已命名的全局 mutex 有两种可见性。如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见，在这种情况下，服务器上各个其他终端服务器会话中都可以拥有一个名称相同的独立 mutex。如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。在终端服务器会话中，只是名称前缀不同的两个 mutex 是独立的 mutex，这两个 mutex 对于终端服务器会话中的所有进程均为可见。即：前缀名称“Global\”和“Local\”仅用来说明 mutex 名称相对于终端服务器会话（而并非相对于进程）的范围。最后需要注意“Global\”和“Local\”是大小写敏感的。
     *既然父类实现了IDisposalble接口，那么说明这个类一定需要你手工释放那些非托管的资源。所以必须使用try/finally，亦或我讨厌的using，调用Close()方法来释放Mutex所占用的所有资源！
     **/
    class MutexDemo
    {
        #region 检测进程
        static bool flag = false;
        Mutex x = new Mutex(true, @"Global\program",out flag);
        #endregion

        private static Mutex mut = new Mutex();
        private const int numIterations = 1;
        private const int numThreads = 3;

        public void Run()
        {
            // Create the threads that will use the protected resource.
            for (int i = 0; i < numThreads; i++)
            {
                Thread myThread = new Thread(new ThreadStart(MyThreadProc));
                myThread.Name = String.Format("Thread{0}", i + 1);
                myThread.Start();
            }

            // The main thread exits, but the application continues to
            // run until all foreground threads have exited.
        }

        private static void MyThreadProc()
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource();
            }
        }

        // This method represents a resource that must be synchronized
        // so that only one thread at a time can enter.
        private static void UseResource()
        {
            // Wait until it is safe to enter.
            mut.WaitOne();

            Console.WriteLine("{0} has entered the protected area",
                Thread.CurrentThread.Name);

            // Place code to access non-reentrant resources here.

            // Simulate some work.
            Thread.Sleep(500);

            Console.WriteLine("{0} is leaving the protected area\r\n",
                Thread.CurrentThread.Name);

            // Release the Mutex.
            mut.ReleaseMutex();
        }
    }
}
