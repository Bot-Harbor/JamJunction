using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace JamJunction.App.Events;

public class GuildConnectionCreated
{
    public static Task NodeConnectionOnGuildConnectionCreated(LavalinkGuildConnection sender, GuildConnectionCreatedEventArgs args)
    {
        if (sender.IsConnected)
        {
            var guildId = sender.Guild.Id;
            var audioPlayerController = new AudioPlayerController();
            Bot.GuildAudioPlayers.Add(guildId, audioPlayerController);
        }
        
        return Task.CompletedTask;
    }
}