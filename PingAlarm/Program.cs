using PingAlarm;

IHost host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<PingWorker>();
    })
    .Build();

await host.RunAsync();
