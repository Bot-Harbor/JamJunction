using DSharpPlus;
using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles Lavalink player destruction events.
/// </summary>
/// <remarks>
/// This event is triggered when a Lavalink player instance is destroyed,
/// typically when the bot disconnects from a voice channel or the player
/// session ends.
///
/// When the player is destroyed, all related player UI messages and
/// stored guild data are cleaned up to prevent stale state from
/// remaining in memory or Discord channels.
/// </remarks>
public class PlayerDestroyedEvent
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

    public PlayerDestroyedEvent(DiscordClient discordClient)
    {
        _discordClient = discordClient;
    }
    
    /// <summary>
    /// Executes cleanup operations when a Lavalink player is destroyed.
    /// </summary>
    /// <param name="sender">
    /// The event source that triggered the player destruction.
    /// </param>
    /// <param name="eventArgs">
    /// The <see cref="PlayerDestroyedEventArgs"/> containing information
    /// about the destroyed Lavalink player instance.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous cleanup operation.
    /// </returns>
    /// <remarks>
    /// This method removes the active player message from the Discord channel
    /// and clears the corresponding guild and user interaction data stored
    /// within the bot's internal caches.
    /// </remarks>
    public async Task PlayerDestroyed(object sender, PlayerDestroyedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        _ = channel.DeleteMessageAsync(guildData.PlayerMessage);

        foreach (var userData in Bot.UserData.Values)
            if (userData.GuildId == guildId)
            {
                var userToRemove = Bot.UserData.FirstOrDefault(x =>
                    x.Value.GuildId == guildId).Key;
                Bot.UserData.Remove(userToRemove);
            }

        Bot.GuildData.Remove(guildId);
    }
}