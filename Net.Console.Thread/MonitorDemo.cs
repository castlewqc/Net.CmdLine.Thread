using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * Monitor只有获取锁的方法Enter, TryEnter；释放锁的方法Wait, Exit；还有消息通知方法Pulse, PulseAll。
     *  Monitor.Pulse();//通知等待_locker资源的线程,在这个代码同步块代码全部执行完毕后，被通知的线程才执行
     *  Monitor.Wait(m_smplQueue, 50000) 阻塞return true
     * 运行
     * new MonitorDemo().Run();
     *   
     * 运行结果
1.Wait
2.Pulse
...
...
...
...
Enqueue
1.Pulse
1.Wait
Dequeue
0
2.Pulse...
Enqueue
1.Pulse
1.Wait
Dequeue
1
2.Pulse...
Enqueue
1.Pulse
1.Wait
Dequeue
2
2.Pulse...
Enqueue
1.Pulse
1.Wait
Dequeue
     * 
     * 
     **/
    class MonitorDemo
    {
        const int MAX_LOOP_TIME = 10;
        Queue m_smplQueue = new Queue();
        public void Run()
        {
            //Create the MonitorSample object.
            MonitorDemo test = new MonitorDemo();
            //Create the first thread.
            Thread tFirst = new Thread(new ThreadStart(test.FirstThread));
            //Create the second thread.
            Thread tSecond = new Thread(new ThreadStart(test.SecondThread));
            //Start threads.
            tFirst.Start();
            tSecond.Start();
            //wait to the end of the two threads
            tFirst.Join();
            tSecond.Join();
            //Print the number of queue elements.
            Console.WriteLine("Queue Count = " + test.GetQueueCount().ToString());
        }
        public void FirstThread()
        {
            int counter = 0;
            lock (m_smplQueue)
            {
                while (counter < MAX_LOOP_TIME)
                {
                    //Wait, if the queue is busy.

                    Console.WriteLine("1.Wait");
                    Monitor.Wait(m_smplQueue);
                    //Push one element.
                    Console.WriteLine("Enqueue");
                    m_smplQueue.Enqueue(counter);
                    //Release the waiting thread.
                    Console.WriteLine("1.Pulse");
                    Monitor.Pulse(m_smplQueue);

                    counter++;
                }
                int i = 0;
            }
        }
        public void SecondThread()
        {
            lock (m_smplQueue)
            {
                Console.WriteLine("2.Pulse");
                //Release the waiting thread.
                Monitor.Pulse(m_smplQueue);//从不同步的代码块中调用了对象同步方法
                //Wait in the loop, while the queue is busy.
                //Exit on the time-out when the first thread stops.
                Thread.Sleep(2000);
                Console.WriteLine("...");
                Console.WriteLine("...");
                Console.WriteLine("...");
                Console.WriteLine("...");
                while (Monitor.Wait(m_smplQueue, 50000))
                {
                    Console.WriteLine("Dequeue");
                    //Pop the first element.
                    int counter = (int)m_smplQueue.Dequeue();
                    //Print the first element.
                    Console.WriteLine(counter.ToString());
                    //Release the waiting thread.
                    Console.WriteLine("2.Pulse...");
                    Monitor.Pulse(m_smplQueue);
                }
            }
        }
        //Return the number of queue elements.
        public int GetQueueCount()
        {
            return m_smplQueue.Count;
        }
    }
}
