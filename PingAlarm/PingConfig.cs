using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm
{
    public class PingConfig
    {

        public PingConfig(IConfiguration configuration) {

            var config = configuration.GetSection("Config");
            Timeout = config.GetValue<int>("Timeout");
            Sleep = config.GetValue<int>("Sleep");
            Hosts = config.GetSection("Hosts").Get<List<PingHost>>();
        }

        public int Timeout { get; set; }

        public int Sleep { get; set; } = 2000;

        public int MinimumFailures { get; set; } = 2;

        public bool AlarmSent { get; set; } = false;   



        public List<PingHost> Hosts { get; set; }

    }
}
