using PingAlarm.Gpio;
using System.Net.NetworkInformation;

namespace PingAlarm.Network
{
    public class PingWorker : BackgroundService
    {
        private readonly ILogger<PingWorker> _log;

        private readonly GpioStatus _gpioStatus;
        private readonly PingConfig _pingConfig;
        public PingWorker(
            PingConfig pingConfig,
            ILogger<PingWorker> log,
            GpioStatus gpioStatus)
        {
            _pingConfig = pingConfig;
            _gpioStatus = gpioStatus;
            _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _log.LogDebug("Ping All");
                    _gpioStatus.NetworkBlink();
                    await PingAllHosts();
                    await Task.Delay(_pingConfig.Sleep, stoppingToken);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Pinghost Failed");
                }
            }
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