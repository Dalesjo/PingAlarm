using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Alarms
{
    public class GpioStatusConfig
    {
        public GpioStatusConfig(IConfiguration configuration)
        {

            var section = configuration.GetSection("GpioStatus");

            
            Alarm = section.GetValue<GpioOutputPin>("Alarm");
            AlarmTime = section.GetValue<int>("AlarmTime");
            Enabled = section.GetValue<bool>("Enabled");
            NetworkStatus = section.GetValue<GpioOutputPin>("NetworkStatus");
            GuardStatus = section.GetValue<GpioOutputPin>("GuardStatus");
        }


        public int AlarmTime { get; }
        public bool Enabled { get; }
        public GpioOutputPin Alarm {get;}
        public GpioOutputPin NetworkStatus { get; }
        public GpioOutputPin GuardStatus { get; }
    }

}
