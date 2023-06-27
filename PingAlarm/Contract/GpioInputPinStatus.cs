namespace PingAlarm.Contract
{
    public class GpioInputPinStatus
    {
        public int Failures { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }
}