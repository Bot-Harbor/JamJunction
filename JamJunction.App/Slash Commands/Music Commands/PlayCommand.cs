using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Lavalink.Platforms;
using JamJunction.App.Lavalink.Platforms.Enums;
using JamJunction.App.Lavalink.Platforms.Interfaces;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to queue tracks for playback.
/// </summary>
/// <remarks>
/// This command allows users to search for or provide a direct URL to a track
/// from supported streaming platforms. The bot retrieves the appropriate
/// platform handler and queues the requested track using the Lavalink player.
///
/// Supported platforms include Spotify, YouTube, Deezer, SoundCloud,
/// and YouTube Music.
/// </remarks>
public class PlayCommand : ApplicationCommandModule
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

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Queues a track for playback in the user's voice channel.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user who executed it.
    /// </param>
    /// <param name="query">
    /// A keyword search or direct URL used to locate the track to play.
    /// </param>
    /// <param name="streamingPlatform">
    /// The streaming platform to use when searching for the track.
    /// If omitted, the platform may be inferred from the query.
    /// </param>
    /// <param name="queueNext">
    /// Determines whether the track should be inserted at the front of the
    /// queue rather than added to the end.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the play command.
    /// </returns>
    /// <remarks>
    /// This command performs several validations before queuing a track:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is connected to the same voice channel.</item>
    /// <item>Ensures the Lavalink player is active.</item>
    /// <item>Ensures the queue has not reached its maximum capacity.</item>
    /// </list>
    ///
    /// Once validation succeeds, the command selects the appropriate
    /// platform handler and delegates playback logic to the corresponding
    /// <see cref="IPlatform"/> implementation.
    /// </remarks>
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

        IEnumerable<IPlatform> platforms =
        [
            new SpotifyPlatform(_audioService),
            new YoutubePlatform(),
            new DeezerPlatform(_audioService),
            new SoundCloudPlatform(_audioService),
            new YouTubeMusicPlatform()
        ];

        var platformHandler = new PlatformHandler();

        var isUrl = Uri.TryCreate(query, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (isUrl)
        {
            foreach (var platform in platforms)
            {
                if (query!.Contains(platform.Url))
                {
                    platformHandler.Execute(platform, player, context, query, queueNext);
                    break;
                }
            }
        }
        else
        {
            foreach (var platform in platforms)
            {
                if (platform.Platform == streamingPlatform)
                {
                    platformHandler.Execute(platform, player, context, query, queueNext);
                    break;
                }
            }
        }
    }
}