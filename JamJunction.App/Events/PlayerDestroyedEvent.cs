using DSharpPlus;
using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events;

public class PlayerDestroyedEvent
{
    private readonly DiscordClient _discordClient;

    public PlayerDestroyedEvent(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

    public async Task PlayerDestroyed(object sender, PlayerDestroyedEventArgs eventargs)
    {
        var guildId = eventargs.Player.GuildId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        _  = channel.DeleteMessageAsync(guildData.Message);
        Bot.GuildData.Remove(guildId);
    }
}