using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using JamJunction.App.Secrets;
using JamJunction.App.Slash_Commands;

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
            AutoReconnect = true
        };

        Client = new DiscordClient(discordConfig);

        Client.Ready += Client_Ready;
        
        SlashCommands();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
    
    private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }

    public static void SlashCommands()
    {
        var slashCommands = Client.UseSlashCommands();
        
        slashCommands.RegisterCommands<PingCommand>();
    }
}