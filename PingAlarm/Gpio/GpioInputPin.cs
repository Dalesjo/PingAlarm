namespace PingAlarm.Gpio
{
    public class GpioInputPin
    {
        public int Failures { get; set; } = 0;
        public bool High { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Pin { get; set; }

        public bool PullUp { get; set; }
        public int Verify { get; set; } = 0;
    }
}