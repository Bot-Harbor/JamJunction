using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Interfaces;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Lavalink;

/// <summary>
/// Handles execution of platform-specific playback logic.
/// </summary>
/// <remarks>
/// This class acts as a dispatcher that invokes the appropriate
/// platform implementation (such as Spotify, YouTube, Deezer,
/// or SoundCloud) to resolve and play tracks.
/// </remarks>
public class PlatformHandler
{
    /// <summary>
    /// Executes the playback logic for the specified platform.
    /// </summary>
    /// <param name="platform">
    /// The <see cref="IPlatform"/> implementation responsible for
    /// resolving the track or playlist from a specific music platform.
    /// </param>
    /// <param name="player">
    /// The <see cref="QueuedLavalinkPlayer"/> that manages playback
    /// and the queue.
    /// </param>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> representing the Discord
    /// interaction that triggered the playback request.
    /// </param>
    /// <param name="query">
    /// The search query or URL used to locate the track or playlist.
    /// </param>
    /// <param name="queueNext">
    /// Indicates whether the track should be inserted next in the queue
    /// instead of being added to the end.
    /// </param>
    /// <remarks>
    /// This method delegates playback handling to the provided
    /// platform implementation.
    /// </remarks>
    public void Execute(IPlatform platform, QueuedLavalinkPlayer player, InteractionContext context, string query,
        bool queueNext = false)
    {
        platform.PlayTrack(player, context, query, queueNext);
    }
}