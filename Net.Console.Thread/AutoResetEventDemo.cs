using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * 
     * set：指的是将一个事件设置为有信号，那么被这个事件堵塞的线程就会继续下去。
     * reset：指的是将一个事件设置为无信号，那么尝试继续的事件就会被堵塞。
     * AutoResetEvent 可比做闸门 构造函数参数true 默认闸门开启，false 默认闸门关闭
     * waitOne() 表示刷卡，你线程有没有票（信号）
     * set() 给信号（票）,开启闸门
     * reset() 关闭闸门 AutoResetEvent表示闸门开启让一线程过了后会自动关闭
     * 
     * 运行
     * AutoResetEventDemo waitHandler = new AutoResetEventDemo();
     * waitHandler.Run();
     **/
    class AutoResetEventDemo
    {
        private static AutoResetEvent event_1 = new AutoResetEvent(true);
        private static AutoResetEvent event_2 = new AutoResetEvent(false);

        public void Run()
        {
            Console.WriteLine("Press Enter to create three threads and start them.\r\n" +
                           "The threads wait on AutoResetEvent #1, which was created\r\n" +
                           "in the signaled state, so the first thread is released.\r\n" +
                           "This puts AutoResetEvent #1 into the unsignaled state.");
            Console.ReadLine();

            for (int i = 1; i < 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }
            Thread.Sleep(250);

            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine("Press Enter to release another thread.");
                Console.ReadLine();
                event_1.Set();
                Thread.Sleep(250);
            }

            Console.WriteLine("\r\nAll threads are now waiting on AutoResetEvent #2.");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Press Enter to release a thread.");
                Console.ReadLine();
                event_2.Set();
                Thread.Sleep(250);
            }

            // Visual Studio: Uncomment the following line.
            //Console.Readline();
        }
        static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine("{0} waits on AutoResetEvent #1.", name);
            event_1.WaitOne();
            Console.WriteLine("{0} is released from AutoResetEvent #1.", name);

            Console.WriteLine("{0} waits on AutoResetEvent #2.", name);
            event_2.WaitOne();
            Console.WriteLine("{0} is released from AutoResetEvent #2.", name);

            Console.WriteLine("{0} ends.", name);



        }
    }
}
