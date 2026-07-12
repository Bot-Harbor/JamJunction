using DSharpPlus;
using DSharpPlus.Entities;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles track stuck events from the Lavalink player.
/// </summary>
/// <remarks>
/// This event is triggered when Lavalink detects that a track has become
/// stuck during playback and cannot continue streaming normally.
///
/// The bot attempts to recover by restarting playback of the affected
/// track. If playback still fails after the retry attempt, an error
/// message is displayed and the stored guild playback state is cleared.
/// </remarks>
public class TrackStuckEvent
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

    public TrackStuckEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    /// <summary>
    /// Executes recovery logic when a track becomes stuck during playback.
    /// </summary>
    /// <param name="sender">
    /// The event source that triggered the track stuck event.
    /// </param>
    /// <param name="eventArgs">
    /// The <see cref="TrackStuckEventArgs"/> containing information about
    /// the track that became stuck and the Lavalink player instance.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous recovery operation.
    /// </returns>
    /// <remarks>
    /// This method notifies users that the track failed to load correctly
    /// and attempts to restart playback of the same track. If playback still
    /// fails after the retry attempt, an additional error message is shown
    /// and the guild playback data is removed to prevent stale player state.
    /// </remarks>
    public async Task TrackStuck(object sender, TrackStuckEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        // The failure path below removes the guild data to reset the player. If a
        // stuck event still arrives afterwards (a genuinely broken track can keep
        // stalling), there is nothing left to recover — bail out silently instead
        // of throwing on a missing key.
        if (!Bot.GuildData.TryGetValue(guildId, out var guildData))
            return;

        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var errorEmbed = new ErrorEmbed();

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        var track = eventArgs.Track;

        // A reattempt was already made for this exact track and it is stuck again,
        // so the retry has failed. Stop retrying (replaying a broken track just gets
        // it stuck again and loops forever), tell the user the reattempt failed,
        // remove the track information embed and reset the stored player state.
        if (guildData.ReattemptedTrackIdentifier == track.Identifier)
        {
            _ = channel.DeleteMessageAsync(guildData.PlayerMessage);

            var failedMessage = await channel.SendMessageAsync(
                new DiscordMessageBuilder(errorEmbed.CouldNotLoadTrackOnAttemptError()));

            // Stop the broken track so the player does not keep stalling in the
            // voice channel after we have given up on it.
            if (player != null)
                await player.StopAsync();

            foreach (var userData in Bot.UserData.Values)
                if (userData.GuildId == guildId)
                {
                    var userToRemove = Bot.UserData.FirstOrDefault(x =>
                        x.Value.GuildId == guildId).Key;
                    Bot.UserData.Remove(userToRemove);
                }

            Bot.GuildData.Remove(guildId);

            await Task.Delay(10000);
            _ = channel.DeleteMessageAsync(failedMessage);
            return;
        }

        // First time this track got stuck: notify the user and reattempt playback
        // once. Record the track identifier so a second stuck event for the same
        // track is handled as a failure above instead of retrying endlessly.
        guildData.ReattemptedTrackIdentifier = track.Identifier;

        var errorMessage = await channel.SendMessageAsync(
            new DiscordMessageBuilder(errorEmbed.TrackFailedToLoadError()));

        await Task.Delay(5000);

        _ = channel.DeleteMessageAsync(errorMessage);

        // Force an immediate replay of the stuck track (enqueue: false) rather than
        // appending it to the queue. The stuck track is still the player's CurrentTrack,
        // so the default enqueue behaviour would just add a duplicate to the queue.
        if (player != null)
            await player.PlayAsync(track, false);
    }
}