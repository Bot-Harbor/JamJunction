using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class PlayBackFinished
{
    public static Task PlayBackIsFinished(LavalinkGuildConnection sender, TrackFinishEventArgs args)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = sender.Guild.Id;
        var audioPlayerController = Bot.GuildAudioPlayers[guildId];
        
        var channel = sender.Channel.Guild.GetChannel(guildId);
        
        if (args.Reason == TrackEndReason.Finished)
        {
            var connection = sender;
            
            if (audioPlayerController.Queue.Count > 0)
            {
                audioPlayerController.CurrentSongData = audioPlayerController.Queue.Peek();
                var nextTrackInQueue = audioPlayerController.Queue.Dequeue();
        
                // Object reference not set to an instance of an object. Error Generating Embed.
                channel.SendMessageAsync(
                    new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(nextTrackInQueue, sender)));
                
                Console.WriteLine("Next Song");
                connection.PlayAsync(nextTrackInQueue);
            }
            else
            {
                channel.SendMessageAsync(
                    new DiscordMessageBuilder().AddEmbed(audioEmbed.QueueSomethingEmbedBuilder()));
                
                Console.WriteLine("No more songs :(");
                
                audioPlayerController.FirstSongInTrack = true;
            }
        }

        if (args.Reason == TrackEndReason.Stopped)
        {
            audioPlayerController.Volume = 50;
            audioPlayerController.PauseInvoked = false;
            audioPlayerController.MuteInvoked = false;
            audioPlayerController.FirstSongInTrack = true;
            audioPlayerController.Queue.Clear();
            
            Console.WriteLine("Stop has been used");
        }
        
        if (args.Reason == TrackEndReason.LoadFailed)
        {
            // Error Generating Embed
            channel.SendMessageAsync(
                new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackFailedToLoadEmbedBuilder()));
            Task.Delay(TimeSpan.FromSeconds(5));

            if (audioPlayerController.Queue.Count > 0)
            {
                var connection = sender;

                audioPlayerController.CurrentSongData = audioPlayerController.Queue.Peek();
                var nextTrackInQueue = audioPlayerController.Queue.Dequeue();

                // Error Generating Embed
                channel.SendMessageAsync(
                    new DiscordMessageBuilder(audioEmbed.SongEmbedBuilder(nextTrackInQueue, sender)));

                connection.PlayAsync(nextTrackInQueue);
            }

            if (args.Reason == TrackEndReason.LoadFailed)
            {
                // Error Generating Embed
                channel.SendMessageAsync(
                    new DiscordMessageBuilder().AddEmbed(errorEmbed.CouldNotLoadTrackOnAttemptEmbedBuilder()));
                
                audioPlayerController.Volume = 50;
                audioPlayerController.PauseInvoked = false;
                audioPlayerController.MuteInvoked = false;
                audioPlayerController.FirstSongInTrack = true;
                audioPlayerController.Queue.Clear();
            }
        }

        return Task.CompletedTask;
    }
}