namespace PingAlarm.Network
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

        public List<PingHost> Hosts { get; set; }
        public int MinimumFailures { get; } = 2;
        public int Sleep { get; } = 2000;
        public int Timeout { get; }
    }
}