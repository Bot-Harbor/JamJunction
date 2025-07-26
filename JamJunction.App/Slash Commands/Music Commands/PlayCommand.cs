using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using JamJunction.App.Lavalink.Enums;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private ErrorEmbed ErrorEmbed { get; } = new();

    [SlashCommand("play", "Queue a track.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Input", "Enter a keyword or url to search.")]
        string query,
        [Option("Platform", "Pick a streaming platform. Default platform for keywords is Spotify.")]
        Platform streamingPlatform = default
    )
    {
        await context.DeferAsync();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    ErrorEmbed.BuildValidVoiceChannelError(context)));
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
                    ErrorEmbed.BuildNoConnectionError(context)));
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
                    ErrorEmbed.BuildSameVoiceChannelError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player.Queue.Count >= 100)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    ErrorEmbed.BuildQueueIsFullError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (streamingPlatform == default) streamingPlatform = CheckForUrl(query);

        var platformHandler = new PlatformHandler(_audioService);

        switch (streamingPlatform)
        {
            case Platform.Spotify:
                await platformHandler.PlayFromSpotify(player, query, context, guildId);
                return;
            case Platform.YouTube:
                await platformHandler.PlayFromYoutube(player, query, context, guildId);
                return;
            case Platform.SoundCloud:
                await platformHandler.PlayFromSoundCloud(player, query, context, guildId);
                return;
            default:
                await platformHandler.PlayFromSpotify(player, query, context, guildId);
                return;
        }
    }

    private Platform CheckForUrl(string query)
    {
        return query switch
        {
            var a when a.Contains("youtube.com") => Platform.YouTube,
            var b when b.Contains("spotify.com") => Platform.Spotify,
            var c when c.Contains("soundcloud.com") => Platform.SoundCloud,
            _ => default
        };
    }
}