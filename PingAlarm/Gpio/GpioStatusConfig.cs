namespace PingAlarm.Gpio
{
    public class GpioStatusConfig
    {
        public GpioStatusConfig(IConfiguration configuration)
        {
            var section = configuration.GetSection("GpioStatus");

            Alarm = section.GetSection("Alarm").Get<GpioOutputPin>();

            Enabled = section.GetValue<bool>("Enabled");

            NetworkStatus = section.GetSection("NetworkStatus").Get<GpioOutputPin>();
            GuardStatus = section.GetSection("GuardStatus").Get<GpioOutputPin>();
        }

        public GpioOutputPin Alarm { get; set; }
        public bool Enabled { get; set; }
        public GpioOutputPin GuardStatus { get; set; }
        public GpioOutputPin NetworkStatus { get; set; }
    }
}