using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the "add to queue" button shown on an AI song recommendation.
/// </summary>
/// <remarks>
/// This event is triggered when a user presses the button on a recommendation
/// embed posted after a track finishes. The handler validates the user's voice
/// channel, resolves the recommended track, queues it, and removes the
/// recommendation message so it cannot be added twice.
/// </remarks>
public class AddRecommendationButtonEvent : IButton
{
    /// <summary>
    /// Provides access to the Lavalink audio service used for managing
    /// audio playback and retrieving player instances.
    /// </summary>
    private readonly IAudioService _audioService;

    /// <summary>
    /// The Discord client used to interact with the Discord API.
    /// </summary>
    private readonly DiscordClient _discordClient;

    public AddRecommendationButtonEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Gets or sets the voice channel that the interacting user is currently connected to.
    /// </summary>
    private DiscordChannel UserVoiceChannel { get; set; }

    /// <summary>
    /// Executes the add-recommendation button interaction logic.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// button interaction, the user who triggered it, and the guild context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method performs the same voice-channel validation as the other player
    /// controls before queuing the recommended track. Once queued, the
    /// recommendation message is deleted and the player embed is refreshed so the
    /// queue count stays accurate.
    /// </remarks>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "add-recommendation")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync(true);

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        errorEmbed.ValidVoiceChannelError());
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.ValidVoiceChannelError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await btnInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.NoPlayerError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState!.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.SameVoiceChannelError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
            var player = await lavalinkPlayer.GetPlayerAsync(guildId, UserVoiceChannel, false);

            if (player == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.NoConnectionError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            // Remove the recommendation message immediately so it cannot be queued
            // twice, and clear the stored recommendation state for this guild.
            _ = btnInteractionArgs.Message.DeleteAsync();

            Bot.GuildData.TryGetValue(guildId, out var guildData);

            var query = guildData?.RecommendationQuery;
            var source = guildData?.RecommendationSource;

            if (guildData != null)
            {
                guildData.RecommendationMessage = null;
                guildData.RecommendationQuery = null;
                guildData.RecommendationSource = null;
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.AudioTrackError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            if (player.Queue.Count >= 100)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.QueueIsFullError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var track = await _audioService.Tracks.LoadTrackAsync(query, ResolveSearchMode(source));

            if (track == null || track.IsLiveStream)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    errorEmbed.AudioTrackError());
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            // YouTube Music is served through a YouTube search, so the loaded track
            // carries a plain youtube.com URI. Rebuild it with a music.youtube.com URI
            // (the same way the play command constructs YouTube Music tracks) so the
            // player UI labels the queued suggestion "YouTube Music" rather than "YouTube".
            if (source == "youtubemusic")
                track = new LavalinkTrack
                {
                    SourceName = "youtube",
                    Identifier = track.Identifier,
                    IsSeekable = track.IsSeekable,
                    IsLiveStream = track.IsLiveStream,
                    Title = track.Title,
                    Author = track.Author,
                    StartPosition = TimeSpan.Zero,
                    Duration = track.Duration,
                    Uri = new Uri($"https://music.youtube.com/watch?v={track.Identifier}"),
                    ArtworkUri = track.ArtworkUri
                };

            await player.PlayAsync(track);

            // Refresh the player embed so the queue count reflects the new track.
            if (guildData?.PlayerMessage != null)
                try
                {
                    var updatedPlayerMessage =
                        await btnInteractionArgs.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
                    await updatedPlayerMessage.ModifyAsync(
                        audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            var confirmMessage = await channel.CreateFollowupMessageAsync(
                audioPlayerEmbed.TrackAddedToQueue(track));

            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(confirmMessage.Id);
        }
    }

    /// <summary>
    /// Maps a Lavalink track <c>SourceName</c> to the matching
    /// <see cref="TrackSearchMode"/> so the recommended track is resolved from the
    /// same platform the finished track came from.
    /// </summary>
    /// <param name="source">
    /// The <c>SourceName</c> of the track the recommendation was based on (e.g.
    /// <c>spotify</c>, <c>youtube</c>). May be <c>null</c> if the source was not
    /// recorded.
    /// </param>
    /// <returns>
    /// The <see cref="TrackSearchMode"/> for the given source, defaulting to
    /// <see cref="TrackSearchMode.Spotify"/> for an unknown or missing source.
    /// </returns>
    private static TrackSearchMode ResolveSearchMode(string source) => source switch
    {
        "youtube" => TrackSearchMode.YouTube,
        "youtubemusic" => TrackSearchMode.YouTube,
        "deezer" => TrackSearchMode.Deezer,
        "soundcloud" => TrackSearchMode.SoundCloud,
        "applemusic" => TrackSearchMode.AppleMusic,
        "spotify" => TrackSearchMode.Spotify,
        _ => TrackSearchMode.Spotify
    };
}
