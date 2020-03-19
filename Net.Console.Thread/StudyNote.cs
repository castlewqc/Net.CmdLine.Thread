using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class StudyNote
    {
        /*
         * 
         * Thread 代价高 ThreadPool 线程都是异步的，没有返回值和无法判断什么时候完成
         * 线程同步方式
         * 
         * 1. 维护自由锁(InterLocked)实现同步
         *
         * 2. 监视器（Monitor）和互斥锁（lock）
         *
         * 3. 读写锁（ReadWriteLock)
         *
         * 4. 系统内核对象
         *
         *  1) 互斥(Mutex), 信号量(Semaphore), 事件(AutoResetEvent/ManualResetEvent)
         *
         *  2) 线程池
         * 
         * 
         * 
         * 
         * WaitHandle
         * 其子类 
         * Metux  与Monitor一样是线程相关的，拥有获得锁的线程才能释放
         * EventWaitHandler 及其子类 AutoResetEvent ManualResetEvent 线程无关的
         * 方法
         * 动态方法 WaitOne()
         * 静态方法 SignalAndWait(WaitHandle,WaitHandle)
         * 静态方法 WaitAll(WaitHandle[]) , WaitAny(WaitHandle[])
         * 
         * 
         * 
         * exclusive 排外的
         * 
         * 
         * 原子操作
         * Interlocked.Increment 方法：让++成为原子操作；Interlocked.Decrement 方法让--成为原子操作。
         * 
         **/

        /*
         * Join  用来阻塞调用线程，直到某个线程终止或经过了指定时间为止
         * 
         * 
         * public void Join();
         * public bool Join(int millisecondsTimeout);
         * public bool Join(TimeSpan timeout);
         * 
         * 
         * 线程启动后才可以Join
         * A线程中B.Join() B线程执行完成A才会继续执行
         * 
         * 
         * 
         * 答案就在委托 的BeginInvoke() 方法上, BeginInvoke() 也是(异步)启动一个新线程. 例如
MyDelegate dele = new MyDelegate (MyFunction);
dele.BeginInvoke(10,"abcd");
void MyFunction(int count, string str);
可以实现参数的传递.

如何收集线程函数 的 返回值?

与 BeginInvoke 对应 有个 EndInvoke 方法,而且运行完毕返回 IAsyncResult 类型的返回值.
这样我们可以这样收集 线程函数的 返回值

MyDelegate dele = new MyDelegate (MyFunction);
IAsyncResult ref = dele.BeginInvoke(10,"abcd");
...
int result = dele.EndInvoke(ref); <----收集 返回值
int MyFunction(int count, string str); <----带参数和返回值的 线程函数
         **/

        public enum ThreadPriority
        {
            // 摘要: 
            //     可以将 System.Threading.Thread 安排在具有任何其他优先级的线程之后。
            Lowest = 0,
            //
            // 摘要: 
            //     可以将 System.Threading.Thread 安排在具有 Normal 优先级的线程之后，在具有 Lowest 优先级的线程之前。
            BelowNormal = 1,
            //
            // 摘要: 
            //     可以将 System.Threading.Thread 安排在具有 AboveNormal 优先级的线程之后，在具有 BelowNormal 优先级的线程之前。
            //     默认情况下，线程具有 Normal 优先级。
            Normal = 2,
            //
            // 摘要: 
            //     可以将 System.Threading.Thread 安排在具有 Highest 优先级的线程之后，在具有 Normal 优先级的线程之前。
            AboveNormal = 3,
            //
            // 摘要: 
            //     可以将 System.Threading.Thread 安排在具有任何其他优先级的线程之前。
            Highest = 4,
        }
    }
}
