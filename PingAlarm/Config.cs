using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm
{
    internal class Config
    {
        public int Timeout { get; set; }

        public List<Host> Hosts { get; set; }
    }
}
