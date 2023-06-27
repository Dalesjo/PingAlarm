namespace PingAlarm.TwillioAlarm
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

        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public bool Enabled { get; set; }
        public string Language { get; set; } = "sv-SE";
        public string PhoneNumber { get; set; }
        public List<string> Recepients { get; set; }
    }
}