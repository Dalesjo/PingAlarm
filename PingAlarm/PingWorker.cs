using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingAlarm
{
    public class PingWorker : BackgroundService
    {
        private readonly ILogger<PingWorker> _logger;

        public PingWorker(ILogger<PingWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var host = new Host()
            {
                IPNumber = "192.168.52.132",
                HasFailed = false
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);


                var ping = createPing(host);

                var result = await ping;

                if(result.Status == IPStatus.Success)
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("failure");
                }


                await Task.Delay(1000, stoppingToken);
            }
        }

        private Task<PingReply> createPing(Host host)
        {
            var timeout = 2000;
            var ping = new Ping();
            var reply = ping.SendPingAsync(host.IPNumber,timeout);

            return reply;
        }
    }
}