using System.Text.Json;
using System.Text.Json.Serialization;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to display lyrics for the currently playing track.
/// </summary>
/// <remarks>
/// Lyrics are fetched directly from LRCLIB (https://lrclib.net) using the current
/// track's title, artist, and duration. LRCLIB is keyless, source-agnostic, and
/// returns synced lyrics, so it works regardless of which platform the track was
/// played from. Lyrics are returned privately to the requesting user.
/// </remarks>
public class LyricsCommand : ApplicationCommandModule
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Dedicated client for LRCLIB lookups. Uses a generous timeout because LRCLIB's
    /// fuzzy search is slow, and a descriptive User-Agent as LRCLIB requests.
    /// </summary>
    private static readonly HttpClient LrcLibHttpClient = CreateLrcLibHttpClient();

    /// <summary>
    /// Provides access to the Lavalink audio service used for retrieving the
    /// active player and its current track.
    /// </summary>
    private readonly IAudioService _audioService;

    private readonly ILogger<LyricsCommand> _logger;

    private readonly IHostEnvironment _hostEnvironment;

    public LyricsCommand(IAudioService audioService, ILogger<LyricsCommand> logger, IHostEnvironment hostEnvironment)
    {
        _audioService = audioService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    /// <summary>
    /// Displays lyrics for the track currently playing in the voice channel.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user who executed it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the slash command.
    /// </returns>
    [SlashCommand("lyrics", "Shows lyrics for the current track.")]
    public async Task LyricsCommandAsync(InteractionContext context)
    {
        await context.DeferAsync(true);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();
        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.ValidVoiceChannelError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.NoPlayerError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.SameVoiceChannelError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.NoConnectionError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.PlayerInactiveError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lyrics = await GetLrcLibLyricsAsync(player.CurrentTrack);

        if (lyrics.Status == LyricsStatus.NotFound)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.LyricsNotFoundError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (lyrics.Status == LyricsStatus.Failed)
        {
            // Only expose the technical failure detail to developers. In any non-development
            // environment the client sees a generic message while the full detail stays in
            // the logs (already written by GetLrcLibLyricsAsync).
            var developerDetails = _hostEnvironment.IsDevelopment()
                ? lyrics.ErrorMessage
                : null;

            var errorMessage = await context.FollowUpAsync(
                errorEmbed.LyricsLookupError(developerDetails));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var embeds = audioPlayerEmbed.Lyrics(
            player.CurrentTrack,
            lyrics.Text).ToList();

        if (embeds.Count == 0)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.LyricsNotFoundError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        // Edit the deferred response with the first embed so the lyrics persist.
        // Deleting the response would tear down the ephemeral message instead.
        await context.EditResponseAsync(
            new DiscordWebhookBuilder().AddEmbed(embeds[0]));

        // Remaining embeds (for long lyrics) go out as additional ephemeral followups.
        foreach (var embed in embeds.Skip(1))
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AsEphemeral().AddEmbed(embed));
        }
    }

    private static HttpClient CreateLrcLibHttpClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("JamJunction (Discord music bot)");
        return client;
    }

    /// <summary>
    /// Fetches lyrics for the supplied track from LRCLIB. Tries the exact-match endpoint
    /// first, then falls back to the fuzzy search endpoint. Returns <see cref="LyricsStatus.NotFound"/>
    /// when LRCLIB has no usable lyrics, or <see cref="LyricsStatus.Failed"/> when the request errors.
    /// </summary>
    private async Task<LyricsResult> GetLrcLibLyricsAsync(Lavalink4NET.Tracks.LavalinkTrack track)
    {
        try
        {
            var durationSeconds = (int)Math.Round(track.Duration.TotalSeconds);

            var getUri =
                $"https://lrclib.net/api/get?artist_name={Uri.EscapeDataString(track.Author)}" +
                $"&track_name={Uri.EscapeDataString(track.Title)}" +
                $"&duration={durationSeconds}";

            var record = await GetLrcLibRecordAsync(getUri);

            if (record == null)
            {
                var searchUri =
                    $"https://lrclib.net/api/search?q={Uri.EscapeDataString($"{track.Title} {track.Author}")}";
                record = await GetLrcLibSearchFirstAsync(searchUri);
            }

            var text = record?.PlainLyrics;

            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(record?.SyncedLyrics))
            {
                text = StripLrcTimestamps(record.SyncedLyrics);
            }

            return string.IsNullOrWhiteSpace(text)
                ? LyricsResult.NotFound()
                : LyricsResult.Found(NormalizeLyricsText(text));
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "LRCLIB lyrics lookup failed.");
            return LyricsResult.Failed("The lyrics request to LRCLIB could not be completed. Check the bot logs.");
        }
    }

    private static async Task<LrcLibRecord> GetLrcLibRecordAsync(string requestUri)
    {
        using var response = await LrcLibHttpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<LrcLibRecord>(stream, SerializerOptions);
    }

    private static async Task<LrcLibRecord> GetLrcLibSearchFirstAsync(string requestUri)
    {
        using var response = await LrcLibHttpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var results = await JsonSerializer.DeserializeAsync<List<LrcLibRecord>>(stream, SerializerOptions);

        return results?.FirstOrDefault(result =>
            !string.IsNullOrWhiteSpace(result.PlainLyrics) ||
            !string.IsNullOrWhiteSpace(result.SyncedLyrics));
    }

    private static string StripLrcTimestamps(string syncedLyrics)
    {
        var strippedLines = syncedLyrics
            .Split('\n')
            .Select(line => System.Text.RegularExpressions.Regex.Replace(
                line, @"^\s*\[\d{1,2}:\d{2}(?:\.\d{1,3})?\]\s*", string.Empty));

        return string.Join("\n", strippedLines);
    }

    private static string NormalizeLyricsText(string lyrics)
    {
        var normalizedLyrics = lyrics
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Trim();

        while (normalizedLyrics.Contains("\n\n\n"))
        {
            normalizedLyrics = normalizedLyrics.Replace("\n\n\n", "\n\n");
        }

        return normalizedLyrics;
    }

    private sealed class LrcLibRecord
    {
        [JsonPropertyName("plainLyrics")]
        public string PlainLyrics { get; set; }

        [JsonPropertyName("syncedLyrics")]
        public string SyncedLyrics { get; set; }
    }

    private sealed class LyricsResult
    {
        private LyricsResult(LyricsStatus status, string text = null, string errorMessage = null)
        {
            Status = status;
            Text = text;
            ErrorMessage = errorMessage;
        }

        public LyricsStatus Status { get; }

        public string Text { get; }

        public string ErrorMessage { get; }

        public static LyricsResult Found(string text)
        {
            return new LyricsResult(LyricsStatus.Found, text);
        }

        public static LyricsResult NotFound()
        {
            return new LyricsResult(LyricsStatus.NotFound);
        }

        public static LyricsResult Failed(string errorMessage)
        {
            return new LyricsResult(LyricsStatus.Failed, errorMessage: errorMessage);
        }
    }

    private enum LyricsStatus
    {
        Found,
        NotFound,
        Failed
    }
}
