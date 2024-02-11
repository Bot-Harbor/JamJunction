using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Events;

public class PlayBackFinished
{
    public static async Task PlayBackIsFinished(LavalinkGuildConnection sender, TrackFinishEventArgs args)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = sender.Guild.Id;
        var audioPlayerController = Bot.GuildAudioPlayers[guildId];

        var channelId = audioPlayerController.ChannelId;
        var channel = sender.Guild.GetChannel(channelId);
        
        if (args.Reason == TrackEndReason.Finished)
        {
            var connection = sender;
            
            if (audioPlayerController.Queue.Count > 0)
            {
                audioPlayerController.CurrentSongData = audioPlayerController.Queue.Peek();
                var nextTrackInQueue = audioPlayerController.Queue.Dequeue();
                
                await channel.SendMessageAsync(
                    new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(sender)));
                
                await connection.PlayAsync(nextTrackInQueue);
            }
            else
            {
                await channel.SendMessageAsync(
                    new DiscordMessageBuilder().AddEmbed(audioEmbed.QueueSomethingEmbedBuilder()));
                
                audioPlayerController.Volume = 50;
                audioPlayerController.PauseInvoked = false;
                audioPlayerController.MuteInvoked = false;
                audioPlayerController.FirstSongInTrack = true;
                audioPlayerController.Queue.Clear();

                var tokenSource = audioPlayerController.CancellationTokenSource = new CancellationTokenSource();
                
                await Task.Delay(TimeSpan.FromMinutes(10), tokenSource.Token);

                if (!audioPlayerController.CancellationTokenSource.IsCancellationRequested)
                {
                    await sender.DisconnectAsync();   
                }
            }
        }

        if (args.Reason == TrackEndReason.Stopped)
        {
            audioPlayerController.Volume = 50;
            audioPlayerController.PauseInvoked = false;
            audioPlayerController.MuteInvoked = false;
            audioPlayerController.FirstSongInTrack = true;
            audioPlayerController.Queue.Clear();
            
            var tokenSource = audioPlayerController.CancellationTokenSource = new CancellationTokenSource();
                
            await Task.Delay(TimeSpan.FromMinutes(10), tokenSource.Token);

            if (!audioPlayerController.CancellationTokenSource.IsCancellationRequested)
            {
                await sender.DisconnectAsync();   
            }
        }
        
        if (args.Reason == TrackEndReason.LoadFailed)
        {
            await channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackFailedToLoadEmbedBuilder()));
            await Task.Delay(TimeSpan.FromSeconds(5));

            if (audioPlayerController.Queue.Count > 0)
            {
                var connection = sender;

                audioPlayerController.CurrentSongData = audioPlayerController.Queue.Peek();
                var nextTrackInQueue = audioPlayerController.Queue.Dequeue();
                
                await channel.SendMessageAsync(
                    new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(sender)));

                await connection.PlayAsync(nextTrackInQueue);
            }

            if (args.Reason == TrackEndReason.LoadFailed)
            {
                await channel.SendMessageAsync(
                    new DiscordMessageBuilder().AddEmbed(errorEmbed.CouldNotLoadTrackOnAttemptEmbedBuilder()));
                
                audioPlayerController.Volume = 50;
                audioPlayerController.PauseInvoked = false;
                audioPlayerController.MuteInvoked = false;
                audioPlayerController.FirstSongInTrack = true;
                audioPlayerController.Queue.Clear();
                
                var tokenSource = audioPlayerController.CancellationTokenSource = new CancellationTokenSource();
                
                await Task.Delay(TimeSpan.FromMinutes(10), tokenSource.Token);

                if (!audioPlayerController.CancellationTokenSource.IsCancellationRequested)
                {
                    await sender.DisconnectAsync();   
                }
            }
        }
    }
}