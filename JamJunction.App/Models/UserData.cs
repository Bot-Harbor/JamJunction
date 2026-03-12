using DSharpPlus.Entities;

namespace JamJunction.App.Models;

/// <summary>
/// Represents data stored for an individual Discord user interacting with Jam Junction.
/// </summary>
/// <remarks>
/// This class is used to maintain user-specific state during bot interactions,
/// such as queue pagination preferences or temporary interaction data.
/// </remarks>
public class UserData
{
    /// <summary>
    /// Gets or sets the unique identifier of the Discord guild (server)
    /// associated with this stored data.
    /// </summary>
    public ulong GuildId { get; set; }

    /// <summary>
    /// Gets or sets the current page number being viewed in the queue interface.
    /// </summary>
    /// <remarks>
    /// This value is used to track pagination when users navigate through
    /// the music queue.
    /// </remarks>
    public string CurrentPageNumber { get; set; } = "1";

    /// <summary>
    /// Gets or sets the Discord message that displays the current queue view.
    /// </summary>
    /// <remarks>
    /// This message is updated when users navigate between queue pages
    /// or perform queue-related actions.
    /// </remarks>
    public DiscordMessage ViewQueueMessage { get; set; }
}