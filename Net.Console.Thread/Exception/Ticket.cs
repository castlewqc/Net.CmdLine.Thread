using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.Exception
{
    public class Ticket 
    {
        private static volatile int ticketCount = 100;
        public void Sold()
        {
            while (ticketCount > 0)
            {
                if (ticketCount > 0)
                {
                    ticketCount--;
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + "卖1张票，剩余" + ticketCount + "张");
                    Thread.Sleep(100);
                }
                else
                {
                    Console.WriteLine("票已卖完");
                    return;
                }
            }
        }
    }
}
