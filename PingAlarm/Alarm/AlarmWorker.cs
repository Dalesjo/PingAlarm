using PingAlarm.Gpio;
using PingAlarm.Network;
using System.Diagnostics;

namespace PingAlarm.Alarm
{
    internal class AlarmWorker : BackgroundService
    {
        private readonly AlarmConfig _alarmConfig;
        private readonly GpioGuardConfig _gpioconfig;
        private readonly ILogger<AlarmWorker> _log;

        private readonly PingConfig _pingConfig;
        private readonly GpioStatus _gpioStatus;
        private readonly Stopwatch _stopwatch = new();
        private readonly TwillioAlarm.TwillioAlarm _twillioAlarm;
        private bool Active;
        public AlarmWorker(
            ILogger<AlarmWorker> log,
            AlarmConfig alarmConfig,
            GpioGuardConfig gpioConfig,
            PingConfig pingConfig,
            GpioStatus gpioStatus,
            TwillioAlarm.TwillioAlarm twillioAlarm
            )
        {
            _alarmConfig = alarmConfig;
            _gpioconfig = gpioConfig;
            _pingConfig = pingConfig;
            _gpioStatus = gpioStatus;
            _log = log;
            _twillioAlarm = twillioAlarm;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);

                if (Active && !_alarmConfig.Enabled)
                {
                    _log.LogInformation("Alarm was disabled, turning of alarms.");
                    StopAlarm();
                    continue;
                }

                if (Active && _stopwatch.Elapsed.TotalMilliseconds > _alarmConfig.AlarmTime)
                {
                    await Cooldown(cancellationToken);
                    continue;
                }

                var isAlarming = AnyAlarms();
                if (_alarmConfig.Enabled && isAlarming)
                {
                    /* Start alarm */
                    await StartAlarm();
                    continue;
                }

            }
        }

        private bool AnyAlarms()
        {
            var failedPings = _pingConfig.Hosts.Any(h => h.Failures >= h.MinimumFailures) && _pingConfig.Enabled;
            var failedGpios = _gpioconfig.Guards.Any(h => h.Failures >= h.MinimumFailures) && _gpioconfig.Enabled;

            return failedPings || failedGpios;
        }

        private async Task Cooldown(CancellationToken cancellationToken)
        {
            _log.LogInformation("Alarm stopped after running {running}ms, alarm will not activate again before {cooldown}ms has passed", _stopwatch.Elapsed.TotalMilliseconds, _alarmConfig.Cooldown);
            StopAlarm();
            await Task.Delay(_alarmConfig.Cooldown, cancellationToken);
        }
        private string GetAlarms()
        {
            var failedPings = _pingConfig.Hosts.Where(h => h.Failures >= h.MinimumFailures).Select(h => h.Name);
            var failedGpios = _gpioconfig.Guards.Where(h => h.Failures >= h.MinimumFailures).Select(h => h.Name);

            return string.Join(",", failedPings) + "," + string.Join(",", failedGpios);
        }

        private async Task StartAlarm()
        {
            var alarms = GetAlarms();
            _log.LogInformation("Alarm activated for {alarms}", alarms);

            _gpioStatus.AlarmOn();

            if (!Active)
            {
                _stopwatch.Restart();
                await _twillioAlarm.Alarm(alarms);
            }

            Active = true;
            
        }

        private void StopAlarm()
        {
            Active = false;
            _stopwatch.Reset();

            _gpioStatus.AlarmOff();
        }
    }
}