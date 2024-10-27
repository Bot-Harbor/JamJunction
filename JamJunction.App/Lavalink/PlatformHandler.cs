﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Secrets;
using Lavalink4NET;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using SpotifyAPI.Web;
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
    
    public async Task PlayFromSpotify(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        if (query.Contains("spotify.com"))
        {
            FullTrack fullTrack;

            try
            {
                var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(
                    new ClientCredentialsAuthenticator(SpotifySecrets.ClientId, SpotifySecrets.ClientSecret));

                var spotify = new SpotifyClient(config);
                
                var trackId = Regex.Match(query, @"(?<=track/)[^?]+").Value;
                fullTrack = await spotify.Tracks.Get(trackId);
            }
            catch (Exception)
            {
                fullTrack = null;
            }

            if (fullTrack == null)
            {
                await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                return;
            }

            var seekable = true;
            var liveStream = false;

            if (fullTrack.DurationMs == 0)
            {
                seekable = false;
                liveStream = true;
            }

            var author = fullTrack.Artists.FirstOrDefault()!.Name;
            var duration = TimeSpan.FromMilliseconds(fullTrack.DurationMs);
            var uri = $"https://open.spotify.com/track/{fullTrack.Id}";
            var artworkUri = fullTrack.Album.Images.FirstOrDefault()!.Url;

            var youtubeTrack = new LavalinkTrack
            {
                SourceName = "spotify",
                Identifier = fullTrack.Id,
                IsSeekable = seekable,
                IsLiveStream = liveStream,
                Title = fullTrack.Name,
                Author = author,
                StartPosition = TimeSpan.Zero,
                Duration = duration,
                Uri = new Uri(uri),
                ArtworkUri = new Uri(artworkUri),
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
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.TrackInformation(youtubeTrack, player))));

                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeTrack)));
        }
        else
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
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.TrackInformation(spotifyTrack, player))));
                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(spotifyTrack)));
        }
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

            var artworkUri = video.Thumbnails.FirstOrDefault()?.Url!;

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
                ArtworkUri = new Uri(artworkUri!),
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
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.TrackInformation(youtubeTrack, player))));

                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeTrack)));
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

            var artworkUri = video.Thumbnails.FirstOrDefault()?.Url!;

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
                ArtworkUri = new Uri(artworkUri!),
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
                        new DiscordInteractionResponseBuilder(AudioPlayerEmbed.TrackInformation(youtubeTrack, player))));

                return;
            }

            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeTrack)));
        }
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
                    new DiscordInteractionResponseBuilder(AudioPlayerEmbed.TrackInformation(soundcloudTrack, player))));
            return;
        }

        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(soundcloudTrack)));
    }
}