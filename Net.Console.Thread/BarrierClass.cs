using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * Barrier 屏障 不同线程 相同阶段执行完后才可以执行下一阶段 
     */
    class BarrierClass
    {
        public void Do()
        {
            int taskSize = 5;
            Barrier barrier = new Barrier(taskSize, (b) =>
            {
                Console.WriteLine(string.Format("{0}当前阶段编号：{1}{0}", "-".PadRight(15, '-'), b.CurrentPhaseNumber));
            });

            var tasks = new Task[taskSize];

            for (int i = 0; i < taskSize; i++)
            {
                tasks[i] = Task.Factory.StartNew((n) =>
                {
                    Console.WriteLine("Task : #{0}   ---->  处理了第一部份数据。", n);
                    barrier.SignalAndWait();

                    Console.WriteLine("Task : #{0}   ---->  处理了第二部份数据。", n);
                    barrier.SignalAndWait();

                    Console.WriteLine("Task : #{0}   ---->  处理了第三部份数据。", n);
                    barrier.SignalAndWait();

                }, i);
            }

            Task.WaitAll(tasks);
        }
    }
}
