namespace PingAlarm.Network
{
    public class PingHost
    {
        public int Failures { get; set; } = 0;
        public string IPNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}