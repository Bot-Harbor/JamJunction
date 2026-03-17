using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Platforms.Enums;
using JamJunction.App.Lavalink.Platforms.Interfaces;
using JamJunction.App.Models;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;

namespace JamJunction.App.Lavalink.Platforms;

/// <summary>
/// Provides SoundCloud platform integration for Jam Junction.
/// </summary>
/// <remarks>
/// This implementation of <see cref="IPlatform"/> resolves SoundCloud
/// tracks, playlists, and sets and sends them to the
/// <see cref="QueuedLavalinkPlayer"/> for playback through Lavalink.
/// </remarks>
public class SoundCloudPlatform : IPlatform
{
    /// <summary>
    /// Provides access to the Lavalink audio service used for managing
    /// audio playback and retrieving player instances.
    /// </summary>
    /// <remarks>
    /// This service is used to interact with Lavalink through Lavalink4NET,
    /// allowing the application to control music playback, queues, filters,
    /// and other audio-related functionality.
    /// </remarks>
    private readonly IAudioService _audioService;

    public SoundCloudPlatform(IAudioService audioService)
    {
        _audioService = audioService;
    }
    
    /// <summary>
    /// Gets or sets the platform type handled by this implementation.
    /// </summary>
    public Platform Platform { get; set; } = Platform.SoundCloud;
    
    /// <summary>
    /// Gets or sets the base URL used to identify SoundCloud queries.
    /// </summary>
    /// <remarks>
    /// This value helps determine whether a user query should be
    /// processed by the SoundCloud platform handler.
    /// </remarks>
    public string Url { get; set; } = "soundcloud.com";
    
    /// <summary>
    /// Stores guild-specific data related to the current playback session.
    /// </summary>
    private GuildData GuildData { get; set; }
    
    /// <summary>
    /// Provides embed builders used to display player status
    /// and queue updates.
    /// </summary>
    private AudioPlayerEmbed AudioPlayerEmbed { get; } = new();
    
    /// <summary>
    /// Provides embed builders used to display playback errors
    /// and invalid request messages.
    /// </summary>
    private ErrorEmbed ErrorEmbed { get; } = new();
    
    /// <summary>
    /// Stores the Discord message that displays the current player
    /// information within the guild text channel.
    /// </summary>
    private DiscordMessage DiscordMessage { get; set; }
    
    /// <summary>
    /// Resolves a SoundCloud query or URL and sends the resulting track,
    /// playlist, or set to the Lavalink player.
    /// </summary>
    /// <param name="player">
    /// The <see cref="QueuedLavalinkPlayer"/> responsible for managing
    /// playback and the queue.
    /// </param>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information
    /// about the Discord interaction that initiated the request.
    /// </param>
    /// <param name="query">
    /// The SoundCloud URL or search query used to locate the track
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
    /// - Individual SoundCloud tracks
    /// - SoundCloud playlists and sets
    /// - URLs containing query parameters
    ///
    /// Tracks are converted into <see cref="LavalinkTrack"/> instances
    /// before being added to the player's queue.
    /// </remarks>
    public async Task PlayTrack(QueuedLavalinkPlayer player, InteractionContext context, string query,
        bool queueNext = false)
    {
        var trackQueueItems = new List<ITrackQueueItem>();
        
        var channel = context.Channel;
        var guildId = context.Guild.Id;

        if (query.Contains("soundcloud.com") && query.Contains("/sets"))
        {
            if (query.Contains("in=user"))
            {
                var searchUrl = query.Split('?')[0];

                var soundcloudTrack = await _audioService.Tracks.LoadTrackAsync(searchUrl!, TrackSearchMode.SoundCloud);

                if (soundcloudTrack == null)
                {
                    var errorMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder()
                            .AddEmbed(ErrorEmbed.AudioTrackError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteMessageAsync(errorMessage);
                    return;
                }

                if (soundcloudTrack.IsLiveStream)
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
                    await player.Queue.InsertAsync(0, new TrackQueueItem(soundcloudTrack));
                }
                else
                {
                    await player.PlayAsync(soundcloudTrack);
                }

                if (player.Queue.IsEmpty)
                {
                    await player.SetVolumeAsync(.50f);
                    DiscordMessage = await context
                        .FollowUpAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                AudioPlayerEmbed.TrackInformation(soundcloudTrack, player))));
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
                        .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(soundcloudTrack)));

                await Task.Delay(10000);
                _ = context.DeleteFollowupAsync(DiscordMessage.Id);

                return;
            }

            var trackLoadResult = await _audioService.Tracks.LoadTracksAsync(query!, TrackSearchMode.SoundCloud);

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

                var soundCloudTrack = new LavalinkTrack
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
                    ArtworkUri = new Uri(track.ArtworkUri!.ToString())
                };

                if (soundCloudTrack.IsLiveStream)
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

                trackQueueItems.Add(new TrackQueueItem(soundCloudTrack));
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
                        .AddEmbed(ErrorEmbed.AudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteMessageAsync(errorMessage);
                return;
            }

            if (soundcloudTrack.IsLiveStream)
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
                await player.Queue.InsertAsync(0, new TrackQueueItem(soundcloudTrack));
            }
            else
            {
                await player.PlayAsync(soundcloudTrack);
            }

            if (player.Queue.IsEmpty)
            {
                await player.SetVolumeAsync(.50f);
                DiscordMessage = await context
                    .FollowUpAsync(new DiscordFollowupMessageBuilder(
                        new DiscordInteractionResponseBuilder(
                            AudioPlayerEmbed.TrackInformation(soundcloudTrack, player))));
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
                    .AddEmbed(AudioPlayerEmbed.TrackAddedToQueue(soundcloudTrack)));

            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(DiscordMessage.Id);
        }
    }
}