using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AngleSharp.Common;
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
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using Playlist = YoutubeExplode.Playlists.Playlist;

namespace JamJunction.App.Lavalink;

public class PlatformHandler
{
    private readonly IAudioService _audioService;

    public PlatformHandler(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private GuildData GuildData { get; set; }
    private AudioPlayerEmbed AudioPlayerEmbed { get; } = new();
    private ErrorEmbed ErrorEmbed { get; } = new();
    private DiscordMessage DiscordMessage { get; set; }

    public async Task PlayFromSpotify(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(
            new ClientCredentialsAuthenticator(SpotifySecrets.ClientId, SpotifySecrets.ClientSecret));

        var spotify = new SpotifyClient(config);

        var channel = context.Channel;

        if (query.Contains("spotify.com"))
        {
            if (query.Contains("/album"))
            {
                FullAlbum fullAlbum;

                try
                {
                    var albumId = Regex.Match(query, @"(?<=album/)[^?]+").Value;
                    fullAlbum = await spotify.Albums.Get(albumId);
                }
                catch (Exception)
                {
                    fullAlbum = null;
                }

                if (fullAlbum == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var track in fullAlbum.Tracks.Items!.Take(25))
                {
                    if (player.Queue.Count >= 25)
                    {
                        break;
                    }

                    var seekable = true;
                    var liveStream = false;

                    if (track.DurationMs == 0)
                    {
                        seekable = false;
                        liveStream = true;
                    }

                    var author = track.Artists.FirstOrDefault()!.Name;
                    var duration = TimeSpan.FromMilliseconds(track.DurationMs);
                    var uri = $"https://open.spotify.com/track/{track.Id}";
                    var artworkUri = fullAlbum.Images.FirstOrDefault()!.Url;

                    var spotifyTrack = new LavalinkTrack()
                    {
                        SourceName = "spotify",
                        Identifier = track.Id,
                        IsSeekable = seekable,
                        IsLiveStream = liveStream,
                        Title = track.Name,
                        Author = author,
                        StartPosition = TimeSpan.Zero,
                        Duration = duration,
                        Uri = new Uri(uri),
                        ArtworkUri = new Uri(artworkUri)
                    };

                    if (spotifyTrack.IsLiveStream)
                    {
                        var errorMessage = await context
                            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                                .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    await player.Queue.AddAsync(new TrackQueueItem(spotifyTrack));
                }

                if (GuildData.FirstSongInQueue)
                {
                    var firstTrack = player.Queue.FirstOrDefault()!.Track;
                    await player.PlayAsync(firstTrack!, false);
                    await player.Queue.RemoveAtAsync(0);

                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(firstTrack, player))));
                    GuildData.Message = DiscordMessage;
                    return;
                }

                _ = context.Channel.DeleteMessageAsync(GuildData.Message);

                var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                GuildData.Message = guildMessage;

                var albumUrl = $"https://open.spotify.com/album/{fullAlbum.Id}";
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(AudioPlayerEmbed
                            .AlbumAddedToQueue(fullAlbum, albumUrl)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
            else if (query.Contains("/playlist"))
            {
                FullPlaylist fullPlaylist;
                
                try
                {
                    var playlistId = Regex.Match(query, @"(?<=playlist/)[^?]+").Value;
                    fullPlaylist = await spotify.Playlists.Get(playlistId);
                }
                catch (Exception)
                {
                    fullPlaylist = null;
                }
                
                if (fullPlaylist == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var item in fullPlaylist.Tracks!.Items!.Take(25))
                {
                    var track = item.Track as FullTrack;
                    
                    if (player.Queue.Count >= 25)
                    {
                        break;
                    }

                    var seekable = true;
                    var liveStream = false;

                    if (track!.DurationMs == 0)
                    {
                        seekable = false;
                        liveStream = true;
                    }

                    var author = track.Artists.FirstOrDefault()!.Name;
                    var duration = TimeSpan.FromMilliseconds(track.DurationMs);
                    var uri = $"https://open.spotify.com/track/{track.Id}";
                    var artworkUri = track.Album.Images.FirstOrDefault()!.Url;

                    var spotifyTrack = new LavalinkTrack()
                    {
                        SourceName = "spotify",
                        Identifier = track.Id,
                        IsSeekable = seekable,
                        IsLiveStream = liveStream,
                        Title = track.Name,
                        Author = author,
                        StartPosition = TimeSpan.Zero,
                        Duration = duration,
                        Uri = new Uri(uri),
                        ArtworkUri = new Uri(artworkUri)
                    };

                    if (spotifyTrack.IsLiveStream)
                    {
                        var errorMessage = await context
                            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                                .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    await player.Queue.AddAsync(new TrackQueueItem(spotifyTrack));
                }

                if (GuildData.FirstSongInQueue)
                {
                    var firstTrack = player.Queue.FirstOrDefault()!.Track;
                    await player.PlayAsync(firstTrack!, false);
                    await player.Queue.RemoveAtAsync(0);

                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(firstTrack, player))));
                    GuildData.Message = DiscordMessage;
                    return;
                }

                _ = context.Channel.DeleteMessageAsync(GuildData.Message);

                var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                GuildData.Message = guildMessage;

                var playlistUrl = $"https://open.spotify.com/playlist/{fullPlaylist.Id}";
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(AudioPlayerEmbed
                            .PlaylistAddedToQueue(fullPlaylist, playlistUrl)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
            else
            {
                FullTrack fullTrack;

                try
                {
                    var trackId = Regex.Match(query, @"(?<=track/)[^?]+").Value;
                    fullTrack = await spotify.Tracks.Get(trackId);
                }
                catch (Exception)
                {
                    fullTrack = null;
                }

                if (fullTrack == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
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

                var spotifyTrack = new LavalinkTrack
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
                    ArtworkUri = new Uri(artworkUri)
                };

                if (spotifyTrack.IsLiveStream)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                GuildData = Bot.GuildData[guildId];
                GuildData.TextChannelId = context.Channel.Id;

                await player.PlayAsync(spotifyTrack);

                if (player.Queue.IsEmpty)
                {
                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(spotifyTrack, player))));
                    GuildData.Message = DiscordMessage;
                    return;
                }

                _ = context.Channel.DeleteMessageAsync(GuildData.Message);

                var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                GuildData.Message = guildMessage;

                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(spotifyTrack)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
        }
        else
        {
            var searchResult = await _audioService.Tracks.SearchAsync(
                query,
                loadOptions: new TrackLoadOptions(TrackSearchMode.Spotify),
                categories: ImmutableArray.Create(SearchCategory.Track));

            if (searchResult == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            var spotifyTrack = new ExtendedLavalinkTrack(searchResult!.Tracks[0]);

            if (spotifyTrack.IsLiveStream)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            await player!.PlayAsync(spotifyTrack.Track);

            if (player.Queue.IsEmpty)
            {
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(spotifyTrack, player))));
                GuildData.Message = DiscordMessage;
                return;
            }

            _ = context.Channel.DeleteMessageAsync(GuildData.Message);

            var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(
                    AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

            GuildData.Message = guildMessage;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(spotifyTrack)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }

    public async Task PlayFromYoutube(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var youtube = new YoutubeClient();

        var channel = context.Channel;

        if (query.Contains("youtube.com"))
        {
            if (query.Contains("/playlist"))
            {
                IEnumerable<PlaylistVideo> playlist;
                Playlist playlistData;

                try
                {
                    playlist = await youtube.Playlists.GetVideosAsync(query);
                    playlistData = await youtube.Playlists.GetAsync(query);
                }
                catch (Exception)
                {
                    playlist = null;
                    playlistData = null;
                }

                if (playlist == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var video in playlist.Take(25))
                {
                    if (player.Queue.Count >= 25)
                    {
                        break;
                    }

                    var seekable = true;
                    var liveStream = false;

                    if (video.Duration == TimeSpan.Zero)
                    {
                        seekable = false;
                        liveStream = true;
                    }

                    var artworkUri = video.Thumbnails.FirstOrDefault()?.Url!;

                    var youtubeVideo = new LavalinkTrack
                    {
                        SourceName = "youtube",
                        Identifier = video.Id,
                        IsSeekable = seekable,
                        IsLiveStream = liveStream,
                        Title = video.Title,
                        Author = video.Author.ToString(),
                        StartPosition = TimeSpan.Zero,
                        Duration = (TimeSpan)video.Duration!,
                        Uri = new Uri(video.Url),
                        ArtworkUri = new Uri(artworkUri!)
                    };

                    if (youtubeVideo.IsLiveStream)
                    {
                        var errorMessage = await context
                            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                                .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    await player.Queue.AddAsync(new TrackQueueItem(youtubeVideo));
                }

                if (GuildData.FirstSongInQueue)
                {
                    var firstTrack = player.Queue.FirstOrDefault()!.Track;
                    await player.PlayAsync(firstTrack!, false);
                    await player.Queue.RemoveAtAsync(0);

                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(firstTrack, player))));
                    GuildData.Message = DiscordMessage;
                    return;
                }

                _ = context.Channel.DeleteMessageAsync(GuildData.Message);

                var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                GuildData.Message = guildMessage;

                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(AudioPlayerEmbed
                            .PlaylistAddedToQueue(playlistData)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
            else
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
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
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

                var youtubeVideo = new LavalinkTrack
                {
                    SourceName = "youtube",
                    Identifier = video.Id,
                    IsSeekable = seekable,
                    IsLiveStream = liveStream,
                    Title = video.Title,
                    Author = video.Author.ToString(),
                    StartPosition = TimeSpan.Zero,
                    Duration = (TimeSpan)video.Duration!,
                    Uri = new Uri(video.Url),
                    ArtworkUri = new Uri(artworkUri!)
                };

                if (youtubeVideo.IsLiveStream)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                GuildData = Bot.GuildData[guildId];
                GuildData.TextChannelId = context.Channel.Id;

                await player.PlayAsync(youtubeVideo);

                if (player.Queue.IsEmpty)
                {
                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(youtubeVideo, player))));
                    GuildData.Message = DiscordMessage;
                    return;
                }

                _ = context.Channel.DeleteMessageAsync(GuildData.Message);

                var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                GuildData.Message = guildMessage;

                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeVideo)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
        }
        else
        {
            IEnumerable<VideoSearchResult> videos;

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
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                ;
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

            var youtubeVideo = new LavalinkTrack
            {
                SourceName = "youtube",
                Identifier = video.Id,
                IsSeekable = seekable,
                IsLiveStream = liveStream,
                Title = video.Title,
                Author = video.Author.ToString(),
                StartPosition = TimeSpan.Zero,
                Duration = (TimeSpan)video.Duration!,
                Uri = new Uri(video.Url),
                ArtworkUri = new Uri(artworkUri!)
            };

            if (youtubeVideo.IsLiveStream)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            await player.PlayAsync(youtubeVideo);

            if (player.Queue.IsEmpty)
            {
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(youtubeVideo, player))));
                GuildData.Message = DiscordMessage;
                return;
            }

            _ = context.Channel.DeleteMessageAsync(GuildData.Message);

            var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(
                    AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

            GuildData.Message = guildMessage;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeVideo)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }

    public async Task PlayFromSoundCloud(QueuedLavalinkPlayer player, string query,
        InteractionContext context, ulong guildId)
    {
        var channel = context.Channel;

        if (query.Contains("soundcloud.com") && query.Contains("/sets")) // Change to /playlists or something
        {
            var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query!, TrackSearchMode.SoundCloud);

            if (trackLoadResult.Playlist == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            foreach (var track in trackLoadResult.Tracks.Take(25))
            {
                if (player.Queue.Count >= 25)
                {
                    break;
                }

                var seekable = true;
                var liveStream = false;

                if (track.Duration.Hours == 0 && track.Duration is { Minutes: 0, Seconds: 0 })
                {
                    seekable = false;
                    liveStream = true;
                }

                var soundCloudTrack = new LavalinkTrack()
                {
                    SourceName = "soundcloud",
                    Identifier = track.Identifier,
                    IsSeekable = seekable,
                    IsLiveStream = liveStream,
                    Title = track.Title,
                    Author = track.Author,
                    StartPosition = TimeSpan.Zero,
                    Duration = track.Duration,
                    Uri = new Uri(track.Uri!.ToString()),
                    ArtworkUri = new Uri(track.ArtworkUri!.ToString()),
                };

                if (soundCloudTrack.IsLiveStream)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                GuildData = Bot.GuildData[guildId];
                GuildData.TextChannelId = context.Channel.Id;

                await player.Queue.AddAsync(new TrackQueueItem(soundCloudTrack));
            }

            if (GuildData.FirstSongInQueue)
            {
                var firstTrack = player.Queue.FirstOrDefault()!.Track;
                await player.PlayAsync(firstTrack!, false);
                await player.Queue.RemoveAtAsync(0);

                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(firstTrack, player))));
                GuildData.Message = DiscordMessage;
                return;
            }

            _ = context.Channel.DeleteMessageAsync(GuildData.Message);

            var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(
                    AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

            GuildData.Message = guildMessage;

            var playlistUrl = query;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed
                        .PlaylistAddedToQueue(trackLoadResult, playlistUrl)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
        else
        {
            var soundcloudTrack = await _audioService.Tracks.LoadTrackAsync(query!, TrackSearchMode.SoundCloud);

            if (soundcloudTrack == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (soundcloudTrack.IsLiveStream)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.LiveSteamError(context)));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            await player.PlayAsync(soundcloudTrack!);

            if (player.Queue.IsEmpty)
            {
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(soundcloudTrack, player))));
                GuildData.Message = DiscordMessage;
                return;
            }

            _ = context.Channel.DeleteMessageAsync(GuildData.Message);

            var guildMessage = await context.FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(
                    AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

            GuildData.Message = guildMessage;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(soundcloudTrack)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }
}