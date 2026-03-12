using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;

namespace JamJunction.App.Lavalink;

public class LavalinkPlayerHandler
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

    public LavalinkPlayerHandler(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Retrieves the <see cref="QueuedLavalinkPlayer"/> associated with the specified guild
    /// for use within slash commands.
    /// </summary>
    /// <param name="guildId">
    /// The unique identifier of the Discord guild where the player should be retrieved.
    /// </param>
    /// <param name="voiceChannel">
    /// The voice channel that the player should connect to or operate within.
    /// </param>
    /// <param name="connectToVoiceChannel">
    /// Indicates whether the player should join the specified voice channel if it is not already connected.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> containing the <see cref="QueuedLavalinkPlayer"/> if retrieval succeeds;
    /// otherwise <c>null</c>.
    /// </returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, DiscordChannel voiceChannel,
        bool connectToVoiceChannel = true)
    {
        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var playerOptions = new QueuedLavalinkPlayerOptions { HistoryCapacity = 10000 };

            var result = await _audioService.Players
                .RetrieveAsync(guildId, voiceChannel.Id,
                    PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

            if (!result.IsSuccess) throw new Exception();

            return result.Player;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Retrieves the <see cref="QueuedLavalinkPlayer"/> associated with the specified guild
    /// for use within event handlers.
    /// </summary>
    /// <param name="guildId">
    /// The unique identifier of the Discord guild where the player should be retrieved.
    /// </param>
    /// <param name="voiceChannelId">
    /// The identifier of the voice channel that the player should connect to or operate within.
    /// </param>
    /// <param name="connectToVoiceChannel">
    /// Indicates whether the player should join the specified voice channel if it is not already connected.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> containing the <see cref="QueuedLavalinkPlayer"/> if retrieval succeeds;
    /// otherwise <c>null</c>.
    /// </returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var playerOptions = new QueuedLavalinkPlayerOptions { HistoryCapacity = 10000 };

            var result = await _audioService.Players
                .RetrieveAsync(guildId, voiceChannelId,
                    PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

            if (!result.IsSuccess) throw new Exception();

            return result.Player;
        }
        catch (Exception)
        {
            return null;
        }
    }
}