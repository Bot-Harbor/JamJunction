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
}