using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class TaskFactorySample
    {
        public void One()
        {
            
            var del = new Func<string, char, int>(doo);

            del.BeginInvoke("a", 'b', callback, del);

            Console.ReadKey();
        }
        static void callback(IAsyncResult ar)
        {

            var del = (Func<string, char, int>)ar.AsyncState;
            Console.WriteLine("callback调用");
            var res = del.EndInvoke(ar);
            Console.WriteLine("doo返回值：{0}", res);

        }

        static int doo(string a, char b)
        {
            Console.WriteLine("doo调用：{0} {1}", a, b);
            return 1;

        }
    }
}
