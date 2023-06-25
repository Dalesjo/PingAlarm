namespace PingAlarm.Contract
{
    public class AlarmStatus
    {
        public bool Enabled { get; set; }

        public DateTimeOffset Changed { get; set; }
    }
}
