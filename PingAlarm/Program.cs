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
        services.AddSingleton<GpioGuardConfig>();
        services.AddSingleton<GpioStatusConfig>();
        services.AddSingleton<TwillioConfig>();

        services.AddSingleton<TwillioAlarm>();
        services.AddSingleton<GpioStatus>();
        services.AddSingleton<Alarm>();

        services.AddHostedService<PingWorker>();
        services.AddHostedService<GpioGuardWorker>();
    })
    .Build();

await host.RunAsync();
