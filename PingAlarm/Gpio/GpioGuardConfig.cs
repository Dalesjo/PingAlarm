namespace PingAlarm.Gpio
{
    public class GpioGuardConfig
    {
        public GpioGuardConfig(IConfiguration configuration)
        {
            var section = configuration.GetSection("GpioGuard");

            Enabled = section.GetValue<bool>("Enabled");
            Sleep = section.GetValue<int>("Sleep");
            Guards = section.GetSection("Guards").Get<List<GpioInputPin>>();
        }

        public bool Enabled { get; }
        public List<GpioInputPin> Guards { get; }
        public int Sleep { get; }
    }
}