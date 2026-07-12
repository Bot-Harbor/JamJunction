using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using JamJunction.App.Secrets;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Ai;

/// <summary>
/// Generates AI-powered "you might also like" song recommendations based on a
/// track the user just finished listening to.
/// </summary>
/// <remarks>
/// Recommendations are produced entirely offline by a local large language model
/// served through <see href="https://ollama.com">Ollama</see>. The recommender is a
/// non-essential, best-effort feature: any failure (Ollama not running, network
/// error, malformed response) results in a <c>null</c> recommendation rather than
/// an exception, so it can never disrupt playback.
///
/// The feature is disabled when <see cref="OllamaSecrets.Enabled"/> is <c>false</c>
/// or no host has been configured.
/// </remarks>
public static class SongRecommender
{
    /// <summary>
    /// The Ollama chat completion endpoint on the configured local server.
    /// </summary>
    private static readonly string ChatEndpoint =
        $"http://{OllamaSecrets.Host}:{OllamaSecrets.Port}/api/chat";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// JSON schema describing the structured recommendation Ollama must return.
    /// Passing this as the request <c>format</c> constrains the model's output to
    /// valid JSON we can deserialize directly into a <see cref="SongRecommendation"/>.
    /// </summary>
    private static readonly object ResponseSchema = new
    {
        type = "object",
        properties = new
        {
            title = new { type = "string" },
            artist = new { type = "string" },
            reason = new { type = "string" }
        },
        required = new[] { "title", "artist", "reason" }
    };

    /// <summary>
    /// Shared HTTP client used to talk to the local Ollama server. A generous
    /// timeout accommodates slower local model inference; the request still runs in
    /// the background so it never blocks playback.
    /// </summary>
    private static readonly Lazy<HttpClient> Client = new(() =>
        new HttpClient { Timeout = TimeSpan.FromSeconds(30) });

    /// <summary>
    /// Indicates whether the recommender is enabled. The feature is disabled when
    /// it has been switched off or no Ollama host has been configured.
    /// </summary>
    public static bool IsEnabled =>
        OllamaSecrets.Enabled && !string.IsNullOrWhiteSpace(OllamaSecrets.Host);

    /// <summary>
    /// Asks the local model to recommend a single song similar in mood or style to
    /// the supplied track.
    /// </summary>
    /// <param name="track">
    /// The <see cref="LavalinkTrack"/> the user just finished listening to.
    /// </param>
    /// <returns>
    /// A <see cref="SongRecommendation"/> describing a suggested track, or
    /// <c>null</c> when the recommender is disabled or the request could not be
    /// completed for any reason.
    /// </returns>
    public static async Task<SongRecommendation> RecommendAsync(LavalinkTrack track)
    {
        if (!IsEnabled)
            return null;

        try
        {
            var request = new
            {
                model = OllamaSecrets.Model,
                stream = false,
                format = ResponseSchema,
                options = new { temperature = 0.8 },
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content =
                            "You are a music recommendation engine for a Discord music bot. " +
                            "Given a song the user just listened to, recommend exactly one different, " +
                            "real, well-known song they are likely to enjoy next. Prefer a different " +
                            "artist with a similar mood, genre, or era. Keep the reason to a single short sentence. " +
                            "Respond only with JSON matching the requested schema."
                    },
                    new
                    {
                        role = "user",
                        content =
                            $"The user just listened to \"{track.Title}\" by {track.Author}. " +
                            "Recommend one different song they might enjoy next."
                    }
                }
            };

            var response = await Client.Value.PostAsJsonAsync(ChatEndpoint, request);

            if (!response.IsSuccessStatusCode)
                return null;

            var chatResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(SerializerOptions);

            var json = chatResponse?.Message?.Content;

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var recommendation = JsonSerializer.Deserialize<SongRecommendation>(json, SerializerOptions);

            if (recommendation == null ||
                string.IsNullOrWhiteSpace(recommendation.Title) ||
                string.IsNullOrWhiteSpace(recommendation.Artist))
                return null;

            return recommendation;
        }
        catch
        {
            // The recommender is best-effort. Swallow any failure so a bad request
            // or an unreachable Ollama server never interferes with playback or
            // crashes the event handler.
            return null;
        }
    }
}

/// <summary>
/// Minimal representation of the response returned by Ollama's chat endpoint.
/// </summary>
internal sealed class OllamaChatResponse
{
    /// <summary>
    /// The assistant message produced by the model. Its <see cref="OllamaMessage.Content"/>
    /// holds the JSON recommendation payload.
    /// </summary>
    [JsonPropertyName("message")]
    public OllamaMessage Message { get; set; }
}

/// <summary>
/// A single chat message within an Ollama response.
/// </summary>
internal sealed class OllamaMessage
{
    /// <summary>
    /// The role of the message author (e.g. <c>assistant</c>).
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    /// <summary>
    /// The message content. For recommendation requests this is a JSON string
    /// matching the <see cref="SongRecommendation"/> schema.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

/// <summary>
/// Represents a single AI-generated song recommendation.
/// </summary>
public sealed class SongRecommendation
{
    /// <summary>
    /// The title of the recommended song.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// The artist who performs the recommended song.
    /// </summary>
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    /// <summary>
    /// A short, one-sentence explanation of why the song was recommended.
    /// </summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}
