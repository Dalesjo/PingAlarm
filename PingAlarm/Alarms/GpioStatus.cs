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

        private bool AlarmStarted { get; set; } = false;

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

            _gpioController.OpenPin(_gpioConfig.Alarm.Pin, PinMode.Output);
            _gpioController.OpenPin(_gpioConfig.NetworkStatus.Pin, PinMode.Output);
            _gpioController.OpenPin(_gpioConfig.GuardStatus.Pin,PinMode.Output);
        }

        public async Task Alarm(CancellationToken cancellationToken)
        {
            if (!_gpioConfig.Enabled)
            {
                return;
            }

            if(AlarmStarted)
            {
                _log.LogInformation("Alarm is already started, aborting");
                return;
            }


            var on = _gpioConfig.Alarm.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.Alarm.High ? PinValue.Low : PinValue.High;

            AlarmStarted = true;
            _gpioController.Write(_gpioConfig.Alarm.Pin, on);
            _log.LogDebug("Alarm Started");
            _log.LogDebug("Alarm will run for {AlarmTime}ms",_gpioConfig.AlarmTime);
            
            await Task.Delay(_gpioConfig.AlarmTime, cancellationToken);
            
            _gpioController.Write(_gpioConfig.Alarm.Pin, off);
            _log.LogDebug("Alarm Stopped");
            AlarmStarted = false;
        }

        public void NetworkBlink()
        {
            if (!_gpioConfig.Enabled)
            {
                return;
            }

            var on = _gpioConfig.NetworkStatus.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.NetworkStatus.High ? PinValue.Low : PinValue.High;
            var set = NetworkBlinky ? on : off;
            _log.LogDebug("NetworkBlinky Blink: {set}", set);

            _gpioController.Write(_gpioConfig.NetworkStatus.Pin, set);
            NetworkBlinky = !NetworkBlinky;
        }

        public void GuardBlink()
        {
            if (!_gpioConfig.Enabled)
            {
                return;
            }

            var on = _gpioConfig.GuardStatus.High ? PinValue.High : PinValue.Low;
            var off = _gpioConfig.GuardStatus.High ? PinValue.Low : PinValue.High;
            var set = GuardBlinky ? on : off;
            _log.LogDebug("Guard Blink: {set}", set);

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
