using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Enums;
using JamJunction.App.Lavalink.Interfaces;
using JamJunction.App.Models;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace JamJunction.App.Lavalink;

public class DeezerPlatform : IPlatform
{
    private readonly IAudioService _audioService;

    public DeezerPlatform(IAudioService audioService)
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
        LavalinkTrack deezerTrack;

        var trackQueueItems = new List<ITrackQueueItem>();
        
        var channel = context.Channel;
        var guildId = context.Guild.Id;

        if (query.Contains("deezer.com") && query.Contains("album"))
        {
            var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query!, TrackSearchMode.Deezer);

            if (trackLoadResult.Playlist == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            var result = await _audioService.Tracks.LoadTracksAsync(trackLoadResult.Tracks[0].Uri!.ToString(),
                new TrackLoadOptions(TrackSearchMode.None));
            var artworkUri = result.Track!.ArtworkUri!.ToString();

            foreach (var track in trackLoadResult.Tracks.Take(100))
            {
                if (player.Queue.Count >= 100) break;

                var seekable = true;
                var liveStream = false;

                if (track.Duration.Hours == 0 && track.Duration is { Minutes: 0, Seconds: 0 })
                {
                    seekable = false;
                    liveStream = true;
                }

                deezerTrack = new LavalinkTrack()
                {
                    SourceName = "deezer",
                    Identifier = track.Identifier,
                    IsSeekable = seekable,
                    IsLiveStream = liveStream,
                    Title = track.Title,
                    Author = track.Author,
                    StartPosition = TimeSpan.Zero,
                    Duration = track.Duration,
                    Uri = new Uri(track.Uri!.ToString()),
                    ArtworkUri = new Uri(artworkUri)
                };

                if (deezerTrack.IsLiveStream)
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

                trackQueueItems.Add(new TrackQueueItem(deezerTrack));
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

            var albumUrl = query;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed
                        .AlbumAddedToQueue(trackLoadResult, albumUrl)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);

            return;
        }

        if (query.Contains("deezer.com") && query.Contains("playlist"))
        {
            var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query!, TrackSearchMode.Deezer);

            if (trackLoadResult.Playlist == null)
            {
                var errorMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder()
                        .AddEmbed(ErrorEmbed.AudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            foreach (var track in trackLoadResult.Tracks.Take(100))
            {
                if (player.Queue.Count >= 100) break;

                var seekable = true;
                var liveStream = false;

                if (track.Duration.Hours == 0 && track.Duration is { Minutes: 0, Seconds: 0 })
                {
                    seekable = false;
                    liveStream = true;
                }

                deezerTrack = new LavalinkTrack()
                {
                    SourceName = "deezer",
                    Identifier = track.Identifier,
                    IsSeekable = seekable,
                    IsLiveStream = liveStream,
                    Title = track.Title,
                    Author = track.Author,
                    StartPosition = TimeSpan.Zero,
                    Duration = track.Duration,
                    Uri = new Uri(track.Uri!.ToString()),
                    ArtworkUri = new Uri(track.ArtworkUri!.ToString())
                };

                if (deezerTrack.IsLiveStream)
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

                trackQueueItems.Add(new TrackQueueItem(deezerTrack));
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

            var albumUrl = query;

            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed
                        .PlaylistAddedToQueue(trackLoadResult, albumUrl)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);

            return;
        }

        deezerTrack = await _audioService.Tracks.LoadTrackAsync(query!, TrackSearchMode.Deezer);

        if (deezerTrack == null)
        {
            var errorMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.AudioTrackError()));
            await Task.Delay(10000);
            _ = channel.DeleteMessageAsync(errorMessage);
            return;
        }

        if (deezerTrack.IsLiveStream)
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
            await player.Queue.InsertAsync(0, new TrackQueueItem(deezerTrack));
        }
        else
        {
            await player.PlayAsync(deezerTrack);
        }

        if (player.Queue.IsEmpty)
        {
            await player.SetVolumeAsync(.50f);
            DiscordMessage = await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        AudioPlayerEmbed.TrackInformation(deezerTrack, player))));
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
                .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(deezerTrack)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(DiscordMessage.Id);
    }
}