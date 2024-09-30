using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;
    private LavalinkTrack Track { get; set; }

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }
    
    [SlashCommand("play", "Queue a song.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Song", "Enter the name or url of the song you want to queue.")]
        string query,
        [Option("Platform", "Pick a streaming platform.")]
        Platform streamingPlatform = default
    )
    {
        await context.DeferAsync();

        var player = await GetPlayerAsync(context, connectToVoiceChannel: true);
        
        switch (streamingPlatform)
        {
            case Platform.YouTube:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.YouTube);
                break;
            case Platform.YouTubeMusic:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.YouTubeMusic);
                break;
            case Platform.SoundCloud:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.SoundCloud);
                break;
            default:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.SoundCloud);
                break;
        }

        if (Track is null)
        {
            // Use audio player embed class
            var errorResponse = new DiscordFollowupMessageBuilder()
                .WithContent("😖 No results.");

            await context
                .FollowUpAsync(errorResponse);
        }
        
        var position = await player!.PlayAsync(Track);
        
        if (position == 0)
        {
            // Use audio player embed class
            var currentSongEmbed = new DiscordEmbedBuilder
            {
                Description = $"💿  •  **Now playing**: {Track.Title}\n" +
                              $"🎙️  •  **Artist**: {Track.Author}\n" +
                              $"⌛  •  **Song Duration** (HH:MM:SS): {RoundSeconds(Track.Duration)}\n" +
                              $"🔴  •  **Is a Livestream**: {Track.IsLiveStream}",
                Color = DiscordColor.Teal,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = Track.ArtworkUri!.AbsoluteUri
                }
            };
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(currentSongEmbed));
        }
        else
        {
            // Use audio player embed class
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"🔈 Added to queue: {Track.Uri}"))
                .ConfigureAwait(false);
        }
    }

    private TimeSpan RoundSeconds(TimeSpan  timespan)
    {
        return TimeSpan.FromSeconds(Math.Round(timespan.TotalSeconds));
    }
    
    private async ValueTask<QueuedLavalinkPlayer?> GetPlayerAsync(InteractionContext interactionContext, bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);
        
        var playerOptions = new QueuedLavalinkPlayerOptions { HistoryCapacity = 10000 };

        var result = await _audioService.Players
            .RetrieveAsync(interactionContext.Guild.Id, interactionContext.Member?.VoiceState.Channel!.Id,
                playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                // Use audio player embed class and move out of this method
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            var errorResponse = new DiscordFollowupMessageBuilder()
                .WithContent(errorMessage)
                .AsEphemeral();

            await interactionContext
                .FollowUpAsync(errorResponse);
        }

        return result.Player;
    }
}