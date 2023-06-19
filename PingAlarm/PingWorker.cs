using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingAlarm
{
    public class PingWorker : BackgroundService
    {
        private readonly ILogger<PingWorker> _log;

        private PingConfig _pingConfig;

        public PingWorker(
            PingConfig pingConfig,
            ILogger<PingWorker> log)
        {
            _pingConfig = pingConfig;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _log.LogDebug("Ping All");
                await PingAllHosts();
                await VerifyAllHosts();
                await Task.Delay(_pingConfig.Sleep, stoppingToken);
            }
        }

        private async Task VerifyAllHosts()
        {
            var alarm = CheckForFailedHosts();

            if (alarm == true && _pingConfig.AlarmSent == false)
            {
                _log.LogInformation("ALARM!!!!!");
                _pingConfig.AlarmSent = true;
                return;
            }

            if (alarm == false && _pingConfig.AlarmSent == true)
            {
                _log.LogInformation("ALARM SILENT");
                _pingConfig.AlarmSent = false;
                return;
            }
        }

        private bool CheckForFailedHosts()
        {
            var alarm = _pingConfig.Hosts.Any(h => h.Failures >= _pingConfig.MinimumFailures);
            var failed = _pingConfig.Hosts.Where(h => h.Failures >= _pingConfig.MinimumFailures);

            foreach (var host in failed)
            {
                _log.LogInformation("Host Alarm: {IPNumber}", host.IPNumber);
            }

            return alarm;
        }

        private async Task PingAllHosts()
        {

            var allHosts = new List<Task>();
            foreach(var host in _pingConfig.Hosts)
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

            if(reply.Status == IPStatus.Success)
            {
                if(host.Failures > 0)
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