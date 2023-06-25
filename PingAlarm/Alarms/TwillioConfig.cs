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

        public bool Enabled { get; set; }
        public string AccountSid { get; set; } 

        public string AuthToken { get; set; }

        public string PhoneNumber { get; set; }
        public string Language { get; set; } = "sv-SE";

        public List<string> Recepients { get; set; }
    }
}