using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.Timer
{
    class 周期性执行
    {
        System.Threading.Timer timer2 = null;
        public void Build()
        {
            /**
             * bad way
             */
            System.Timers.Timer timer = new System.Timers.Timer();

            int i = 0;

            /**
             * 间隔3s执行到第三次后变更为间隔1s执行
             * 
             * 可以应用到 间隔3s执行，期间触发执行后延续3s执行
             * 
             */ 
            timer2 = new System.Threading.Timer((o) =>
            {
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(++i);
                if (i == 3)
                {
                    timer2.Change(1000, 1000);
                }
            }, null, 0, 3000);
        }
    }
}
