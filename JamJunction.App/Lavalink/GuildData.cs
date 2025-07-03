namespace JamJunction.App.Lavalink;

public class GuildData
{
    public ulong TextChannelId { get; set; }
    public bool FirstSongInQueue { get; set; } = true;
    public bool RepeatMode { get; set; } = true;
}