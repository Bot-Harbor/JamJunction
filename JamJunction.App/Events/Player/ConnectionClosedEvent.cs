using DSharpPlus;
using Lavalink4NET.Events;

namespace JamJunction.App.Events.Player;

public class ConnectionClosedEvent
{
    private readonly DiscordClient _discordClient;

    public ConnectionClosedEvent(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

    public async Task ConnectionClosed(object sender, ConnectionClosedEventArgs eventArgs)
    {
        if (Bot.GuildData != null)
        {
            foreach (var guildData in Bot.GuildData.Values)
            {
                var guild = await _discordClient.GetGuildAsync(guildData.PlayerMessage.Channel!.GuildId!.Value);
                var channel = guild.GetChannel(guildData.PlayerMessage.ChannelId);
                await channel.DeleteMessageAsync(guildData.PlayerMessage);
            }

            Bot.GuildData!.Clear();
        }

        if (Bot.UserData != null)
        {
            foreach (var userData in Bot.UserData.Values)
            {
                var guild = await _discordClient.GetGuildAsync(userData.GuildId);
                var channel = guild.GetChannel(userData.ViewQueueMessage.ChannelId);
                await channel.DeleteMessageAsync(userData.ViewQueueMessage);
            }

            Bot.UserData!.Clear();
        }
    }
}