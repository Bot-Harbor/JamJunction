using DSharpPlus;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles track started events from the Lavalink player.
/// </summary>
/// <remarks>
/// This event is triggered whenever a new track begins playing on the
/// Lavalink player. It updates the player UI message in the Discord
/// text channel to reflect the currently playing track.
///
/// The event ensures that the player embed always displays the correct
/// track information when playback transitions to a new track.
/// </remarks>
public class TrackStartedEvent
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

    public TrackStartedEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    /// <summary>
    /// Executes logic when a Lavalink track begins playing.
    /// </summary>
    /// <param name="sender">
    /// The event source that triggered the track started event.
    /// </param>
    /// <param name="eventArgs">
    /// The <see cref="TrackStartedEventArgs"/> containing information
    /// about the track that started playing and the player instance.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous event handling operation.
    /// </returns>
    /// <remarks>
    /// This method retrieves the active Lavalink player and updates the
    /// player embed message in the associated Discord text channel.
    ///
    /// If the track is the first track added to the queue, the existing
    /// player message is preserved. Otherwise, the previous player message
    /// is removed and replaced with a new embed reflecting the current track.
    /// </remarks>
    public async Task TrackStarted(object sender, TrackStartedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var track = eventArgs.Track;

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (guildData.FirstSongInQueue)
        {
            guildData.FirstSongInQueue = false;
            return;
        }

        var playerMessage = guildData.PlayerMessage;
        _ = channel.DeleteMessageAsync(playerMessage);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        playerMessage = await channel.SendMessageAsync(audioPlayerEmbed.TrackInformation(track, player, true));
        guildData.PlayerMessage = playerMessage;
    }
}