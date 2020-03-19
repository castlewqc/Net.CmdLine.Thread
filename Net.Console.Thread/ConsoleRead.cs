using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class ConsoleRead
    {
        AutoResetEvent _get = new AutoResetEvent(false);
        AutoResetEvent _got = new AutoResetEvent(false);
        static string value;
        public void  TryReadLine()
        {
            int TimeOutCount = 0;
            Thread thread = new Thread(() => 
            {
                _get.WaitOne();
                value = Console.ReadLine();
                _got.Set();
            });
            thread.Start();
            _get.Set();
            while(!_got.WaitOne(TimeSpan.FromSeconds(2)))
            {
                if (TimeOutCount++ > 10)
                {
                    thread.Abort();
                    throw new TimeoutException();
                }
            }
            Console.WriteLine(value+".");
        }
    }
}
