using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Alarms
{
    public class GpioOutputPin
    {
        public string Name { get; set; } = string.Empty;

        public int Pin { get; set; }

        public bool High { get; set; }

    }
}
