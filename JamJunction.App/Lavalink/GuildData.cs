using DSharpPlus.Entities;

namespace JamJunction.App.Lavalink;

public class GuildData
{
    public ulong TextChannelId { get; set; }
    public DiscordMessage Message { get; set; }
    public DiscordMessage ViewQueueMessage { get; set; }
    public bool FirstSongInQueue { get; set; } = true;
    public bool RepeatMode { get; set; } = true;
}