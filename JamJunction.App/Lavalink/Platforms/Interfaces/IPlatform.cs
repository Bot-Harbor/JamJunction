using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Platforms.Enums;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Lavalink.Platforms.Interfaces;

/// <summary>
/// Defines the contract for platform integrations used by Jam Junction
/// to resolve and play audio tracks.
/// </summary>
/// <remarks>
/// Implementations of this interface handle loading tracks from
/// supported platforms such as Spotify, YouTube, Deezer, SoundCloud,
/// and YouTube Music.
/// </remarks>
public interface IPlatform
{
    /// <summary>
    /// Gets or sets the platform type associated with the implementation.
    /// </summary>
    Platform Platform { get; set; }

    /// <summary>
    /// Gets or sets the URL associated with the platform request.
    /// </summary>
    /// <remarks>
    /// This may represent a track, playlist, album, or search query
    /// depending on the platform being used.
    /// </remarks>
    string Url { get; set; }

    /// <summary>
    /// Resolves a query or URL from the platform and sends the resulting
    /// track to the Lavalink player for playback.
    /// </summary>
    /// <param name="player">
    /// The <see cref="QueuedLavalinkPlayer"/> responsible for managing
    /// playback and the track queue.
    /// </param>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the Discord
    /// interaction that initiated the request.
    /// </param>
    /// <param name="query">
    /// The search query or direct platform URL used to locate the track.
    /// </param>
    /// <param name="queueNext">
    /// Indicates whether the track should be inserted next in the queue
    /// instead of being appended to the end.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous playback operation.
    /// </returns>
    Task PlayTrack(QueuedLavalinkPlayer player, InteractionContext context, string query, bool queueNext = false);
}