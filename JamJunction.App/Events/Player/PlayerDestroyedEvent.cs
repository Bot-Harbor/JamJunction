using DSharpPlus;
using Lavalink4NET.Events.Players;
using Newtonsoft.Json;

namespace JamJunction.App.Events.Player;

public class PlayerDestroyedEvent
{
    private readonly DiscordClient _discordClient;

    public PlayerDestroyedEvent(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

    public async Task PlayerDestroyed(object sender, PlayerDestroyedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        _ = channel.DeleteMessageAsync(guildData.Message);
        
        foreach (var userData in Bot.UserData.Values)
        {
            if (userData.GuildId == guildId)
            { 
                var userToRemove = Bot.UserData.FirstOrDefault(x =>
                    x.Value.GuildId == guildId).Key;
                Console.WriteLine(userToRemove);
                Bot.UserData.Remove(userToRemove);
            }
        }
        
        Bot.GuildData.Remove(guildId);
    }
}