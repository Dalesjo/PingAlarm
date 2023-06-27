namespace PingAlarm.Gpio
{
    public class GpioOutputPin
    {
        public bool High { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Pin { get; set; }
    }
}