using System.Collections.Immutable;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using Lavalink4NET;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace JamJunction.App.Lavalink;

public class PlatformHandler
{
    private readonly IAudioService _audioService;
    private GuildData GuildData { get; set; }
    private AudioPlayerEmbed AudioPlayerEmbed { get; set; } = new();
    private ErrorEmbed ErrorEmbed { get; set; } = new();

    public PlatformHandler(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public async Task PlayFromYoutube(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var youtube = new YoutubeClient();

        if (query.Contains("youtube.com"))
        {
            Video video;

            try
            {
                video = await youtube.Videos.GetAsync(query);
            }
            catch (Exception)
            {
                video = null;
            }

            if (video == null)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                return;
            }

            var seekable = true;
            var liveStream = false;

            if (video.Duration == TimeSpan.Zero)
            {
                seekable = false;
                liveStream = true;
            }

            var youtubeTrack = new LavalinkTrack
            {
                SourceName = "youtube",
                Identifier = video.Id,
                IsSeekable = seekable,
                IsLiveStream = liveStream,
                Title = video.Title,
                Author = video.Author.ToString(),
                StartPosition = TimeSpan.Zero,
                Duration = (TimeSpan) video.Duration!,
                Uri = new Uri(video.Url),
                ArtworkUri = new Uri(video.Thumbnails.FirstOrDefault()?.Url!),
            };

            if (youtubeTrack.IsLiveStream)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId))
            {
                Bot.GuildData.Add(guildId, new GuildData());
            }

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            await player.PlayAsync(youtubeTrack);

            if (player.Queue.IsEmpty)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongInformation(youtubeTrack, player))));

                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.SongAddedToQueue(youtubeTrack)));
        }
        else
        {
            IReadOnlyList<VideoSearchResult> videos;

            try
            {
                videos = await youtube.Search.GetVideosAsync(query);
            }
            catch (Exception)
            {
                videos = null;
            }

            if (videos == null)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                return;
            }

            var video = videos.FirstOrDefault();

            var seekable = true;
            var liveStream = false;
            
            if (video!.Duration == TimeSpan.Zero)
            {
                seekable = false;
                liveStream = true;
            }

            var youtubeTrack = new LavalinkTrack
            {
                SourceName = "youtube",
                Identifier = video.Id,
                IsSeekable = seekable,
                IsLiveStream = liveStream,
                Title = video.Title,
                Author = video.Author.ToString(),
                StartPosition = TimeSpan.Zero,
                Duration = (TimeSpan) video.Duration!,
                Uri = new Uri(video.Url),
                ArtworkUri = new Uri(video.Thumbnails.FirstOrDefault()?.Url!),
            };

            if (youtubeTrack.IsLiveStream)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId))
            {
                Bot.GuildData.Add(guildId, new GuildData());
            }

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            await player.PlayAsync(youtubeTrack);

            if (player.Queue.IsEmpty)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongInformation(youtubeTrack, player))));

                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.SongAddedToQueue(youtubeTrack)));
        }
    }

    public async Task PlayFromSpotify(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var searchResult = await _audioService.Tracks.SearchAsync(
            query,
            loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
            categories: ImmutableArray.Create(SearchCategory.Track));

        if (searchResult == null)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.AudioTrackError(context)));
            return;
        }
        
        var spotifyTrack = new ExtendedLavalinkTrack(searchResult!.Tracks[0]);

        if (spotifyTrack.IsLiveStream)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.LiveSteamError(context)));

            return;
        }

        if (!Bot.GuildData.ContainsKey(guildId))
        {
            Bot.GuildData.Add(guildId, new GuildData());
        }

        GuildData = Bot.GuildData[guildId];
        GuildData.TextChannelId = context.Channel.Id;

        await player!.PlayAsync(spotifyTrack.Track);

        if (player.Queue.IsEmpty)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongInformation(spotifyTrack, player))));
            return;
        }

        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(AudioPlayerEmbed.SongAddedToQueue(spotifyTrack)));
    }

    public async Task PlayFromSoundCloud(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var soundcloudTrack = await _audioService.Tracks.LoadTrackAsync(query!, TrackSearchMode.SoundCloud);

        if (soundcloudTrack == null)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.AudioTrackError(context)));
            return;
        }
        
        if (soundcloudTrack.IsLiveStream)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.LiveSteamError(context)));
            return;
        }

        if (!Bot.GuildData.ContainsKey(guildId))
        {
            Bot.GuildData.Add(guildId, new GuildData());
        }

        GuildData = Bot.GuildData[guildId];
        GuildData.TextChannelId = context.Channel.Id;

        await player.PlayAsync(soundcloudTrack!);

        if (player.Queue.IsEmpty)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongInformation(soundcloudTrack, player))));
            return;
        }

        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(AudioPlayerEmbed.SongAddedToQueue(soundcloudTrack)));
    }
}