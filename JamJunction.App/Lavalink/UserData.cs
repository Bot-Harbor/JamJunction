namespace JamJunction.App.Lavalink;

public class UserData
{
    public ulong GuildId { get; set; }
    public string CurrentPageNumber { get; set; } = "1";
    // Add view queue message here to prevent message from being edited/deleted for other user
}