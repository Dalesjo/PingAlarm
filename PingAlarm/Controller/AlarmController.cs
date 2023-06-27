using Microsoft.AspNetCore.Mvc;
using PingAlarm.Alarm;
using PingAlarm.Contract;
using PingAlarm.Gpio;
using PingAlarm.Network;

namespace PingAlarm.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly AlarmConfig _alarmConfig;
        private readonly ILogger<AlarmController> _log;
        private GpioGuardConfig _gpioconfig;
        private PingConfig _pingConfig;

        public AlarmController(
            AlarmConfig alarmConfig,
            ILogger<AlarmController> log,
            PingConfig pingConfig,
            GpioGuardConfig gpioConfig
            )
        {
            _log = log;
            _alarmConfig = alarmConfig;

            _gpioconfig = gpioConfig;
            _pingConfig = pingConfig;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var result = getAlarmStatus();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Set(AlarmSet alarmSet)
        {
            var successfull = _alarmConfig.Set(alarmSet.Enabled, alarmSet.Password);

            

            if (successfull)
            {
                _gpioconfig.Enabled = alarmSet.GpioEnabled;
                _pingConfig.Enabled = alarmSet.PingEnabled;
                
                var state = _alarmConfig.Enabled ? "Enabled" : "Disabled";
                _log.LogInformation("Alarm is {state}", state);

                var result = getAlarmStatus();
                return Ok(result);
            }

            _log.LogInformation("Alarm could not be set, wrong password.");
            var defaultResult = getAlarmStatus();
            return Unauthorized(defaultResult);
        }
        private AlarmStatus getAlarmStatus()
        {
            var result = new AlarmStatus()
            {
                Enabled = _alarmConfig.Enabled,
                Changed = _alarmConfig.Changed,
                GpioEnabled = _gpioconfig.Enabled,
                PingEnabled = _pingConfig.Enabled,
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
                    Failures = host.Failures
                }).ToList();

            return result;
        }
    }
}