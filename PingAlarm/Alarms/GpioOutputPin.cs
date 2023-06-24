using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Alarms
{
    public class GpioOutputPin
    {
        public string Name { get; } = string.Empty;

        public int Pin { get; }

        public bool High { get; }

    }
}
