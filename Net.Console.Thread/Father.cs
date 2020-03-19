using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    public class Father
    {
        private int field_private
        {
            get;set;
        }

        // 子类
        protected int field_protected
        {
            get; set;
        }

        // 同一包
        internal int field_intenal
        {
            get; set;
        }

        // 同一包 或者 子类
        internal protected int field_internal_protected
        {
            get; set;
        }

        // 同一包 & 子类
        private protected int field_private_protected 
        {
            get; set;
        }
    }
}
