﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using JamJunction.App.Events;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;
using JamJunction.App.Slash_Commands.Other_Commands;
using Lavalink4NET;
using Microsoft.Extensions.Hosting;

namespace JamJunction.App;

internal sealed class Bot : BackgroundService
{
    public static readonly Dictionary<ulong, GuildData> GuildData = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordClient _discordClient;
    private readonly IAudioService _audioService;

    public Bot(IServiceProvider serviceProvider, DiscordClient discordClient, IAudioService audioService)
    {
        _serviceProvider = serviceProvider;
        _discordClient = discordClient;
        _audioService = audioService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _discordClient.ConnectAsync();
        
        var trackStartedEvent = new TrackStartedEvent(_discordClient, _audioService);
        _audioService.TrackStarted += trackStartedEvent.TrackStarted;
        
        var trackEndedEvent = new TrackEndedEvent(_discordClient, _audioService);
        _audioService.TrackEnded += trackEndedEvent.TrackEnded;
        
        ConfigSlashCommands();
        ButtonEvents();
    }
    
    private void ConfigSlashCommands()
    {
        var slashCommands =
            _discordClient.UseSlashCommands(new SlashCommandsConfiguration {Services = _serviceProvider});

        slashCommands.RegisterCommands<PingCommand>();
        slashCommands.RegisterCommands<CaptionCommand>();
        slashCommands.RegisterCommands<CurrentSongCommand>();
        slashCommands.RegisterCommands<PlayCommand>();
        slashCommands.RegisterCommands<PauseCommand>();
        slashCommands.RegisterCommands<ResumeCommand>();
        slashCommands.RegisterCommands<StopCommand>();
        slashCommands.RegisterCommands<VolumeCommand>();
        slashCommands.RegisterCommands<SeekCommand>();
        slashCommands.RegisterCommands<RestartCommand>();
        slashCommands.RegisterCommands<LeaveCommand>();
        slashCommands.RegisterCommands<ViewQueueCommand>();
        slashCommands.RegisterCommands<ShuffleQueueCommand>();
        slashCommands.RegisterCommands<SkipCommand>();
        slashCommands.RegisterCommands<HelpCommand>();
        slashCommands.RegisterCommands<PositionCommand>();
    }

    private void ButtonEvents()
    {
        var buttonHandler = new ButtonHandler();

        _discordClient.ComponentInteractionCreated += async (sender, args) =>
        {
            await buttonHandler.Execute(new PauseButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new ResumeButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new SkipButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new StopButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new ShuffleButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new VolumeDownButton(), sender, args);
            await buttonHandler.Execute(new VolumeUpButton(), sender, args);
            await buttonHandler.Execute(new ViewQueueButton(), sender, args);
            await buttonHandler.Execute(new RestartButton(), sender, args);
        };
    }
}