using System.Device.Gpio;

namespace PingAlarm.Gpio
{
    public class GpioStatus
    {
        private readonly GpioStatusConfig _gpioConfig;
        private readonly ILogger<GpioStatus> _log;
        private readonly GpioController? _gpioController;

        public GpioStatus(
            GpioStatusConfig gpioConfig,
            ILogger<GpioStatus> log,
            IHostApplicationLifetime applicationLifetime)
        {
            _gpioConfig = gpioConfig;
            _log = log;

            if (!_gpioConfig.Enabled)
            {
                _log.LogWarning("GpioStatus is not enabled in appsettings.json");
                return;
            }

            _gpioController = new GpioController();
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);

            _gpioController.OpenPin(_gpioConfig.Alarm.Pin, PinMode.Output);
            _gpioController.OpenPin(_gpioConfig.NetworkStatus.Pin, PinMode.Output);
            _gpioController.OpenPin(_gpioConfig.GuardStatus.Pin, PinMode.Output);

            TurnOffAllPins();
        }

        private bool GuardBlinky { get; set; } = true;
        private bool NetworkBlinky { get; set; } = true;

        public void AlarmOff()
        {
            if (_gpioController == null)
            {
                return;
            }

            var off = GetPinValue(_gpioConfig.Alarm, false);
            _gpioController.Write(_gpioConfig.Alarm.Pin, off);

            _log.LogDebug("Alarm Started");
        }

        public void AlarmOn()
        {
            if (_gpioController == null)
            {
                return;
            }

            var on = GetPinValue(_gpioConfig.Alarm, true);
            _gpioController.Write(_gpioConfig.Alarm.Pin, on);

            _log.LogDebug("Alarm Started");
        }

        public void GuardBlink()
        {
            if (_gpioController == null)
            {
                return;
            }

            var set = GetPinValue(_gpioConfig.GuardStatus, GuardBlinky);
            _log.LogDebug("Guard Blink: {set}", set);

            _gpioController.Write(_gpioConfig.GuardStatus.Pin, set);
            GuardBlinky = !GuardBlinky;
        }

        public void NetworkBlink()
        {
            if (_gpioController == null)
            {
                return;
            }

            var set = GetPinValue(_gpioConfig.NetworkStatus, NetworkBlinky);
            _log.LogDebug("NetworkBlinky Blink: {set}", set);

            _gpioController.Write(_gpioConfig.NetworkStatus.Pin, set);
            NetworkBlinky = !NetworkBlinky;
        }

        public void TurnOffAllPins()
        {
            if (_gpioController == null)
            {
                return;
            }

            _log.LogDebug("Disabled all GPIO outputs.");

            var AlarmOff = GetPinValue(_gpioConfig.Alarm, false);
            _gpioController.Write(_gpioConfig.Alarm.Pin, AlarmOff);

            var networkOff = GetPinValue(_gpioConfig.NetworkStatus, false);
            _gpioController.Write(_gpioConfig.NetworkStatus.Pin, networkOff);

            var GuardOff = GetPinValue(_gpioConfig.GuardStatus, false);
            _gpioController.Write(_gpioConfig.GuardStatus.Pin, GuardOff);
        }

        private static PinValue GetPinValue(GpioOutputPin gpioOutputPin, bool onOff)
        {
            var on = gpioOutputPin.High ? PinValue.High : PinValue.Low;
            var off = gpioOutputPin.High ? PinValue.Low : PinValue.High;
            return onOff ? on : off;
        }

        private void OnApplicationStopping()
        {
            if(_gpioController == null)
            {
                return;
            }
            
            _log.LogInformation("Applications topping, turning off all LEDs.");
            TurnOffAllPins();
            _gpioController.ClosePin(_gpioConfig.Alarm.Pin);
            _gpioController.ClosePin(_gpioConfig.NetworkStatus.Pin);
            _gpioController.ClosePin(_gpioConfig.GuardStatus.Pin);
        }
    }
}