namespace PingAlarm.Contract
{
    public class PingHostStatus
    {
        public string Name { get; set; } = string.Empty;

        public int Failures { get; set; } = 0;
    }
}
