using System.Net;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Platforms.Enums;
using JamJunction.App.Lavalink.Platforms.Interfaces;
using JamJunction.App.Models;
using JamJunction.App.Secrets;
using JamJunction.App.Views.Embeds;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Tracks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace JamJunction.App.Lavalink.Platforms;

/// <summary>
/// Provides YouTube platform integration for Jam Junction.
/// </summary>
/// <remarks>
/// This implementation of <see cref="IPlatform"/> resolves YouTube videos,
/// playlists, and search queries using the YoutubeExplode library and
/// queues the resulting tracks into the <see cref="QueuedLavalinkPlayer"/>
/// for playback through Lavalink.
/// </remarks>
public class YoutubePlatform : IPlatform
{
    /// <summary>
    /// Gets or sets the platform type handled by this implementation.
    /// </summary>
    /// <remarks>
    /// This value identifies the platform as YouTube within the
    /// Jam Junction platform handling system.
    /// </remarks>
    public Platform Platform { get; set; } = Platform.YouTube;
    
    /// <summary>
    /// Gets or sets the base URL used to identify YouTube queries.
    /// </summary>
    /// <remarks>
    /// Queries containing this value will be routed to the YouTube
    /// platform handler.
    /// </remarks>
    public string Url { get; set; } = "youtube.com";
    
    /// <summary>
    /// Stores guild-specific playback information such as the player
    /// message and associated text channel.
    /// </summary>
    private GuildData GuildData { get; set; }

    /// <summary>
    /// Provides embed builders used for displaying audio player
    /// information and queue updates.
    /// </summary>
    private AudioPlayerEmbed AudioPlayerEmbed { get; } = new();
    
    /// <summary>
    /// Provides embed builders used for displaying error messages
    /// related to playback or track loading.
    /// </summary>
    private ErrorEmbed ErrorEmbed { get; } = new();

    /// <summary>
    /// Stores the Discord message used to display the audio player
    /// interface within the guild.
    /// </summary>
    private DiscordMessage DiscordMessage { get; set; }

    /// <summary>
    /// Resolves a YouTube query or URL and sends the resulting track
    /// or playlist to the Lavalink player.
    /// </summary>
    /// <param name="player">
    /// The <see cref="QueuedLavalinkPlayer"/> responsible for managing
    /// playback and the queue.
    /// </param>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command interaction.
    /// </param>
    /// <param name="query">
    /// The YouTube URL or search query used to locate the video
    /// or playlist.
    /// </param>
    /// <param name="queueNext">
    /// Indicates whether the track should be inserted next in the queue
    /// instead of being appended to the end.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous playback operation.
    /// </returns>
    /// <remarks>
    /// This method supports:
    /// - YouTube video URLs
    /// - YouTube playlist URLs
    /// - YouTube search queries
    ///
    /// Tracks are retrieved using the YoutubeExplode library and converted
    /// into <see cref="LavalinkTrack"/> instances before being added to the
    /// player's queue.
    /// </remarks>
    public async Task PlayTrack(QueuedLavalinkPlayer player, InteractionContext context, string query,
        bool queueNext = false)
    {
        var address = ProxySecrets.Address;
        var username = ProxySecrets.Username;
        var password = ProxySecrets.Password;

        var proxy = new WebProxy(address)
        {
            Credentials = new NetworkCredential(username, password)
        };

        var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true
        };

        var httpClient = new HttpClient(handler);
        var youtube = new YoutubeClient(httpClient);

        var trackQueueItems = new List<ITrackQueueItem>();

        var channel = context.Channel;
        var guildId = context.Guild.Id;

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
                catch (Exception e)
                {
                    playlist = null;
                    playlistData = null;
                    Console.WriteLine(e);
                }

                if (playlist == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var video in playlist.Take(100))
                {
                    if (player.Queue.Count >= 100) break;

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
                                .AddEmbed(ErrorEmbed.LiveSteamError()));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    trackQueueItems.Add(new TrackQueueItem(youtubeVideo));
                }

                for (var i = 0; i < trackQueueItems.Count; i++)
                {
                    if (player.Queue.Count >= 100) break;

                    if (queueNext)
                    {
                        await player.Queue.InsertAsync(i, trackQueueItems[i]);
                    }
                    else
                    {
                        await player.Queue.AddAsync(trackQueueItems[i]);
                    }
                }

                if (GuildData.FirstSongInQueue)
                {
                    var firstTrack = player.Queue.FirstOrDefault()!.Track;
                    await player.PlayAsync(firstTrack!, false);
                    await player.Queue.RemoveAtAsync(0);
                    await player.SetVolumeAsync(.50f);

                    try
                    {
                        DiscordMessage = await context
                            .FollowUpAsync(new DiscordFollowupMessageBuilder(
                                new DiscordInteractionResponseBuilder(
                                    AudioPlayerEmbed.TrackInformation(firstTrack, player))));
                        GuildData.PlayerMessage = DiscordMessage;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    return;
                }

                try
                {
                    var updatedPlayerMessage = await channel.GetMessageAsync(GuildData.PlayerMessage.Id);
                    await updatedPlayerMessage.ModifyAsync(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
                }
                catch (Exception e)
                {
                    GuildData.PlayerMessage =
                        await context.FollowUpAsync(
                            new DiscordFollowupMessageBuilder(
                                AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));
                    Console.WriteLine(e);
                }

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
                catch (Exception e)
                {
                    video = null;
                    Console.WriteLine(e);
                }

                if (video == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError()));
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
                            .AddEmbed(ErrorEmbed.LiveSteamError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                GuildData = Bot.GuildData[guildId];
                GuildData.TextChannelId = context.Channel.Id;

                if (queueNext)
                {
                    await player.Queue.InsertAsync(0, new TrackQueueItem(youtubeVideo));
                }
                else
                {
                    await player.PlayAsync(youtubeVideo);
                }

                if (player.Queue.IsEmpty)
                {
                    await player.SetVolumeAsync(.50f);
                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(youtubeVideo, player))));
                    GuildData.PlayerMessage = DiscordMessage;
                    return;
                }

                try
                {
                    var updatedPlayerMessage = await channel.GetMessageAsync(GuildData.PlayerMessage.Id);
                    await updatedPlayerMessage.ModifyAsync(
                        AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
                }
                catch (Exception e)
                {
                    GuildData.PlayerMessage =
                        await context.FollowUpAsync(
                            new DiscordFollowupMessageBuilder(
                                AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));
                    Console.WriteLine(e);
                }

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
                videos = await youtube.Search.GetVideosAsync(query).CollectAsync(1);
            }
            catch (Exception e)
            {
                videos = null;
                Console.WriteLine(e);
            }

            if (videos == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
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
                        .AddEmbed(ErrorEmbed.LiveSteamError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

            GuildData = Bot.GuildData[guildId];
            GuildData.TextChannelId = context.Channel.Id;

            if (queueNext)
            {
                await player.Queue.InsertAsync(0, new TrackQueueItem(youtubeVideo));
            }
            else
            {
                await player.PlayAsync(youtubeVideo);
            }

            if (player.Queue.IsEmpty)
            {
                await player.SetVolumeAsync(.50f);
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(youtubeVideo, player))));
                GuildData.PlayerMessage = DiscordMessage;
                return;
            }

            try
            {
                var updatedPlayerMessage = await channel.GetMessageAsync(GuildData.PlayerMessage.Id);
                await updatedPlayerMessage.ModifyAsync(
                    AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
            }
            catch (Exception e)
            {
                GuildData.PlayerMessage =
                    await context.FollowUpAsync(
                        new DiscordFollowupMessageBuilder(
                            AudioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));
                Console.WriteLine(e);
            }

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(youtubeVideo)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }
}