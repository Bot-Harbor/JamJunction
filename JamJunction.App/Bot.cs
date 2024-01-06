using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using JamJunction.App.Events;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Secrets;
using JamJunction.App.Slash_Commands;
using JamJunction.App.Slash_Commands.Music_Commands;
using JamJunction.App.Slash_Commands.Other_Commands;
using Microsoft.Extensions.Logging;

namespace JamJunction.App;

public abstract class Bot
{
    public static DiscordClient Client { get; set; }

    public static async Task RunBotAsync()
    {
        var discord = new DiscordSecrets();

        var discordConfig = new DiscordConfiguration()
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
            Port = discord.Port,
            Secured = true
        };

        var lavaLinkConfig = new LavalinkConfiguration
        {
            Password = discord.Password,
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint,
        };
        
        Client = new DiscordClient(discordConfig);
        
        Client.Ready += ClientReady.Client_Ready;
        
        ButtonEvents();
        
        SlashCommands();

        var lavaLink = Client.UseLavalink();
        
        await Client.ConnectAsync();
        await lavaLink.ConnectAsync(lavaLinkConfig);

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
    }

    private static void ButtonEvents()
    {
        Client.ComponentInteractionCreated += async (sender, args) =>
        {
            await ButtonHandler.Execute(new PauseButton(), sender, args);
        };
    }
}