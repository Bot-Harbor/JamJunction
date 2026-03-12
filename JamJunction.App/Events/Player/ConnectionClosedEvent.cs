using DSharpPlus;
using Lavalink4NET.Events;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles Lavalink connection closed events.
/// </summary>
/// <remarks>
/// This event is triggered when the Lavalink connection is closed or lost.
/// When this occurs, all active player and queue messages stored in the
/// application's cached guild and user data are removed to prevent
/// stale messages from remaining in Discord channels.
///
/// After cleanup, the cached guild and user interaction data is cleared.
/// </remarks>
public class ConnectionClosedEvent
{
    /// <summary>
    /// The Discord client used to interact with the Discord API.
    /// </summary>
    /// <remarks>
    /// This client provides access to guilds, channels, users, and events
    /// within Discord. It is commonly used to retrieve guild information,
    /// resolve voice states, and perform actions such as sending or deleting messages.
    /// </remarks>
    private readonly DiscordClient _discordClient;

    public ConnectionClosedEvent(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }

    /// <summary>
    /// Executes cleanup operations when the Lavalink connection is closed.
    /// </summary>
    /// <param name="sender">
    /// The event source that triggered the connection closed event.
    /// </param>
    /// <param name="eventArgs">
    /// The <see cref="ConnectionClosedEventArgs"/> containing information
    /// about the Lavalink connection closure.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous cleanup operation.
    /// </returns>
    /// <remarks>
    /// This method removes all stored player and queue messages from Discord
    /// channels that were previously tracked in <c>Bot.GuildData</c> and
    /// <c>Bot.UserData</c>. This ensures the bot does not leave outdated
    /// player UI messages behind when the Lavalink node disconnects.
    /// </remarks>
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