using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    /*
     * 
     * 信号量
     * 只控制线程数量不控制现货
     * 
     * 
     * Semaphore sema=new Semaphore(x,y);
     * 初始有x的空余位置，总位置有y个
     * 
     * WaitOne() 等待
     * Release(i) 释放i个资源 
     **/ 
    class SemaphoreDemo
    {
    }
}
