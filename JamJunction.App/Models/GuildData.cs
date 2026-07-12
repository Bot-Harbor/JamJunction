using DSharpPlus.Entities;

namespace JamJunction.App.Models;

/// <summary>
/// Represents data stored for a Discord guild used by Jam Junction.
/// </summary>
/// <remarks>
/// This class is used to maintain guild-specific state or configuration
/// such as queue pagination, player settings, or other server-related data
/// required during bot interactions.
/// </remarks>
public class GuildData
{
    /// <summary>
    /// Gets or sets the ID of the text channel where the audio player
    /// message is displayed and updated.
    /// </summary>
    public ulong TextChannelId { get; set; }

    /// <summary>
    /// Gets or sets the Discord message that represents the active
    /// audio player interface in the text channel.
    /// </summary>
    public DiscordMessage PlayerMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the next track being
    /// added to the queue is the first track.
    /// </summary>
    /// <remarks>
    /// This is typically used to determine whether playback should
    /// start immediately when a track is queued.
    /// </remarks>
    public bool FirstSongInQueue { get; set; } = true;

    /// <summary>
    /// Gets or sets the currently displayed AI song recommendation message,
    /// if one is present.
    /// </summary>
    /// <remarks>
    /// At most one recommendation is shown per guild at a time. The message is
    /// removed either when the user adds the suggested track via its button or
    /// when the next track starts playing, whichever happens first.
    /// </remarks>
    public DiscordMessage RecommendationMessage { get; set; }

    /// <summary>
    /// Gets or sets the search query for the track suggested by the currently
    /// displayed AI song recommendation.
    /// </summary>
    /// <remarks>
    /// This is the "<c>title artist</c>" query used to look up and queue the
    /// recommended track when the user presses the add button on
    /// <see cref="RecommendationMessage"/>.
    /// </remarks>
    public string RecommendationQuery { get; set; }

    /// <summary>
    /// Gets or sets the source platform of the track the currently displayed AI
    /// recommendation was based on, taken from the Lavalink track
    /// <c>SourceName</c> (e.g. <c>spotify</c>, <c>youtube</c>, <c>deezer</c>,
    /// <c>soundcloud</c>, <c>applemusic</c>).
    /// </summary>
    /// <remarks>
    /// The suggested track is queued from this same platform, so a recommendation
    /// based on a Spotify track resolves to Spotify, one based on a YouTube track
    /// resolves to YouTube, and so on. Cleared alongside
    /// <see cref="RecommendationQuery"/> when the recommendation is queued or removed.
    /// </remarks>
    public string RecommendationSource { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the track that a single reattempt has
    /// already been made for after it became stuck during playback.
    /// </summary>
    /// <remarks>
    /// Used by <c>TrackStuckEvent</c> to limit recovery to one retry per track.
    /// When a track becomes stuck it is reattempted once and its identifier is
    /// stored here; if the same track becomes stuck again, the retry is treated
    /// as failed instead of looping the reattempt endlessly. Keyed on the track
    /// identifier so a different track that later gets stuck still receives a
    /// fresh reattempt.
    /// </remarks>
    public string ReattemptedTrackIdentifier { get; set; }
}