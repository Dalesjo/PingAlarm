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

            
            Enabled = section.GetValue<bool>("Enabled");
            Sleep = section.GetValue<int>("Sleep");
            Guards = section.GetSection("Guards").Get<List<GpioInputPin>>();
        }

        public List<GpioInputPin> Guards { get; }

        public bool Enabled { get; }

        public int Sleep { get; }

    }
}
