namespace PingAlarm.Contract
{
    public class AlarmStatus
    {
        public DateTimeOffset Changed { get; set; }
        public bool Enabled { get; set; }
        public List<GpioInputPinStatus> GpioInputPinStatus { get; set; } = new List<GpioInputPinStatus>();
        public List<PingHostStatus> PingHostStatus { get; set; } = new List<PingHostStatus>();

        public bool GpioEnabled { get; set; }

        public bool PingEnabled { get; set; }
    }
}