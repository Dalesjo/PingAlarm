using PingAlarm.Gpio;
using PingAlarm.Network;
using System.Diagnostics;

namespace PingAlarm.Alarm
{
    internal class AlarmWorker : BackgroundService
    {
        private readonly AlarmConfig _alarmConfig;
        private readonly GpioGuardConfig _gpioconfig;
        private readonly ILogger<GpioGuardWorker> _log;

        private readonly PingConfig _pingConfig;
        private GpioStatus _gpioStatus;
        private Stopwatch _stopwatch = new Stopwatch();
        private TwillioAlarm.TwillioAlarm _twillioAlarm;
        private bool Active;
        private string activeAlarm;
        public AlarmWorker(
            ILogger<GpioGuardWorker> log,
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
                await Task.Delay(1000);

                if (Active && !_alarmConfig.Enabled)
                {
                    _log.LogInformation("Alarm is disabled", activeAlarm);
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
            var failedPings = _pingConfig.Hosts.Any(h => h.Failures > 0) && _pingConfig.Enabled;
            var failedGpios = _gpioconfig.Guards.Any(h => h.Failures > 0) && _gpioconfig.Enabled;

            return failedPings || failedGpios;
        }

        private async Task Cooldown(CancellationToken cancellationToken)
        {
            _log.LogInformation("Alarm stopped after running {running}ms, alarm will not activate again before {cooldown}ms has passed", _stopwatch.Elapsed.TotalMilliseconds, _alarmConfig.Cooldown);
            StopAlarm();
            await Task.Delay(_alarmConfig.Cooldown, cancellationToken);
        }
        private string getAlarms()
        {
            var failedPings = _pingConfig.Hosts.Where(h => h.Failures > 0).Select(h => h.Name);
            var failedGpios = _gpioconfig.Guards.Where(h => h.Failures > 0).Select(h => h.Name);

            return string.Join(",", failedPings) + "," + string.Join(",", failedGpios);
        }

        private async Task StartAlarm()
        {
            var alarms = getAlarms();
            _log.LogInformation("Alarm activated for {alarms}", alarms);

            if (!Active)
            {
                _stopwatch.Restart();
                await _twillioAlarm.Alarm(alarms);
            }

            Active = true;
            _gpioStatus.AlarmOn();
        }

        private void StopAlarm()
        {
            Active = false;
            _stopwatch.Reset();

            _gpioStatus.AlarmOff();
        }
    }
}