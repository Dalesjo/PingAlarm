using Microsoft.Extensions.Hosting;
using PingAlarm.Alarms;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingAlarm.Monitor
{
    public class PingWorker : BackgroundService
    {
        private readonly ILogger<PingWorker> _log;

        private bool AlarmSent = false;
        private PingConfig _pingConfig;
        private TwillioAlarm _twillioAlarm;

        public PingWorker(
            PingConfig pingConfig,
            ILogger<PingWorker> log,
            TwillioAlarm twillioAlarm)
        {
            _pingConfig = pingConfig;
            _log = log;
            _twillioAlarm = twillioAlarm;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _log.LogDebug("Ping All");
                    await PingAllHosts();
                    await VerifyAllHosts();
                    await Task.Delay(_pingConfig.Sleep, stoppingToken);
                } catch(Exception ex)
                {
                    _log.LogError(ex, "Pinghost Failed");
                }
            }
        }

        private async Task VerifyAllHosts()
        {
            var failed = AnyFailedHosts();
            var failedHosts = GetFailedHosts();

            if (failed == true && AlarmSent == false)
            {
                _log.LogInformation("ALARM!!!!!");
                
                await _twillioAlarm.Alarm(failedHosts);

                AlarmSent = true;
                return;
            }

            if (failed == false && AlarmSent == true)
            {
                _log.LogInformation("ALARM SILENT");
                AlarmSent = false;
                return;
            }
        }

        private bool AnyFailedHosts()
        {
            var alarm = _pingConfig.Hosts.Any(h => h.Failures >= _pingConfig.MinimumFailures);
            var failed = _pingConfig.Hosts.Where(h => h.Failures >= _pingConfig.MinimumFailures);

            foreach (var host in failed)
            {
                _log.LogInformation("Host Alarm: {IPNumber}", host.IPNumber);
            }

            return alarm;
        }

        private string GetFailedHosts()
        {
            var failed = _pingConfig.Hosts.Where(h => h.Failures >= _pingConfig.MinimumFailures).Select(h => h.Name);

            return String.Join(',', failed);
        }

        private async Task PingAllHosts()
        {

            var allHosts = new List<Task>();
            foreach (var host in _pingConfig.Hosts)
            {
                var check = PingHost(host);
                allHosts.Add(check);
            }

            await Task.WhenAll(allHosts);
        }

        private async Task PingHost(PingHost host)
        {
            var ping = new Ping();
            var reply = await ping.SendPingAsync(host.IPNumber, _pingConfig.Timeout);

            if (reply.Status == IPStatus.Success)
            {
                if (host.Failures > 0)
                {
                    _log.LogDebug("Ping Successfull for {IPNumber}", host.IPNumber);
                }

                host.Failures = 0;
                return;
            }

            host.Failures++;
            _log.LogDebug("Ping Failed for the {Failures}th time for {IPNumber}", host.Failures, host.IPNumber);
        }
    }
}