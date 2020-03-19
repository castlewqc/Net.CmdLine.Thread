using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy
{
    class Child : Father
    {

        public void Do()
        {

            this.field_private_protected = 2;
            this.field_intenal = 2;
            this.field_internal_protected = 2;
            this.field_protected = 2;

            Father cls1 = new Father();
            cls1.field_intenal = 1;
            cls1.field_internal_protected = 2;
        }

    }
}
