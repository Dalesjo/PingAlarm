using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Monitor
{
    public class PingConfig
    {

        public PingConfig(IConfiguration configuration)
        {

            var config = configuration.GetSection("Ping");
            Timeout = config.GetValue<int>("Timeout");
            Sleep = config.GetValue<int>("Sleep");
            Hosts = config.GetSection("Hosts").Get<List<PingHost>>();
        }

        public int Timeout { get; }

        public int Sleep { get; } = 2000;

        public int MinimumFailures { get; } = 2;

        public List<PingHost> Hosts { get; set; }

    }
}
