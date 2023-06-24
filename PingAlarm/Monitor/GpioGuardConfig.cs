using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace PingAlarm.Monitor
{
    public class GpioGuardConfig
    {

        public GpioGuardConfig(IConfiguration configuration) {

            var section = configuration.GetSection("GpioGuard");

            Guards = section.GetSection("Guards").Get<List<GpioInputPin>>();
            Enabled = section.GetValue<bool>("Enabled");
            Sleep = section.GetValue<int>("Sleep");
        }

        public List<GpioInputPin> Guards { get; }

        public bool Enabled { get; }

        public int Sleep { get; }

    }
}
