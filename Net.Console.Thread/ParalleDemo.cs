using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class ParalleDemo
    {
        public static void Run()
        {
            Stopwatch watch1 = new Stopwatch();
            watch1.Start();
            for (int i = 1; i <= 10; i++)
            {
                Console.Write(i + ",");
                Thread.Sleep(1000);
            }
            watch1.Stop();
            Console.WriteLine(watch1.Elapsed);
            Stopwatch watch2 = new Stopwatch();
            watch2.Start();
            //会调用线程池中的线程 ？ 用的是逻辑线程
            Parallel.For(1, 11, i =>
            {
                Console.WriteLine(i + ",线程ID:" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);
            });
            watch2.Stop();
            Console.WriteLine(watch2.Elapsed);
        }
        public static void Run2()
        {
            var watch = Stopwatch.StartNew();

            watch.Start();

            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            Parallel.For(0, 10, (i, state) =>
            {

                if (i == 7) { state.Break(); return; }
                Console.WriteLine(i);
                //if (bag.Count == 500)
                //{
                //    state.Break();
                //    return;
                //}
                //bag.Add(i);
            });

            Console.WriteLine("当前集合有{0}个元素。", bag.Count);

        }
    }
}
