using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class 并发排他计划对
    {
        void Build()
        {
            ConcurrentExclusiveSchedulerPair 并发排他计划对 = new ConcurrentExclusiveSchedulerPair();
            var concurrentSchedulerTaskFactory = new TaskFactory(并发排他计划对.ConcurrentScheduler);
            var exclusiveSchedulerTaskFactory = new TaskFactory(并发排他计划对.ExclusiveScheduler);

            // 同步执行的
            var task = new Task(() => { Console.WriteLine("并发排他计划对 " + DateTime.Now); System.Threading.Thread.Sleep(5000); });
            var task2 = new Task(() => { Console.WriteLine("并发排他计划对 " + DateTime.Now); System.Threading.Thread.Sleep(2000); });
            task.Start(exclusiveSchedulerTaskFactory.Scheduler);
            task2.Start(exclusiveSchedulerTaskFactory.Scheduler);
        }
    }
}
