using Microsoft.AspNetCore;
using NLog.Extensions.Logging;
using NLog.Web;
using PingAlarm.Alarms;
using PingAlarm.Monitor;
using System.Reflection.PortableExecutable;


var builder = WebApplication.CreateBuilder(args);

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<GpioGuardConfig>();
builder.Services.AddHostedService<GpioGuardWorker>();

builder.Services.AddSingleton<GpioStatusConfig>();
builder.Services.AddSingleton<GpioStatus>();

builder.Services.AddSingleton<TwillioConfig>();
builder.Services.AddSingleton<TwillioAlarm>();

builder.Services.AddSingleton<AlarmConfig>();
builder.Services.AddSingleton<Alarm>();

builder.Services.AddSingleton<PingConfig>();
builder.Services.AddHostedService<PingWorker>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();



app.Run();






var host = WebHost.CreateDefaultBuilder(args)
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
