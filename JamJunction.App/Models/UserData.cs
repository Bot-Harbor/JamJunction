using DSharpPlus.Entities;

namespace JamJunction.App.Models;

public class UserData
{
    public ulong GuildId { get; set; }
    public string CurrentPageNumber { get; set; } = "1";
    public DiscordMessage ViewQueueMessage { get; set; }
}