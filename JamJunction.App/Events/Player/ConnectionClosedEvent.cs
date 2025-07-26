using Lavalink4NET.Events;

namespace JamJunction.App.Events.Player;

public class ConnectionClosedEvent
{
    public Task ConnectionClosed(object sender, ConnectionClosedEventArgs eventArgs)
    {
        Bot.GuildData.Clear();
        return Task.CompletedTask;
    }
}