using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm
{
    internal class Host
    {
        public string IPNumber { get; set; } = string.Empty;

        public bool HasFailed { get; set; } = false;
    }
}
