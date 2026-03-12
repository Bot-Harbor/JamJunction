using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Lavalink.Enums;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("play", "Queue a track.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Input", "Enter a keyword or url to search.")]
        string query,
        [Option("Platform", "Pick a streaming platform. Default platform for keywords is Spotify.")]
        Platform streamingPlatform = default,
        [Option("Queue-Next", "Queues the track next")]
        bool queueNext = default
    )
    {
        await context.DeferAsync();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        var errorEmbed = new ErrorEmbed();

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player = await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel);

        if (player == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (userVoiceChannel.Id != botVoiceState!.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player.Queue.Count >= 100)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.QueueIsFullError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (streamingPlatform == default) streamingPlatform = CheckForUrl(query);

        var platformHandler = new PlatformHandler();

        switch (streamingPlatform)
        {
            case Platform.Spotify:
                platformHandler.Excute(new SpotifyPlatform(_audioService), player, context, query, queueNext);
                return;
            case Platform.YouTube:
                platformHandler.Excute(new YoutubePlatform(), player, context, query, queueNext);
                return;
            case Platform.Deezer:
                platformHandler.Excute(new DeezerPlatform(_audioService), player, context, query, queueNext);
                return;
            case Platform.SoundCloud:
                platformHandler.Excute(new SoundCloudPlatform(_audioService), player, context, query, queueNext);
                return;
            case Platform.YouTubeMusic:
                platformHandler.Excute(new YouTubeMusicPlatform(), player, context, query, queueNext);
                return;
            default:
                platformHandler.Excute(new SpotifyPlatform(_audioService), player, context, query, queueNext);
                return;
        }
    }

    private Platform CheckForUrl(string query)
    {
        return query switch
        {
            var a when a.Contains("spotify.com") => Platform.Spotify,
            var b when b.Contains("youtube.com") || b.Contains("music.youtube") => Platform.YouTube,
            var c when c.Contains("deezer.com") => Platform.Deezer,
            var d when d.Contains("soundcloud.com") => Platform.SoundCloud,
            _ => default
        };
    }
}