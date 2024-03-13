using DSharpPlus;
using DSharpPlus.Lavalink;
using Lavalink4NET;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using JamJunction.App.Events;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Secrets;
using JamJunction.App.Slash_Commands.Music_Commands;
using JamJunction.App.Slash_Commands.Other_Commands;
using Microsoft.Extensions.Logging;

namespace JamJunction.App;

public abstract class Bot
{
    public static readonly Dictionary<ulong, AudioPlayerController> GuildAudioPlayers = new();
    public static DiscordClient Client { get; set; }

    public static async Task RunBotAsync()
    {
        var discord = new DiscordSecrets();

        var discordConfig = new DiscordConfiguration
        {
            Intents = DiscordIntents.All,
            Token = discord.BotToken,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug
        };

        var endpoint = new ConnectionEndpoint
        {
            Hostname = discord.HostName,
            Port = discord.Port
        };

        var lavaLinkConfig = new LavalinkConfiguration
        {
            Password = discord.Password,
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint
        };

        Client = new DiscordClient(discordConfig);

        var lavaLink = Client.UseLavalink();

        await Client.ConnectAsync();
        
        Client.VoiceStateUpdated += VoiceStateUpdated.ClientOnVoiceStateUpdated;
        
        await lavaLink.ConnectAsync(lavaLinkConfig);
        var nodeConnection = lavaLink.GetNodeConnection(endpoint);
        
        nodeConnection.GuildConnectionCreated += GuildConnectionCreated.NodeConnectionOnGuildConnectionCreated;
        nodeConnection.TrackStuck += TrackStuck.TrackIsStuck;
        nodeConnection.PlaybackFinished += PlayBackFinished.PlayBackIsFinished;

        ButtonEvents();
        SlashCommands();

        await Task.Delay(-1);
    }

    private static void SlashCommands()
    {
        var slashCommands = Client.UseSlashCommands();

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

    private static void ButtonEvents()
    {
        Client.ComponentInteractionCreated += async (sender, args) =>
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