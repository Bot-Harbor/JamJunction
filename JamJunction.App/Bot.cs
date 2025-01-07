using DSharpPlus;
using DSharpPlus.SlashCommands;
using JamJunction.App.Events;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Events.Menus;
using JamJunction.App.Lavalink;
using JamJunction.App.Slash_Commands.Music_Commands;
using JamJunction.App.Slash_Commands.Other_Commands;
using Lavalink4NET;
using Microsoft.Extensions.Hosting;

namespace JamJunction.App;

internal sealed class Bot : BackgroundService
{
    public static readonly Dictionary<ulong, GuildData> GuildData = new();
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;
    private readonly IServiceProvider _serviceProvider;

    public Bot(IServiceProvider serviceProvider, DiscordClient discordClient, IAudioService audioService)
    {
        _serviceProvider = serviceProvider;
        _discordClient = discordClient;
        _audioService = audioService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _discordClient.ConnectAsync();

        SlashCommands();
        ButtonEvents();
        MenuEvents();

        var trackStartedEvent = new TrackStartedEvent(_discordClient, _audioService);
        _audioService.TrackStarted += trackStartedEvent.TrackStarted;

        var trackEndedEvent = new TrackEndedEvent(_discordClient, _audioService);
        _audioService.TrackEnded += trackEndedEvent.TrackEnded;

        var trackStuckEvent = new TrackStuckEvent(_discordClient, _audioService);
        _audioService.TrackStuck += trackStuckEvent.TrackStuck;

        var connectionClosedEvent = new ConnectionClosedEvent();
        _audioService.ConnectionClosed += connectionClosedEvent.ConnectionClosed;

        var playerDestroyedEvent = new PlayerDestroyedEvent();
        _audioService.Players.PlayerDestroyed += playerDestroyedEvent.PlayerDestroyed;

        var voiceStateUpdatedEvent = new VoiceStateUpdatedEvent(_audioService);
        _discordClient.VoiceStateUpdated += voiceStateUpdatedEvent.VoiceStateUpdated;
    }

    private void SlashCommands()
    {
        var slashCommands =
            _discordClient.UseSlashCommands(new SlashCommandsConfiguration {Services = _serviceProvider});

        slashCommands.RegisterCommands<PingCommand>();
        slashCommands.RegisterCommands<CaptionCommand>();
        slashCommands.RegisterCommands<CurrentTrackCommand>();
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
        slashCommands.RegisterCommands<RepeatCommand>();
        slashCommands.RegisterCommands<FiltersCommand>();
        slashCommands.RegisterCommands<SkipToCommand>();
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
            await buttonHandler.Execute(new VolumeDownButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new VolumeUpButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new ViewQueueButton(_audioService, _discordClient), sender, args);
            await buttonHandler.Execute(new RestartButton(_audioService, _discordClient), sender, args);
        };
    }

    private void MenuEvents()
    {
        var menuHandler = new MenuHandler();

        _discordClient.ComponentInteractionCreated += async (sender, args) =>
        {
            await menuHandler.Execute(new FilterMenu(_audioService, _discordClient), sender, args);
        };
    }
}