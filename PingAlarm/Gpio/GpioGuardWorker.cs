using Microsoft.Extensions.Hosting;
using System.Device.Gpio;

namespace PingAlarm.Gpio
{
    internal class GpioGuardWorker : BackgroundService
    {
        private readonly ILogger<GpioGuardWorker> _log;
        private readonly GpioGuardConfig _gpioconfig;
        private readonly GpioController _gpioController;
        private readonly GpioStatus _gpioStatus;

        public GpioGuardWorker(
            GpioGuardConfig gpioConfig,
            ILogger<GpioGuardWorker> log,
            GpioStatus gpioStatus
            )
        {
            _gpioconfig = gpioConfig;
            _gpioStatus = gpioStatus;
            _log = log;

            _gpioController = new GpioController();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var pin in _gpioconfig.Guards)
            {
                OpenPin(pin);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _gpioStatus.GuardBlink();

                foreach (var pin in _gpioconfig.Guards)
                {
                    await CheckPin(pin, cancellationToken);
                }

                await Task.Delay(_gpioconfig.Sleep, cancellationToken);
            }

            foreach (var pin in _gpioconfig.Guards)
            {
                ClosePin(pin);
            }
        }

        private async Task CheckPin(GpioInputPin gpioPin, CancellationToken cancellationToken)
        {
            var state = _gpioController.Read(gpioPin.Pin);
            var closed = gpioPin.High ? PinValue.High : PinValue.Low;

            if (state != closed)
            {
                gpioPin.Failures = 0;
                return;
            }

            await Task.Delay(gpioPin.Verify, cancellationToken);
            var verifiedState = _gpioController.Read(gpioPin.Pin);

            if (state == verifiedState)
            {
                _log.LogDebug("Alarm failed for the {Failures}th time for GPIO {gpioPin}", gpioPin.Failures, gpioPin.Pin);
                gpioPin.Failures++;
            }
        }

        private void ClosePin(GpioInputPin gpioPin)
        {
            _gpioController.ClosePin(gpioPin.Pin);
        }

        private void OpenPin(GpioInputPin gpioPin)
        {
            _gpioController.OpenPin(
                gpioPin.Pin,
                gpioPin.PullUp ? PinMode.InputPullUp : PinMode.InputPullUp);

            _log.LogDebug("Registrated {gpioPin}", gpioPin.Pin);
        }
    }
}