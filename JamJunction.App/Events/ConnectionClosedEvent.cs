using Lavalink4NET.Events;

namespace JamJunction.App.Events;

public class ConnectionClosedEvent
{
    public Task AudioServiceOnConnectionClosed(object sender, ConnectionClosedEventArgs eventArgs)
    {
        Bot.GuildData.Clear();
        return Task.CompletedTask;
    }
}