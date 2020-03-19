using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * 自旋锁
     */ 
    class Spin
    {
        public void One()
        {
           
            bool lockTaken = false;
            SpinLock spinLock = new SpinLock();

            Parallel.For(0, 10000, i =>
             {
                 try
                 {
                    //spinLock.Enter(ref lockTaken);
                    spinLock.TryEnter(TimeSpan.FromSeconds(2), ref lockTaken);
                     if (lockTaken)
                         Console.WriteLine(i);
                     else
                         throw new TimeoutException();
                 }
                 finally
                 {
                     if (lockTaken)
                         spinLock.Exit();
                 }
             });
          
        }
        public  static void Two()
        {
            bool isCompleted = false;
            SpinWait spinWait = new SpinWait();
            while (!isCompleted)
            {
                spinWait.SpinOnce();
                Console.WriteLine(spinWait.Count);
                Console.WriteLine(spinWait.NextSpinWillYield);
            }
        }
    }
}
