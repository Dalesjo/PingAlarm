using Microsoft.Extensions.Configuration;
using PingAlarm.Monitor;
using Twilio.Types;

namespace PingAlarm.Alarms
{
    public class TwillioConfig
    {
        public TwillioConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("Twillio");
            Enabled = config.GetValue<bool>("Enabled");
            AccountSid = config.GetValue<string>("AccountSid");
            AuthToken = config.GetValue<string>("AuthToken");
            PhoneNumber = config.GetValue<string>("PhoneNumber");
            Language = config.GetValue<string>("Language");
            Recepients = config.GetSection("Recepients").Get<List<string>>();
        }

        public bool Enabled { get; }
        public string AccountSid { get; } 

        public string AuthToken { get; }

        public string PhoneNumber { get; }
        public string Language { get; } = "sv-SE";

        public List<string> Recepients { get; }
    }
}