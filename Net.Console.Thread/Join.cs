using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{

    /*
     * Join  用来阻塞调用线程，直到某个线程终止或经过了指定时间为止
     * 
     * 
     * public void Join();
     * public bool Join(int millisecondsTimeout);
     * public bool Join(TimeSpan timeout);
     * 
     * 
     * 线程启动后才可以Join
     * A线程中B.Join() B线程执行完成A才会继续执行
     * 
     **/
    public class Join
    {
        public void Run()
        {
            Console.WriteLine("start.");
            Thread thOne = new Thread(new ThreadStart(ThreadMethod));
            Thread thTwo = new Thread(new ParameterizedThreadStart(ThreadMethodWithParam));
            thOne.Name = "thOne";
            thTwo.Name = "thTwo";
            thOne.Start();
            //thOne.Join(1000); 不起作用 因为1线程阻塞2线程，2线程阻塞主线程，则1线程启动必定完成后，再调依次执行2线程，主线程
            thTwo.Start(thOne);
            thTwo.Join();

            Console.WriteLine("end.");

        }
        private static void ThreadMethod()
        {
            int i = 3;
            while (i > 0)
            {
                Console.WriteLine(i);
                --i;
                Thread.Sleep(1000);
            }
        }
        private static void ThreadMethodWithParam(Object beforeThread)
        {
            try
            {
                (beforeThread as Thread).Join();
                Console.WriteLine("Thread Start->" + Thread.CurrentThread.Name);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Thread Start->" + Thread.CurrentThread.Name);
                Console.WriteLine(e.Message);
            }

        }
    }
}
