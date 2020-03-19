using System;
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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.Load += Form3_Load;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            /**
             * 即使窗口关闭，线程依旧在运行，无论是debug还是执行exe
             * 
             */ 
            Thread thread = new Thread(() => {
                while (true)
                {
                    StreamWriter sw = File.AppendText("file.txt");
                    sw.WriteLine(DateTime.Now.ToString());
                    Thread.Sleep(1000);
                    sw.Close();
                }
            });
            thread.Start();
        }
    }
}
