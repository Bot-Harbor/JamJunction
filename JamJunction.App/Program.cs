using DSharpPlus;
using JamJunction.App;
using JamJunction.App.Lavalink;
using JamJunction.App.Quartz_Scheduler;
using JamJunction.App.Secrets;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

var builder = new HostApplicationBuilder();
builder.Services.AddHostedService<Bot>();
builder.Services.AddSingleton<DiscordClient>();

builder.Services.AddSingleton(new DiscordConfiguration
{
    TokenType = TokenType.Bot,
    Token = DiscordSecrets.BotToken,
    Intents = DiscordIntents.All,
    MinimumLogLevel = LogLevel.Information,
    AutoReconnect = true
});

builder.Services.ConfigureLavalink(config =>
{
    config.BaseAddress = new Uri($"http://{LavalinkSecrets.HostName}:{LavalinkSecrets.Port}");
    config.Passphrase = LavalinkSecrets.Password;
    config.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(15));
    config.ReadyTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddLavalink();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    var jobKey = new JobKey("YoutubeCipherJob");
    q.AddJob<YoutubeCipherJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("YoutubeCipherJob")
        .WithCronSchedule("00 30 06 ? * *"));
});

builder.Services.AddQuartzHostedService();

builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Information));

builder.Build().Run();