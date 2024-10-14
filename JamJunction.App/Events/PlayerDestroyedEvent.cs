using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events;

public class PlayerDestroyedEvent
{
    public Task PlayerDestroyed(object sender, PlayerDestroyedEventArgs eventargs)
    {
        var guildId = eventargs.Player.GuildId;
        Bot.GuildData.Remove(guildId);
        return Task.CompletedTask;
    }
}