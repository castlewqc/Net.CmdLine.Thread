using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.Exception
{
    class Obj
    {
       public  Ticket ticket { get;set;}
       public  string str { get; set; }
        public int i = 0;

        public override string ToString()
        {
            return str??"";
        }
    }
    class Test
    {
        public static Obj obj = new Obj { ticket = new Ticket(),str ="old_obj"};

       
        public void Run(object i)
        {
            Console.WriteLine(string.Format("{0} -> {1}.Start", DateTime.Now.ToString("HH:mm:ss"), Thread.CurrentThread.Name));
            var str = i;
            lock (obj)
            {
                Console.WriteLine(string.Format("{0} -> {1}进入锁定区域.", DateTime.Now.ToString("HH:mm:ss"), Thread.CurrentThread.Name));
                Thread.Sleep(5000);
                obj.i++;
                Console.WriteLine("obj i "+i);
                Console.WriteLine(string.Format("地址:{0},参数:{1}", obj.GetHashCode(), obj.ToString()));
                Console.WriteLine(string.Format("地址:{0},参数:{1}",str.GetHashCode(),str.ToString()));
                Console.WriteLine(string.Format("{0} -> {1}退出锁定区域.", DateTime.Now.ToString("HH:mm:ss"), Thread.CurrentThread.Name));
            }
        }

        public static  void Runstatic(object i)
        {
            Console.WriteLine(string.Format("{0} -> {1}.Start", DateTime.Now.ToString("HH:mm:ss"), i.ToString()));
            var str = i;
            lock (obj)
            {
                Console.WriteLine(string.Format("{0} -> {1}进入锁定区域.", DateTime.Now.ToString("HH:mm:ss"), i.ToString()));
                obj.i++;
                Thread.Sleep(3000);
                Console.WriteLine(string.Format("地址:{0},参数:{1}", obj.GetHashCode(), obj.ToString()));
                Console.WriteLine(string.Format("地址:{0},参数:{1}_{2}", str.GetHashCode(), str.ToString(),obj.i));
                Console.WriteLine(string.Format("{0} -> {1}退出锁定区域.", DateTime.Now.ToString("HH:mm:ss"), i.ToString()));
            }
        }
        public void Lock()
        {
            lock(obj)
            {
                Thread.Sleep(5000);
            }
        }

        public void Modify()
        {
            Console.WriteLine("Modify before "+obj.GetHashCode());
            //obj = "....";
            Console.WriteLine("Modify after " + obj.GetHashCode());
        }
    }
}
