using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    // lockthis -> 同一线程互斥，不同线程不互斥
    //互斥同一String引用 不同线程互斥
    // 注意： 不能锁值类型，会发生装箱 故不会互斥同步
    class LockDemo
    {
        public void LockThis()
        {

            lock (this)
            {
                Thread.Sleep(2000);
                Console.WriteLine(DateTime.Now);
            }
        }

        public void LockClass()
        {

            lock (this.GetType())
            {
                Thread.Sleep(2000);
                Console.WriteLine(DateTime.Now);
            }
        }

        private String lockString = "lockString";
        public void LockString()
        {

            lock (String.Intern(MethodBase.GetCurrentMethod().ToString()))
            {
                Thread.Sleep(2000);
                Console.WriteLine(DateTime.Now);
            }
        }

        private static Object staticObj = new Object();

        public void LockStaticObj()
        {

            lock (staticObj)
            {
                Thread.Sleep(2000);
                Console.WriteLine(DateTime.Now);
            }
        }

        // 在同一线程中执行 不会死锁
        // 当一个互斥锁已被占用时，在同一线程中执行的代码仍可以获取和释放该锁。
        // 但是，在其他线程中执行的代码在该锁被释放前是无法获得它的。
        public void Test(int num)
        {
            if (num > 10)
            {
                lock (this) //线程获取/等待锁
                {
                    num--;
                    Console.WriteLine("current thread:{0},num is:{1}", Thread.CurrentThread.ManagedThreadId, num);
                    Test(num);
                }
            }
        }
    }
}
