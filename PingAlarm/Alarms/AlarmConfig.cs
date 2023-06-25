using Microsoft.Extensions.Configuration;
using PingAlarm.Monitor;
using Twilio.Types;

namespace PingAlarm.Alarms
{
    public class AlarmConfig
    {
        public AlarmConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("Alarm");
            Password = config.GetValue<string>("Password");
        }

        public string Password { get; } 
    }
}