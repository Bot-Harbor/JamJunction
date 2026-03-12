using DSharpPlus.SlashCommands;

namespace JamJunction.App.Lavalink.Enums;

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
    /// Represents the Spotify music platform.
    /// </summary>
    /// <remarks>
    /// Allows Jam Junction to resolve and play tracks, albums,
    /// and playlists sourced from Spotify.
    /// </remarks>
    [ChoiceName("Spotify")] Spotify,

    /// <summary>
    /// Represents the YouTube video platform.
    /// </summary>
    /// <remarks>
    /// Used to load and play audio from standard YouTube videos.
    /// </remarks>
    [ChoiceName("YouTube")] YouTube,

    /// <summary>
    /// Represents the Deezer music streaming platform.
    /// </summary>
    /// <remarks>
    /// Allows tracks, albums, and playlists from Deezer
    /// to be resolved and played through the audio player.
    /// </remarks>
    [ChoiceName("Deezer")] Deezer,

    /// <summary>
    /// Represents the SoundCloud audio platform.
    /// </summary>
    /// <remarks>
    /// Used to load and stream tracks hosted on SoundCloud.
    /// </remarks>
    [ChoiceName("SoundCloud")] SoundCloud,

    /// <summary>
    /// Represents the YouTube Music platform.
    /// </summary>
    /// <remarks>
    /// Used to load tracks and playlists specifically from
    /// YouTube Music sources.
    /// </remarks>
    [ChoiceName("YouTubeMusic")] YouTubeMusic
}