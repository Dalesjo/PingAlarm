using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PingAlarm.Alarms;
using PingAlarm.Contract;

namespace PingAlarm.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly ILogger<AlarmController> _log;
        private readonly Alarm _alarm;

        public AlarmController(
            Alarm alarm,
            ILogger<AlarmController> log
            )
        {
            _log = log;
            _alarm = alarm;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Set(AlarmSet alarmSet)
        {

            var successfull = _alarm.Set(alarmSet.Enabled, alarmSet.Password);

            var result = new AlarmStatus()
            {
                Enabled = _alarm.Enabled,
                Changed = _alarm.Changed
            };

            if (successfull)
            {
                var state = _alarm.Enabled ? "Enabled" : "Disabled";
                _log.LogInformation("Alarm is {state}",state);

                return Ok(result);
            }

            _log.LogInformation("Alarm could not be set, wrong password.");
            return Unauthorized(result);
        }
    }
}
