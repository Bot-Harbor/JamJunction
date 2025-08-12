using DSharpPlus.Entities;

namespace JamJunction.App.Models;

public class GuildData
{
    public ulong TextChannelId { get; set; }
    public DiscordMessage PlayerMessage { get; set; }
    public bool FirstSongInQueue { get; set; } = true;
    public bool RepeatMode { get; set; } = true;
}