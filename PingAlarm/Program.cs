using NLog.Extensions.Logging;
using PingAlarm;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<PingConfig>();
        services.AddHostedService<PingWorker>();
    })
    .Build();

await host.RunAsync();
