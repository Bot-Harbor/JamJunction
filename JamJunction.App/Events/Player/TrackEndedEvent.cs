using DSharpPlus;
using DSharpPlus.Entities;
using JamJunction.App.Ai;
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

            if (guildData.RecommendationMessage != null)
                _ = channel.DeleteMessageAsync(guildData.RecommendationMessage);

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
            await channel.DeleteMessageAsync(guildData.PlayerMessage);

            if (guildData.RecommendationMessage != null)
                _ = channel.DeleteMessageAsync(guildData.RecommendationMessage);

            if (player.Queue.HasHistory)
                await player.Queue.History!.ClearAsync();

            foreach (var userData in Bot.UserData.Values)
                if (userData.GuildId == guildId)
                {
                    var userToRemove = Bot.UserData.FirstOrDefault(x =>
                        x.Value.GuildId == guildId).Key;
                    Bot.UserData.Remove(userToRemove);
                }

            Bot.GuildData.Remove(guildId);

            var audioPlayerEmbed = new AudioPlayerEmbed();
            var queueSomethingMessage = await channel.SendMessageAsync(
                new DiscordMessageBuilder(audioPlayerEmbed.QueueSomething()));

            await Task.Delay(10000);
            _ = channel.DeleteMessageAsync(queueSomethingMessage);
            return;
        }

        // The song is over but the queue continues (this was not the last track).
        // Give the AI recommender a 1-in-8 chance to suggest a follow-up track.
        // Runs in the background so it never delays the next track's player UI.
        if (eventArgs.Reason == TrackEndReason.Finished &&
            SongRecommender.IsEnabled &&
            Random.Shared.Next(2) == 0)
            _ = ShowRecommendationAsync(channel, guildId, eventArgs.Track);
    }

    /// <summary>
    /// Fetches an AI song recommendation for the finished track and posts it to the
    /// text channel with an "add to queue" button.
    /// </summary>
    /// <param name="channel">
    /// The <see cref="DiscordChannel"/> in which the recommendation is displayed.
    /// </param>
    /// <param name="guildId">
    /// The identifier of the guild the recommendation belongs to. Used to store the
    /// recommendation state so it can be queued or cleaned up later.
    /// </param>
    /// <param name="finishedTrack">
    /// The track that just finished playing, used as the basis for the recommendation.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous recommendation operation.
    /// </returns>
    /// <remarks>
    /// The recommendation is not removed on a timer. Instead it remains visible until
    /// the user presses its button to queue the suggested track, or until the next
    /// track starts playing (see <see cref="TrackStartedEvent"/>), whichever occurs
    /// first. Only one recommendation is shown per guild at a time.
    /// </remarks>
    private static async Task ShowRecommendationAsync(DiscordChannel channel, ulong guildId,
        Lavalink4NET.Tracks.LavalinkTrack finishedTrack)
    {
        try
        {
            var recommendation = await SongRecommender.RecommendAsync(finishedTrack);

            if (recommendation == null)
                return;

            // Playback may have stopped while the model was generating the
            // recommendation. If the guild is no longer active, don't post an
            // orphaned message that nothing would clean up.
            if (!Bot.GuildData.TryGetValue(guildId, out var guildData))
                return;

            // Only one recommendation should be visible at a time. Remove a previous
            // one that may still be lingering before posting the new suggestion.
            if (guildData.RecommendationMessage != null)
            {
                var previousMessage = guildData.RecommendationMessage;
                guildData.RecommendationMessage = null;
                _ = channel.DeleteMessageAsync(previousMessage);
            }

            var audioPlayerEmbed = new AudioPlayerEmbed();
            var recommendationBuilder = audioPlayerEmbed.SongRecommendation(
                finishedTrack, recommendation.Title, recommendation.Artist, recommendation.Reason);

            var recommendationMessage = await channel.SendMessageAsync(recommendationBuilder);

            guildData.RecommendationMessage = recommendationMessage;
            guildData.RecommendationQuery = $"{recommendation.Title} {recommendation.Artist}";

            // Remember which platform the finished track came from so the suggested
            // track is queued from that same source (Spotify -> Spotify, etc.).
            // YouTube Music tracks report a "youtube" SourceName; the only thing that
            // marks them as YouTube Music is the music.youtube.com URI, so detect that
            // and preserve it. This keeps the queued suggestion labelled "YouTube Music"
            // in the player UI, matching how the play command already handles it.
            guildData.RecommendationSource =
                finishedTrack.SourceName == "youtube" &&
                finishedTrack.Uri?.ToString().Contains("music.youtube.com") == true
                    ? "youtubemusic"
                    : finishedTrack.SourceName;
        }
        catch
        {
            // The recommender is best-effort; never let a failure surface here.
        }
    }
}