using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace JamJunction.App.Events;

public class GuildConnectionCreated
{
    public static Task NodeConnectionOnGuildConnectionCreated(LavalinkGuildConnection sender, GuildConnectionCreatedEventArgs args)
    {
        var guildId = sender.Guild.Id;
        var audioPlayerController = new AudioPlayerController();
        Bot.GuildAudioPlayers.Add(guildId, audioPlayerController);
        
        // Counts The Active Servers
        Console.WriteLine(Bot.GuildAudioPlayers.Count);
        
        return Task.CompletedTask;
    }
}