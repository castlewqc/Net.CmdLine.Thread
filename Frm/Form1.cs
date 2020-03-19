using Net.CmdLine.ThreadStudy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
        }
        static void Print(object objToken)
        {
            Thread.Sleep(5000);
            CancellationToken token = (CancellationToken)objToken;
            Console.WriteLine("Print , 开始等待{0}...", System.Threading.Thread.CurrentThread.ManagedThreadId);
            //if (token.WaitHandle.WaitOne())
            //{
            //    Console.WriteLine("直到 调用 Cancel() 执行此处 ");
            //}
        }
        private async void button1_Click(object sender, EventArgs e)
        {


            var ttt = await AsyncAwaitSample.Method05();
            Console.WriteLine("something1");
            Console.WriteLine("something2" + ttt);
            Console.WriteLine("something3");
            Console.ReadKey();
            //一种多线程取消任务开关对象
            CancellationTokenSource s1 = new CancellationTokenSource();
            CancellationTokenSource s2 = new CancellationTokenSource();

            //s1 , 或者 s2 取消，导致 s3 取消
            CancellationTokenSource s3 = CancellationTokenSource.CreateLinkedTokenSource(s1.Token, s2.Token);

            //异步执行结束 回调
            s1.Token.Register(new Action(() => {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            s2.Token.Register(new Action(() =>
            {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            s3.Token.Register(new Action(() =>
            {
                Console.WriteLine("线程{0} 执行回调!", System.Threading.Thread.CurrentThread.ManagedThreadId);
            }));

            //异步执行
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s1.Token);  //Token 中包含回调信息，执行结束 触发 Register 关联方法。
                                                                              // System.Threading.Thread.Sleep(2000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s2.Token);
            //  System.Threading.Thread.Sleep(2000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Print), s3.Token);



            Console.WriteLine();
            s2.CancelAfter(3000); //若各线程传递各自tocken ,执行回调线程：  s2 , s3
                                  //s1.CancelAfter(3000); //若各线程传递各自tocken ,执行回调线程:  s1 , s3

            //s3.Cancel(); //若各线程传递各自tocken ,只触发 s3 , 不会触发 s1 , s2 回调。

            //注意: 若各线程传递s3.token ,  s1 , s2 任意Cancle , s1 , s2 , s3 均会回调。

            Console.ReadKey();
            //var name = GetName();
            //Console.WriteLine("主线程end");
            //Console.WriteLine(await name);
            //Console.WriteLine("Current Thread Id :{0}", Thread.CurrentThread.ManagedThreadId);

            //Do2();


            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());
            //await Task.Run(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString()));
            //await Task.Run(() => MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString()));
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString());

        }
        static async Task Test()
        {
            // 方法打上async关键字，就可以用await调用同样打上async的方法
            // await 后面的方法将在另外一个线程中执行
            await GetName();
        }

        static async Task<string> GetName()
        {
            // Delay 方法来自于.net 4.5
            await Task.Delay(1000);  // 返回值前面加 async 之后，方法里面就可以用await了
            Console.WriteLine("Current Thread Id :{0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("In antoher thread.....");
            return "Jack";
        }

        public static async void Do2()
        {
            Console.WriteLine("step1,线程ID:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            await AsyncSleep();

            Console.WriteLine("step5,线程ID:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public static async Task AsyncSleep()
        {
            Console.WriteLine("step2,线程ID:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            await Sleep(10);

            Console.WriteLine("step4,线程ID:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private static async Task Sleep(int second)
        {
            Console.WriteLine("step3,线程ID:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
           
            //Thread.Sleep(second * 300);
        }

        static void AsyncAPM()
        {
            Console.WriteLine("Main thread ID={0}", Thread.CurrentThread.ManagedThreadId);

            byte[] s_data = new byte[100];
            FileStream fs = new FileStream(@"d:\1.txt", FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous);
            fs.BeginRead(s_data, 0, s_data.Length, ReadIsDone, fs);

            Console.WriteLine("主线程执行完毕");

            Console.ReadLine();
        }

        private static void ReadIsDone(IAsyncResult ar)
        {
            Thread.Sleep(5000);
            Console.WriteLine("ReadIsDone thread ID={0}", Thread.CurrentThread.ManagedThreadId);

            FileStream fs = (FileStream)ar.AsyncState;
            int bytesRead = fs.EndRead(ar);
            fs.Close();

            Console.WriteLine("Number of bytes read={0}", bytesRead);

        }
        public Task<string> MyAsync()
        {
            var t = new Task<string>((str) =>
            {
                var dt = DateTime.Now;
                Thread.Sleep(4000);

                return String.Format("({0}){1} - {2}",
                    Thread.CurrentThread.ManagedThreadId, dt, DateTime.Now);
            }, null);

            t.Start();

            return t;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // error => Application.Exit(Environment.ExitCode);
            Environment.Exit(Environment.ExitCode);
        }
    }
}
