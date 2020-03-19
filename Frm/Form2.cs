using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frm
{
    
    public partial class Form2 : Form
    {

        static class StaticLazz
        {
            //int a;
            //不能在静态类中声明实例变量 一定要加上static
            public static void Add(int x, int y)
            {
            }
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(()=> {
                while (true)
                {
                    Debug.WriteLine(DateTime.Now);
                    Thread.Sleep(1000);
                }
            }));
            thread.Start();
            
        }

        public class Parent
        {
            public virtual void Do()
            {
                Console.WriteLine("parent");
            }
        }

        public class Child : Parent
        {
            public override void Do()
            {
                base.Do();
                Console.WriteLine("child");
            }
        }
    }
}
