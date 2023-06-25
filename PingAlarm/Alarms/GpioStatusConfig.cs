using Microsoft.Extensions.Configuration;
using PingAlarm.Monitor;
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

            
            Alarm = section.GetSection("Alarm").Get<GpioOutputPin>();
            
            AlarmTime = section.GetValue<int>("AlarmTime");
            Enabled = section.GetValue<bool>("Enabled");

            NetworkStatus = section.GetSection("NetworkStatus").Get<GpioOutputPin>();
            GuardStatus = section.GetSection("GuardStatus").Get<GpioOutputPin>();
        }


        public int AlarmTime { get; set; }
        public bool Enabled { get; set;  }
        public GpioOutputPin Alarm { get; set; }
        public GpioOutputPin NetworkStatus { get; set; }
        public GpioOutputPin GuardStatus { get; set; }
    }

}
