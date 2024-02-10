using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace JamJunction.App.Events;

public class GuildConnectionRemoved
{
    public static Task NodeConnectionOnGuildConnectionRemoved(LavalinkGuildConnection sender, GuildConnectionRemovedEventArgs args)
    {
        Console.WriteLine("Node Disconnected Guild Event");
        return Task.CompletedTask;
    }
}