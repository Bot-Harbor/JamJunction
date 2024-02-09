using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class TrackStuck
{
    public static async Task TrackStuckAsync(LavalinkGuildConnection sender, TrackStuckEventArgs args)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var queue = PlayCommand.Queue;
        var channelId = PlayCommand.ChannelId;
        var channel = sender.Guild.GetChannel(channelId);

        if (queue.Count > 0)
        {
            var connection = sender;

            PlayCommand.CurrentSongData = queue.Peek();
            var nextTrackInQueue = queue.Dequeue();

            await channel.SendMessageAsync(
                new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(nextTrackInQueue, sender)));

            await connection.PlayAsync(nextTrackInQueue);
        }
        else
        {
            await channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbed(audioEmbed.QueueSomethingEmbedBuilder()));
            PlayCommand.FirstSongInTrack = true;
        }
    }
}