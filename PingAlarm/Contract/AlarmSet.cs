namespace PingAlarm.Contract
{
    public class AlarmSet
    {
        public bool Enabled { get; set; }
        public string Password { get; set; } = string.Empty;

        public bool GpioEnabled { get; set; }

        public bool PingEnabled { get; set; }
    }
}