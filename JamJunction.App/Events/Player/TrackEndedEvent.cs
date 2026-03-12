using DSharpPlus;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Payloads.Events;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles track ended events from the Lavalink player.
/// </summary>
/// <remarks>
/// This event is triggered when a track finishes playing or is stopped.
/// It is responsible for managing the player UI state and cleaning up
/// cached guild and user data when playback ends.
///
/// Depending on the reason the track ended, the bot may:
/// - Remove the player embed message
/// - Notify users that the queue is empty
/// - Clear stored guild and user interaction data
/// </remarks>
public class TrackEndedEvent
{
    /// <summary>
    /// Provides access to the Lavalink audio service used for managing
    /// audio playback and retrieving player instances.
    /// </summary>
    /// <remarks>
    /// This service is used to interact with Lavalink through Lavalink4NET,
    /// allowing the application to control music playback, queues, filters,
    /// and other audio-related functionality.
    /// </remarks>
    private readonly IAudioService _audioService;
    
    /// <summary>
    /// The Discord client used to interact with the Discord API.
    /// </summary>
    /// <remarks>
    /// This client provides access to guilds, channels, users, and events
    /// within Discord. It is commonly used to retrieve guild information,
    /// resolve voice states, and perform actions such as sending or deleting messages.
    /// </remarks>
    private readonly DiscordClient _discordClient;

    public TrackEndedEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    /// <summary>
    /// Executes logic when a Lavalink track finishes playing.
    /// </summary>
    /// <param name="sender">
    /// The event source that triggered the track ended event.
    /// </param>
    /// <param name="eventArgs">
    /// The <see cref="TrackEndedEventArgs"/> containing information about
    /// the track that finished playing and the reason playback ended.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous event handling operation.
    /// </returns>
    /// <remarks>
    /// This method performs cleanup and state management when playback ends.
    /// If the track was manually stopped, the player UI is removed and cached
    /// guild and user data are cleared.
    ///
    /// If the player has no remaining tracks in the queue, a temporary message
    /// is displayed prompting users to queue additional tracks.
    /// </remarks>
    public async Task TrackEnded(object sender, TrackEndedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (eventArgs.Reason == TrackEndReason.Stopped)
        {
            _ = channel.DeleteMessageAsync(guildData.PlayerMessage);

            foreach (var userData in Bot.UserData.Values)
                if (userData.GuildId == guildId)
                {
                    var userToRemove = Bot.UserData.FirstOrDefault(x =>
                        x.Value.GuildId == guildId).Key;
                    Bot.UserData.Remove(userToRemove);
                }

            Bot.GuildData.Remove(guildId);
            return;
        }

        if (player.State == PlayerState.NotPlaying)
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            await channel.DeleteMessageAsync(guildData.PlayerMessage);

            var queueSomethingMessage = await channel.SendMessageAsync(audioPlayerEmbed.QueueSomething());

            await Task.Delay(10000);

            _ = channel.DeleteMessageAsync(queueSomethingMessage);

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
}