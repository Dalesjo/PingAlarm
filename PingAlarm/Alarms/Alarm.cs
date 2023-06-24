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
        private bool active { get; set; }

        public Alarm(
            TwillioAlarm twillioAlarm,
            GpioStatus gpioStatus,
            ILogger<Alarm> log) 
        { 

            _twillioAlarm = twillioAlarm;
            _gpioStatus = gpioStatus;
            _log = log;
        }

        public async Task Start(string name,CancellationToken cancellationToken)
        {
            if(active)
            {
                _log.LogDebug("Alarm already started, will not start again. Aborting.");
            }

            active = true;
            var twillio =  _twillioAlarm.Alarm(name);
            var gpio = _gpioStatus.Alarm(cancellationToken);

            await Task.WhenAll(twillio,gpio);
            active = false;
        }
    }
}
