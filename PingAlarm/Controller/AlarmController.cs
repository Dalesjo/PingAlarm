using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PingAlarm.Alarms;
using PingAlarm.Contract;
using PingAlarm.Monitor;

namespace PingAlarm.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly ILogger<AlarmController> _log;
        private readonly Alarm _alarm;

        private GpioGuardConfig _gpioconfig;
        private PingConfig _pingConfig;

        public AlarmController(
            Alarm alarm,
            ILogger<AlarmController> log,
            PingConfig pingConfig,
            GpioGuardConfig gpioConfig
            )
        {
            _log = log;
            _alarm = alarm;

            _gpioconfig = gpioConfig;
            _pingConfig = pingConfig;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Set(AlarmSet alarmSet)
        {

            var successfull = _alarm.Set(alarmSet.Enabled, alarmSet.Password);

            var result = getAlarmStatus();

            if (successfull)
            {
                var state = _alarm.Enabled ? "Enabled" : "Disabled";
                _log.LogInformation("Alarm is {state}",state);

                return Ok(result);
            }

            _log.LogInformation("Alarm could not be set, wrong password.");
            return Unauthorized(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var result = getAlarmStatus();
            return Ok(result);
        }

        private AlarmStatus getAlarmStatus()
        {
            var result = new AlarmStatus()
            {
                Enabled = _alarm.Enabled,
                Changed = _alarm.Changed
            };

            result.PingHostStatus = _pingConfig.Hosts
                .OrderBy(host => host.Name)
                .Select(host => new PingHostStatus
            {
                Name = host.Name,
                Failures = host.Failures
            }).ToList();

            result.GpioInputPinStatus = _gpioconfig.Guards
                .OrderBy(host => host.Name)
                .Select(host => new GpioInputPinStatus()
                {
                    Name = host.Name,
                    Failures= host.Failures
                }).ToList();


            return result;
        }
    }
}
