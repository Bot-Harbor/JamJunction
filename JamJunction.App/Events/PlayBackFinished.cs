using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class PlayBackFinished
{
    public static async Task PlayBackFinishedAsync(LavalinkGuildConnection sender, TrackFinishEventArgs args)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var queue = PlayCommand.Queue;

        var channelId = PlayCommand.ChannelId;
        var channel = sender.Guild.GetChannel(channelId);

        if (args.Reason == TrackEndReason.Finished)
        {
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
        
        if (args.Reason == TrackEndReason.LoadFailed)
        {
            await channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackFailedToLoadEmbedBuilder()));
            await Task.Delay(TimeSpan.FromSeconds(5));

            if (queue.Count > 0)
            {
                var connection = sender;

                PlayCommand.CurrentSongData = queue.Peek();
                var nextTrackInQueue = queue.Dequeue();

                await channel.SendMessageAsync(
                    new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(nextTrackInQueue, sender)));

                await connection.PlayAsync(nextTrackInQueue);
            }

            if (args.Reason == TrackEndReason.LoadFailed)
            {
                await channel.SendMessageAsync(
                    new DiscordMessageBuilder().AddEmbed(errorEmbed.CouldNotLoadTrackOnAttemptEmbedBuilder()));
                ResetAudioPlayer.GeneralReset();
            }
        }
    }
}