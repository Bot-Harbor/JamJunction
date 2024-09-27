using DSharpPlus;
using DSharpPlus.SlashCommands;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;
using JamJunction.App.Slash_Commands.Other_Commands;
using Microsoft.Extensions.Hosting;

namespace JamJunction.App;

public class Bot : BackgroundService
{
    public static readonly Dictionary<ulong, AudioPlayerController> GuildAudioPlayers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordClient _discordClient;

    public Bot(IServiceProvider serviceProvider, DiscordClient discordClient)
    {
        _serviceProvider = serviceProvider;
        _discordClient = discordClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _discordClient
            .ConnectAsync()
            .ConfigureAwait(false);
        
        ConfigSlashCommands();
        ButtonEvents();
    }

    private void ConfigSlashCommands()
    {
        var slashCommands =
            _discordClient.UseSlashCommands(new SlashCommandsConfiguration {Services = _serviceProvider});

        slashCommands.RegisterCommands<PingCommand>();
        slashCommands.RegisterCommands<CaptionCommand>();
        slashCommands.RegisterCommands<PlayCommand>();
        slashCommands.RegisterCommands<PauseCommand>();
        slashCommands.RegisterCommands<ResumeCommand>();
        slashCommands.RegisterCommands<StopCommand>();
        slashCommands.RegisterCommands<VolumeCommand>();
        slashCommands.RegisterCommands<SeekCommand>();
        slashCommands.RegisterCommands<RestartCommand>();
        slashCommands.RegisterCommands<LeaveCommand>();
        slashCommands.RegisterCommands<MuteCommand>();
        slashCommands.RegisterCommands<UnmuteCommand>();
        slashCommands.RegisterCommands<ViewQueueCommand>();
        slashCommands.RegisterCommands<ShuffleQueueCommand>();
        slashCommands.RegisterCommands<CurrentSongCommand>();
        slashCommands.RegisterCommands<SkipCommand>();
        slashCommands.RegisterCommands<HelpCommand>();
        slashCommands.RegisterCommands<SongPositionCommand>();
    }

    private void ButtonEvents()
    {
        _discordClient.ComponentInteractionCreated += async (sender, args) =>
        {
            await ButtonHandler.Execute(new PauseButton(), sender, args);
            await ButtonHandler.Execute(new ResumeButton(), sender, args);
            await ButtonHandler.Execute(new StopButton(), sender, args);
            await ButtonHandler.Execute(new ShuffleButton(), sender, args);
            await ButtonHandler.Execute(new VolumeDownButton(), sender, args);
            await ButtonHandler.Execute(new VolumeUpButton(), sender, args);
            await ButtonHandler.Execute(new ViewQueueButton(), sender, args);
            await ButtonHandler.Execute(new MuteButton(), sender, args);
            await ButtonHandler.Execute(new RestartButton(), sender, args);
            await ButtonHandler.Execute(new SkipButton(), sender, args);
        };
    }
}