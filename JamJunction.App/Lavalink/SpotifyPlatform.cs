using System.Collections.Immutable;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Enums;
using JamJunction.App.Lavalink.Interfaces;
using JamJunction.App.Models;
using JamJunction.App.Secrets;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using SpotifyAPI.Web;

namespace JamJunction.App.Lavalink;

public class SpotifyPlatform : IPlatform
{
    private readonly IAudioService _audioService;

    public SpotifyPlatform(IAudioService audioService)
    {
        _audioService = audioService;
    }
    
    private GuildData GuildData { get; set; }

    private AudioPlayerEmbed AudioPlayerEmbed { get; } = new();
    private ErrorEmbed ErrorEmbed { get; } = new();

    private DiscordMessage DiscordMessage { get; set; }

    public async Task PlayTrack(QueuedLavalinkPlayer player, InteractionContext context, string query,
        bool queueNext = false)
    {
        var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(
            new ClientCredentialsAuthenticator(SpotifySecrets.ClientId, SpotifySecrets.ClientSecret));
        var spotify = new SpotifyClient(config);

        var trackQueueItems = new List<ITrackQueueItem>();

        var channel = context.Channel;
        var guildId = context.Guild.Id;

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
                catch (Exception e)
                {
                    fullAlbum = null;
                    Console.WriteLine(e);
                }

                if (fullAlbum == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var track in fullAlbum.Tracks.Items!.Take(100))
                {
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

                    var spotifyTrack = new LavalinkTrack
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
                                .AddEmbed(ErrorEmbed.LiveSteamError()));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    trackQueueItems.Add(new TrackQueueItem(spotifyTrack));
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

                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(firstTrack, player))));
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
                catch (Exception e)
                {
                    fullPlaylist = null;
                    Console.WriteLine(e);
                }

                if (fullPlaylist == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                foreach (var item in fullPlaylist.Tracks!.Items!.Take(100))
                {
                    var track = item.Track as FullTrack;

                    if (player.Queue.Count >= 100) break;

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

                    var spotifyTrack = new LavalinkTrack
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
                                .AddEmbed(ErrorEmbed.LiveSteamError()));
                        await Task.Delay(10000);
                        _ = channel.DeleteMessageAsync(errorMessage);
                        return;
                    }

                    if (!Bot.GuildData.ContainsKey(guildId)) Bot.GuildData.Add(guildId, new GuildData());

                    GuildData = Bot.GuildData[guildId];
                    GuildData.TextChannelId = context.Channel.Id;

                    trackQueueItems.Add(new TrackQueueItem(spotifyTrack));
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

                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(firstTrack, player))));
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
                catch (Exception e)
                {
                    fullTrack = null;
                    Console.WriteLine(e);
                }

                if (fullTrack == null)
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
                    await player.Queue.InsertAsync(0, new TrackQueueItem(spotifyTrack));
                }
                else
                {
                    await player.PlayAsync(spotifyTrack);
                }

                if (player.Queue.IsEmpty)
                {
                    await player.SetVolumeAsync(.50f);
                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(spotifyTrack, player))));
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
                        .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(spotifyTrack)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);
            }
        }
        else
        {
            var spotifyTrack = await _audioService.Tracks.LoadTrackAsync(query!, TrackSearchMode.Spotify);

            if (spotifyTrack == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (spotifyTrack.IsLiveStream)
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
                await player.Queue.InsertAsync(0, new TrackQueueItem(spotifyTrack));
            }
            else
            {
                await player.PlayAsync(spotifyTrack);
            }

            if (player.Queue.IsEmpty)
            {
                await player.SetVolumeAsync(.50f);
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(spotifyTrack, player))));
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
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(spotifyTrack)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }
}