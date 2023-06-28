using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PingAlarm.TwillioAlarm
{
    public class TwillioAlarm
    {
        private readonly ILogger<TwillioAlarm> _log;
        private readonly TwillioConfig _twillioConfig;

        public TwillioAlarm(
            TwillioConfig twillioConfig,
            ILogger<TwillioAlarm> log
            )
        {
            _twillioConfig = twillioConfig;
            _log = log;
        }

        public async Task Alarm(string name)
        {
            if (!_twillioConfig.Enabled)
            {
                _log.LogWarning("Twillio is not enabled in appsettings.json");
                return;
            }

            var twiml = GenerateTwiml(name);

            TwilioClient.Init(
                _twillioConfig.AccountSid,
                _twillioConfig.AuthToken);

            foreach (var recepient in _twillioConfig.Recepients)
            {
                await Call(recepient, twiml);
            }
        }

        private async Task Call(string phoneNumber, string twiml)
        {
            var CallTo = new Twilio.Types.PhoneNumber(phoneNumber);
            var CallFrom = new Twilio.Types.PhoneNumber(_twillioConfig.PhoneNumber);

            await CallResource.CreateAsync(
                to: CallTo,
                from: CallFrom,
                twiml: twiml,
                timeLimit: 20
            );

            _log.LogInformation("Called {phoneNumber}", phoneNumber);
        }

        private string GenerateTwiml(string name)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<Response>\r\n" +
                $"    <Say language=\"{_twillioConfig.Language}\">" +
                $"      ALarm {name}" +
                $"    </Say>\r\n" +
                "</Response>";
        }
    }
}