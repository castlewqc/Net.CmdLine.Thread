using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.Exception
{
    class TicketRun
    {
        public void Run()
        {
            var ticket = new Ticket();
            for (int i = 1; i <= 3; i++)
                new Thread(ticket.Sold).Start();
            Console.Read();
        }
    }
}
