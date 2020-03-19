using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     *  [Synchronization(SynchronizationAttribute.REQUIRED, true)]
     *       [MethodImpl(MethodImplOptions.Synchronized)]
     *       二选一
     */
    [Synchronization(SynchronizationAttribute.REQUIRED, true)]
    public class SynchronizationAttributeClass : System.ContextBoundObject
    {
        private static int _balance;
        public int Blance
        {
            get
            {
                return _balance;
            }
        }

        public SynchronizationAttributeClass()
        {
            _balance = 1000;
        }

        //MethodImplAttribute--使整个方法上锁，直到方法返回，才释放锁
        /*
         * 锁住方法不是静态的   MethodImplAttribute 相当于 方法加上lock(this)
         * 锁住方法是静态的   MethodImplAttribute 相当于 方法加上lock(typeof(SynchronizationAttributeClass))
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WithDraw(string name, object money)
        {
            if ((int)money <= _balance)
            {
                Thread.Sleep(2000);
                _balance = _balance - (int)money;
                Console.WriteLine("{0} 取钱成功！余额={1}", name, _balance);
            }
            else
            {
                Console.WriteLine("{0} 取钱失败！余额不足！", name);
            }
        }
    }
}
