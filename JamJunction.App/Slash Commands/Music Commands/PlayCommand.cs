using System.Collections.Immutable;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;
    private GuildData GuildData { get; set; }
    private AudioPlayerEmbed AudioPlayerEmbed { get; set; } = new();
    private ErrorEmbed ErrorEmbed { get; set; } = new();

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
        [Option("Platform", "Pick a streaming platform (Default: Youtube).")]
        Platform streamingPlatform = default
    )
    {
        await context.DeferAsync();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;
        
        if (userVoiceChannel == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    ErrorEmbed.ValidVoiceChannelError(context)));

            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player = await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, connectToVoiceChannel: true);

        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    ErrorEmbed.NoConnectionError(context)));

            return;
        }

        var botId = context.Client.CurrentUser.Id;
        context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (userVoiceChannel.Id != botVoiceState!.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    ErrorEmbed.SameVoiceChannelError(context)));

            return;
        }

        SearchResult spotifyTrack = null;
        LavalinkTrack youtubeTrack = null;

        switch (streamingPlatform)
        {
            // Use Youtube Explode
            case Platform.YouTube:

                Console.WriteLine("TEST");
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync("https://youtube.com/watch?v=u_yIGGhubZs");
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var audioStreamUrl = audioStreamInfo.Url;

                Console.WriteLine(audioStreamUrl);
                Console.WriteLine(video.Id);
                Console.WriteLine(video.Title);
                Console.WriteLine(video.Author.ToString());
                Console.WriteLine(video.Duration);

                await player.PlayAsync(new LavalinkTrack
                {
                    Identifier = video.Id,
                    Title = video.Title,
                    Author = video.Author.ToString(),
                    Duration = (TimeSpan) video.Duration!,
                    Uri = new Uri("https://youtube.com/watch?v=u_yIGGhubZs")
                });
                break;
            case Platform.Spotify: // Add condition for checking if an album
                spotifyTrack = await _audioService.Tracks.SearchAsync(
                    query,
                    loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
                    categories: ImmutableArray.Create(SearchCategory.Track));
                break;
            case Platform.SoundCloud:
                break;
            default:
                // Use Youtube Explode
                spotifyTrack = await _audioService.Tracks.SearchAsync(
                    query,
                    loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.YouTube),
                    categories: ImmutableArray.Create(SearchCategory.Track));
                break;
        }

        
        if (streamingPlatform == Platform.YouTube || query.Contains("youtube"))
        {
        }
        else if (streamingPlatform == Platform.Spotify || query.Contains("spotify"))
        {
            
        }
        else if (streamingPlatform == Platform.SoundCloud || query.Contains("soundcloud"))
        {
            var soundcloudTrack = await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.SoundCloud);
            await PlayFromSoundcloud(player, soundcloudTrack, context, guildId);
            return;
        }
        
        if (spotifyTrack == null)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.AudioTrackError(context)));

            return;
        }

        foreach (var result in spotifyTrack.Texts)
        {
            Console.WriteLine($"Value: {result.Text}");
        }

        var track = new ExtendedLavalinkTrack(spotifyTrack!.Tracks[0]);

        if (track.IsLiveStream)
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

        await player!.PlayAsync(track.Track);

        if (player.Queue.IsEmpty)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongInformation(track, player))));

            return;
        }

        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(AudioPlayerEmbed.SongAddedToQueue(track)));
    }
    
    private async Task PlayFromSoundcloud(QueuedLavalinkPlayer player, LavalinkTrack soundcloudTrack, InteractionContext context, ulong guildId)
    {
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