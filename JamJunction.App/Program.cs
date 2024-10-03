﻿using DSharpPlus;
using JamJunction.App;
using JamJunction.App.Secrets;
using Lavalink4NET;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostApplicationBuilder();
builder.Services.AddHostedService<Bot>();
builder.Services.AddSingleton<DiscordClient>();

builder.Services.AddSingleton(new DiscordConfiguration
{
    TokenType = TokenType.Bot,
    Token = DiscordSecrets.BotToken,
    Intents = DiscordIntents.All,
    MinimumLogLevel = LogLevel.Debug,
    AutoReconnect = true,
});

builder.Services.ConfigureLavalink(config =>
{
    config.BaseAddress = new Uri($"http://{DiscordSecrets.HostName}:{DiscordSecrets.Port}");
    config.WebSocketUri = new Uri($"ws://{DiscordSecrets.HostName}:{DiscordSecrets.Port}/v4/websocket");
    config.Passphrase = DiscordSecrets.Password;
    config.ResumptionOptions = new LavalinkSessionResumptionOptions(TimeSpan.FromSeconds(15));
    config.ReadyTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddLavalink();

// Change to information when done
builder.Services.AddLogging(s => s.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Build().Run();