using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class NodeDisconnected
{
    public static Task Disconnected(LavalinkNodeConnection sender, NodeDisconnectedEventArgs e)
    {
        var connectedGuilds = e.LavalinkNode.ConnectedGuilds;

        foreach (var connectedGuild in connectedGuilds)
        {
            var guildId = connectedGuild.Key;
            var audioPlayerController = Bot.GuildAudioPlayers[guildId];
            //Bot.GuildAudioPlayers.Remove(guildId, audioPlayerController);

            if (e.LavalinkNode.IsConnected == false)
            {
                audioPlayerController.Volume = 50;
                audioPlayerController.PauseInvoked = false;
                audioPlayerController.MuteInvoked = false;
                audioPlayerController.FirstSongInTrack = true;
                audioPlayerController.Queue.Clear();
                
                Console.WriteLine("Node disconnected");
            }
        }

        return Task.CompletedTask;
    }
}