using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    public class AsyncAwaitSample
    {
        //返回空
        public async static void Method01()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            await Task.Run(() => { Thread.Sleep(1000); });
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
        }

        //返回可执行的任务
        public async static Task Method02()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            await Task.Run(() => { Thread.Sleep(1000); });
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
        }
        //返回带返回值的可执行任务
        public async static Task<int> Method03()
        {
            Console.WriteLine("Method03:" + Thread.CurrentThread.ManagedThreadId.ToString());
            int rlt = await Task.Run<int>(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("TaskRun:" + Thread.CurrentThread.ManagedThreadId.ToString());
                return 1;
            }).ConfigureAwait(false);
            /**
             * ConfigureAwait(true) ConfigureAwait(false)
             * 一个是在异步执行时捕获上下文，一个是在异步执行时不捕获上下文.
1.当ConfigureAwait(true)，代码由同步执行进入异步执行时，
            当前同步执行的线程上下文信息（比如HttpConext.Current，Thread.CurrentThread.CurrentCulture）
            就会被捕获并保存至SynchronizationContext中，供异步执行中使用，并且供异步执行完成之后（await之后的代码）
            的同步执行中使用（虽然await之后是同步执行的，但是发生了线程切换，会在另外一个线程中执行「ASP.NET场景」）。
            这个捕获当然是有代价的，当时我们误以为性能问题是这个地方的开销引起，但实际上这个开销很小，在我们的应用场景不至于会带来性能问题。
2.当Configurewait(flase)，则不进行线程上下文信息的捕获，async方法中与await之后的代码执行时就无法获取await之前的线程的上下文信息，
            在ASP.NET中最直接的影响就是HttpConext.Current的值为null。
             */
            Console.WriteLine("Method03:" + Thread.CurrentThread.ManagedThreadId.ToString());
            return rlt;
        }
        public async static Task<int> Method04()
        {
            int sum = 0;
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            sum += await Task.Run<int>(() => { Thread.Sleep(1000); return 1; });
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            sum += await Task.Run<int>(() => { Thread.Sleep(1000); return 1; });
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            sum += await Task.Run<int>(() => { Thread.Sleep(1000); return 1; });
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            return sum;
        }

        public async static Task<int> Method05()
        {
            Console.WriteLine("method05 start");
            Console.WriteLine("Method05:"+Thread.CurrentThread.ManagedThreadId.ToString());
            int  i = await Method03().ConfigureAwait(false);
            Console.WriteLine("Method05:" + Thread.CurrentThread.ManagedThreadId.ToString());
            Console.WriteLine("method05 end");
            return i;
        }
        /*
         *
         * demo: 
         *  var ttt = AsyncAwaitSample.Method05();
            Console.WriteLine("something1");
            Console.WriteLine("something2" + ttt.Result);
            Console.WriteLine("something3");
            Console.ReadKey();

         * output:
         * method05 start
Method05:10
Method03:10
something1
TaskRun:6
Method03:6
Method05:6
method05 end
something21
something3
         * 
         * +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
         *  var ttt = AsyncAwaitSample.Method04();
            Console.WriteLine("something1");
            Console.WriteLine("something2" + ttt.Result);
            Console.WriteLine("something3");
            Console.ReadKey();
         * 
         * 9
something1
10
11
10
something23
something3
         * 
         * 注意点：
         * winform wpf 和 console 是不一样的
console await 后面的代码默认是新开线程执行的
而 winform wpf await后面的代码是在原本的线程中执行的
         * 
         */

    }
}
