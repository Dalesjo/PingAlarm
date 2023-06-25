using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Monitor
{
    public class GpioInputPin
    {
        public string Name { get; set; } = string.Empty;

        public int Pin { get; set; }

        public bool PullUp { get; set; }

        public bool High { get; set; }

        public int Verify { get; set; } = 0;

        public int Failures { get; set; } = 0;

    }
}
