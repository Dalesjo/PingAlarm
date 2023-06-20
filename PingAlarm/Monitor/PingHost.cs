using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Monitor
{
    public class PingHost
    {
        public string Name { get; set; } = string.Empty;

        public string IPNumber { get; set; } = string.Empty;

        public int Failures { get; set; } = 0;
    }
}
