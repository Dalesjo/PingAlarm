using PingAlarm.Monitor;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Alarms
{
    public class GpioStatus
    {
        private readonly ILogger<GpioStatus> _log;
        private readonly GpioStatusConfig _gpioConfig;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private GpioController _gpioController;


        private bool NetworkBlinky { get; set; } = true;
        private bool GuardBlinky { get; set; } = true;

        public GpioStatus(
            GpioStatusConfig gpioConfig,
            ILogger<GpioStatus> log,
            IHostApplicationLifetime applicationLifetime)
        {
            _gpioConfig = gpioConfig;
            _log = log;
            _applicationLifetime = applicationLifetime;

            if (!_gpioConfig.Enabled)
            {
                _log.LogWarning("GpioStatus is not enabled in appsettings.json");
                return;
            }

            _gpioController = new GpioController();
            applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);

            _gpioController.OpenPin(_gpioConfig.Alarm.Pin);
            _gpioController.OpenPin(_gpioConfig.NetworkStatus.Pin);
            _gpioController.OpenPin(_gpioConfig.GuardStatus.Pin);
        }

        public async Task Alarm(CancellationToken cancellationToken)
        {
            if (!_gpioConfig.Enabled)
            {
                _log.LogWarning("GpioStatus is not enabled in appsettings.json");
                return;
            }

            var on = _gpioConfig.GuardStatus.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.GuardStatus.High ? PinValue.Low : PinValue.High;

            _gpioController.Write(_gpioConfig.GuardStatus.Pin, on);
            await Task.Delay(_gpioConfig.AlarmTime, cancellationToken);
            _gpioController.Write(_gpioConfig.GuardStatus.Pin, off);
        }

        public void NetworkBlink()
        {
            if (!_gpioConfig.Enabled)
            {
                _log.LogWarning("GpioStatus is not enabled in appsettings.json");
                return;
            }

            var on = _gpioConfig.NetworkStatus.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.NetworkStatus.High ? PinValue.Low : PinValue.High;
            var set = NetworkBlinky ? on : off;

            _gpioController.Write(_gpioConfig.NetworkStatus.Pin, set);
            NetworkBlinky = !NetworkBlinky;
        }

        public void GuardBlink()
        {
            if (!_gpioConfig.Enabled)
            {
                _log.LogWarning("GpioStatus is not enabled in appsettings.json");
                return;
            }

            var on = _gpioConfig.GuardStatus.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.GuardStatus.High ? PinValue.Low : PinValue.High;
            var set = GuardBlinky ? on : off;

            _gpioController.Write(_gpioConfig.GuardStatus.Pin, set);
            GuardBlinky = !GuardBlinky;
        }

        private void OnApplicationStopping()
        {
            _gpioController.ClosePin(_gpioConfig.Alarm.Pin);
            _gpioController.ClosePin(_gpioConfig.NetworkStatus.Pin);
            _gpioController.ClosePin(_gpioConfig.GuardStatus.Pin);
        }
    }
}
