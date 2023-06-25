using NLog.Fluent;
using PingAlarm.Alarms;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Jwt.AccessToken;

namespace PingAlarm.Monitor
{
    internal class GpioGuardWorker : BackgroundService
    {

        private readonly ILogger<GpioGuardWorker> _log;
        private GpioGuardConfig _gpioconfig;
        private GpioController _gpioController;
        private GpioStatus _gpioStatus;
        private Alarm _alarm;

        public GpioGuardWorker(
            GpioGuardConfig gpioConfig,
            ILogger<GpioGuardWorker> log,
            GpioStatus gpioStatus,
            Alarm alarm
            )
        {
            _gpioconfig = gpioConfig;
            _gpioStatus = gpioStatus;
            _log = log;
            _alarm = alarm;

            if (!_gpioconfig.Enabled)
            {
                return;
            }

            _gpioController = new GpioController();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!_gpioconfig.Enabled)
            {
                _log.LogDebug("gpioconfig not enabled...");
                return;
            }

            foreach (var pin in _gpioconfig.Guards)
            {
                RegistratePin(pin);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _gpioStatus.GuardBlink();

                foreach (var pin in _gpioconfig.Guards)
                {
                    await checkPin(pin, cancellationToken);
                }
                
                await Task.Delay(_gpioconfig.Sleep, cancellationToken);
            }

            foreach (var pin in _gpioconfig.Guards)
            {
                UnregisterPin(pin);
            }
        }

        private async Task checkPin(GpioInputPin gpioPin, CancellationToken cancellationToken)
        {
            var state  = _gpioController.Read(gpioPin.Pin);
            var closed = gpioPin.High ? PinValue.High : PinValue.Low;

            if(state != closed)
            {
                return;
            }

            await Task.Delay(gpioPin.Verify, cancellationToken);
            var verifiedState = _gpioController.Read(gpioPin.Pin);

            if(state == verifiedState)
            {
                await _alarm.Start(gpioPin.Name, cancellationToken);
            }
        }

        private void RegistratePin(GpioInputPin gpioPin)
        {
            _gpioController.OpenPin(
                gpioPin.Pin,
                gpioPin.PullUp ? PinMode.InputPullUp : PinMode.InputPullUp);

             var result = _gpioController.Read(gpioPin.Pin);
            //result == PinValue.High;

            _log.LogDebug("Registrated {gpioPin}", gpioPin.Pin);

        }

        private void UnregisterPin(GpioInputPin gpioPin)
        {

            _gpioController.ClosePin(gpioPin.Pin);
        }
    }
}
