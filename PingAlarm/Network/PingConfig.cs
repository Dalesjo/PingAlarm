namespace PingAlarm.Network
{
    public class PingConfig
    {
        public PingConfig(IConfiguration configuration)
        {
            var section = configuration.GetSection("Ping");
            Enabled = section.GetValue<bool>("Enabled");
            Timeout = section.GetValue<int>("Timeout");
            Sleep = section.GetValue<int>("Sleep");
            Hosts = section.GetSection("Hosts").Get<List<PingHost>>();
        }

        public bool Enabled { get; set; } 
        public List<PingHost> Hosts { get; set; }
        public int MinimumFailures { get; } = 2;
        public int Sleep { get; } = 2000;
        public int Timeout { get; }
    }
}