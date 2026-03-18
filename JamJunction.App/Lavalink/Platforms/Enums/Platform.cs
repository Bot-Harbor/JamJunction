using DSharpPlus.SlashCommands;

namespace JamJunction.App.Lavalink.Platforms.Enums;

/// <summary>
/// Represents the supported music platforms that Jam Junction
/// can use to resolve and play audio tracks.
/// </summary>
/// <remarks>
/// Each value corresponds to a platform integration that can load
/// tracks, albums, or playlists and send them to the Lavalink player
/// for playback.
/// </remarks>
public enum Platform
{
    /// <summary>
    /// Represents the Spotify platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks, albums, and playlists specifically from
    /// Spotify sources.
    /// </remarks>
    [ChoiceName("Spotify")] Spotify,

    /// <summary>
    /// Represents the YouTube platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks and playlists specifically from
    /// YouTube sources.
    /// </remarks>
    [ChoiceName("YouTube")] YouTube,

    /// <summary>
    /// Represents the Deezer platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks, albums, and playlists specifically from
    /// Deezer sources.
    /// </remarks>
    [ChoiceName("Deezer")] Deezer,

    /// <summary>
    /// Represents the SoundCloud platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks and sets specifically from
    /// SoundCloud sources.
    /// </remarks>
    [ChoiceName("SoundCloud")] SoundCloud,

    /// <summary>
    /// Represents the YouTube Music platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks and playlists specifically from
    /// YouTube Music sources.
    /// </remarks>
    [ChoiceName("YouTubeMusic")] YouTubeMusic,
    
    /// <summary>
    /// Represents the Apple Music platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks, albums, and playlists specifically from
    /// Apple Music sources.
    /// </remarks>
    [ChoiceName("AppleMusic")] AppleMusic,
}