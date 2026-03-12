using DSharpPlus;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;

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

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var errorEmbed = new ErrorEmbed();

        var errorMessage = await channel.SendMessageAsync(errorEmbed.TrackFailedToLoadError());

        await Task.Delay(5000);

        _ = channel.DeleteMessageAsync(errorMessage);

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        var track = eventArgs.Track;
        await player.PlayAsync(track);

        await Task.Delay(3000);

        if (player.State == PlayerState.NotPlaying)
        {
            errorMessage = await channel.SendMessageAsync(errorEmbed.CouldNotLoadTrackOnAttemptError());

            await Task.Delay(10000);

            await channel.DeleteMessageAsync(errorMessage);

            Bot.GuildData.Remove(guildId);
        }
    }
}