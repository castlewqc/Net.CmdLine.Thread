using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * 读写锁是通过调用AcquireReaderLock，ReleaseReaderLock，AcquireWriterLock，ReleaseWriterLock来完成读锁和写锁控制的
     * 
     * 读的时候插入写操作请使用UpgradeToWriterLock和DowngradeFromWriterLock来进行操作，而不是释放读锁
     * 
     * 
     **/
    class ReaderWriterLockDemo
    {

        private ReadWrite rw = new ReadWrite();
        public void Run()
        {


            ReaderWriterLockDemo e = new ReaderWriterLockDemo();

            //Writer Threads
            Thread wt1 = new Thread(new ThreadStart(e.Write));
            wt1.Start();
            Thread wt2 = new Thread(new ThreadStart(e.Write));
            wt2.Start();

            //Reader Threads
            Thread rt1 = new Thread(new ThreadStart(e.Read));
            rt1.Start();
            Thread rt2 = new Thread(new ThreadStart(e.Read));
            rt2.Start();

            Console.ReadLine();

        }
        private void Write()
        {
         
            int a = 10;
            int b = 11;
            Console.WriteLine("************** Write *************");

            for (int i = 0; i < 5; i++)
            {
                this.rw.WriteInts(a++, b++);
                Thread.Sleep(1000);
            }
            
        }

        private void Read()
        {
            int a = 10;
            int b = 11;
            Console.WriteLine("************** Read *************");

            for (int i = 0; i < 5; i++)
            {
                this.rw.ReadInts(ref a, ref b);
                Console.WriteLine("For i = " + i
                    + " a = " + a
                    + " b = " + b
                    + " TheadID = " + Thread.CurrentThread.GetHashCode());
                Thread.Sleep(1000);
            }
        }
    }

    public class ReadWrite
    {
        private ReaderWriterLock rwl;
        private int x;
        private int y;

        public ReadWrite()
        {
            rwl = new ReaderWriterLock();
        }

        public void ReadInts(ref int a, ref int b)
        {
            rwl.AcquireReaderLock(Timeout.Infinite);
            try
            {
                a = this.x;
                b = this.y;
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        public void WriteInts(int a, int b)
        {
            rwl.AcquireWriterLock(Timeout.Infinite);
            try
            {
                Thread.Sleep(3000);
                this.x = a;
                this.y = b;
                Console.WriteLine("x = " + this.x
                    + " y = " + this.y
                    + " ThreadID = " + Thread.CurrentThread.GetHashCode());
            }
            finally
            {
                rwl.ReleaseWriterLock();
            }
        }
    }
}
