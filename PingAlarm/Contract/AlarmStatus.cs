namespace PingAlarm.Contract
{
    public class AlarmStatus
    {
        public bool Enabled { get; set; }

        public DateTimeOffset Changed { get; set; }

        public List<PingHostStatus> PingHostStatus { get; set;}

        public List<GpioInputPinStatus> GpioInputPinStatus { get; set; }


    }
}
