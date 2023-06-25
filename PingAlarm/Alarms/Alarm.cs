using PingAlarm.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingAlarm.Alarms
{
    public class Alarm
    {
        private readonly ILogger<Alarm> _log;
        private TwillioAlarm _twillioAlarm;
        private GpioStatus _gpioStatus;
        private readonly AlarmConfig _alarmConfig;
        private bool Active { get; set; }

        public bool Enabled { get; private set; } = false;

        public DateTimeOffset Changed { get; private set; } = DateTimeOffset.Now;

        public Alarm(
            TwillioAlarm twillioAlarm,
            AlarmConfig alarmConfig,
            GpioStatus gpioStatus,
            ILogger<Alarm> log) 
        {
            _alarmConfig = alarmConfig;
            _twillioAlarm = twillioAlarm;
            _gpioStatus = gpioStatus;
            _log = log;
        }

        public async Task Start(string name,CancellationToken cancellationToken)
        {
            if(Active)
            {
                _log.LogDebug("Alarm already started, will not start again. Aborting.");
                return;
            }

            if(!Enabled)
            {
                _log.LogDebug("Alarm is not enabled...");
                return;
            }

            Active = true;
            var twillio =  _twillioAlarm.Alarm(name);
            var gpio = _gpioStatus.Alarm(cancellationToken);

            await Task.WhenAll(twillio,gpio);
            Active = false;
        }

        public bool Set(bool onOff, string password)
        {
            if(password != _alarmConfig.Password)
            {
                return false;
            }

            Enabled = onOff;
            Changed = DateTimeOffset.Now;
            return true;
        }
    }
}
