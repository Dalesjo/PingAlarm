namespace PingAlarm.Contract
{
    public class AlarmStatus
    {
        public DateTimeOffset Changed { get; set; }
        public bool Enabled { get; set; }
        public List<GpioInputPinStatus> GpioInputPinStatus { get; set; }
        public List<PingHostStatus> PingHostStatus { get; set; }
    }
}