using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using JamJunction.App.Lavalink.Enums;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;
    private ErrorEmbed ErrorEmbed { get; set; } = new();

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("play", "Queue a song.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Input", "Enter a keyword or url to search.")]
        string query,
        [Option("Platform", "Pick a streaming platform. Default platform is Youtube.")]
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

        if (streamingPlatform == default)
        {
            streamingPlatform = CheckForUrl(query);
        }

        var platformHandler = new PlatformHandler(_audioService);
        
        switch (streamingPlatform)
        {
            case Platform.YouTube:
                await platformHandler.PlayFromYoutube(player, query, context, guildId);
                return;
            case Platform.Spotify:
                await platformHandler.PlayFromSpotify(player, query, context, guildId);
                return;
            case Platform.SoundCloud:
                await platformHandler.PlayFromSoundCloud(player, query, context, guildId);
                return;
            default:
                await platformHandler.PlayFromYoutube(player, query, context, guildId);
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