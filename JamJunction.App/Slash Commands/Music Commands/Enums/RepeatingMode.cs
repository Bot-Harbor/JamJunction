using DSharpPlus.SlashCommands;

namespace JamJunction.App.Slash_Commands.Music_Commands.Enums;

/// <summary>
/// Represents the repeat mode options available for the music player.
/// </summary>
/// <remarks>
/// This enumeration defines how the Lavalink player should behave
/// when a track finishes playing. These values are exposed to users
/// through slash command choices.
/// </remarks>
public enum RepeatingMode
{
    /// <summary>
    /// Disables repeat functionality.
    /// </summary>
    /// <remarks>
    /// Playback will stop when the queue finishes.
    /// </remarks>
    [ChoiceName("None")] None,
    
    /// <summary>
    /// Repeats the currently playing track.
    /// </summary>
    /// <remarks>
    /// The same track will continue playing until the repeat mode is changed.
    /// </remarks>
    [ChoiceName("Repeat Track")] RepeatTrack,
    
    /// <summary>
    /// Repeats the entire queue.
    /// </summary>
    /// <remarks>
    /// When the last track in the queue finishes, playback will restart
    /// from the beginning of the queue.
    /// </remarks>
    [ChoiceName("Repeat Queue")] RepeatQueue
}