using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Monitor
{
    public class GpioInputPin
    {
        public string Name { get; } = string.Empty;

        public int Pin { get; }

        public bool PullUp { get; }

        public bool High { get; }

        public int Verify { get; }

    }
}
