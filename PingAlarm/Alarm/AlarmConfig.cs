namespace PingAlarm.Alarm
{
    public class AlarmConfig
    {
        public AlarmConfig(IConfiguration configuration)
        {
            var section = configuration.GetSection("Alarm");
            Password = section.GetValue<string>("Password");
            Enabled = section.GetValue<bool>("Enabled");
            Cooldown = section.GetValue<int>("Cooldown");
            AlarmTime = section.GetValue<int>("AlarmTime");
        }

        public string Active { get; set; } = string.Empty;
        public int AlarmTime { get; set; }

        public DateTimeOffset Changed { get; private set; } = DateTimeOffset.Now;
        public int Cooldown { get; set; }
        public bool Enabled { get; private set; }
        public string Password { get; set; }
        public bool Set(bool onOff, string password)
        {
            if (password != Password)
            {
                return false;
            }

            Enabled = onOff;
            Changed = DateTimeOffset.Now;

            return true;
        }
    }
}