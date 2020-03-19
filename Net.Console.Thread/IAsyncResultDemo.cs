using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    public delegate void Hello(string name);
    class IAsyncResultDemo
    {
        public void Run()
        {
            int i = 0;
            Hello hello = OnShow;
            AsyncCallback callback = Callback;
            IAsyncResult ar = hello.BeginInvoke("abc", callback, "state");
            //state 是callback 的参数IAsyncResult的AsyncState
            while (true)
            {
                if (i >= 3) break;
                Console.WriteLine(++i);
                Thread.Sleep(2000);
            }
            hello.EndInvoke(ar);
            Console.Read();

        }
        private static void OnShow(string name)
        {
            Console.WriteLine("OnShow Thread ID " + Thread.CurrentThread.ManagedThreadId.ToString());
            Thread.Sleep(3000);
            Console.WriteLine("->" + name);
        }
        private static void Callback(IAsyncResult ar)
        {
            Console.WriteLine("Callback Thread ID " + Thread.CurrentThread.ManagedThreadId.ToString());
            object o = ar.AsyncState;
            Console.WriteLine("Call back  " + o);
        }
    }
}
