using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events;

public class ClientReady
{
    public static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
    {
        return Task.CompletedTask;
    }
}