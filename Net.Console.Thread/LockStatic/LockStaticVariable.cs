using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.LockStatic
{
    class LockStaticVariable
    {
        //lock 是在对象实例上加上互斥锁，但对象实例依旧可以访问+修改
        //lock的对象实例修改后就失去lock的作用了
       
      
        public void Run(object _v)
        {
            // A 线程先执行，B线程到lock等待，A线程修改v的值，B线程可以继续执行
            lock (v)
            {
                v = _v;
                Console.WriteLine("enter:" + DateTime.Now.ToString() + " " + v.GetHashCode());
                Thread.Sleep(1000);
                Console.WriteLine("exit:" + DateTime.Now.ToString());
            }

            //A 线程先执行，B线程到修改v的值
            //注意 string 变量 在内存中只有一份


            //v = _v;
            //lock (v)
            //{
            //    Console.WriteLine("enter:" + DateTime.Now.ToString() + "_" + v.GetHashCode());
            //    Thread.Sleep(1000);
            //    Console.WriteLine("exit:" + DateTime.Now.ToString());
            //}

        }

        private  static object v = new Program();
        
    }
}
