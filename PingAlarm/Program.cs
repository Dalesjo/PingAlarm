using NLog.Extensions.Logging;
using PingAlarm.Alarms;
using PingAlarm.Monitor;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<PingConfig>();
        services.AddSingleton<TwillioConfig>();
        services.AddSingleton<TwillioAlarm>();
        services.AddHostedService<PingWorker>();
    })
    .Build();

await host.RunAsync();
