using DSharpPlus.Entities;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;

namespace JamJunction.App.Lavalink;

public class LavalinkPlayerHandler
{
    private readonly IAudioService _audioService;

    public LavalinkPlayerHandler(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    ///     Gets queued Lavalink player for commands.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="voiceChannel"></param>
    /// <param name="connectToVoiceChannel"></param>
    /// <returns></returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, DiscordChannel voiceChannel,
        bool connectToVoiceChannel = true)
    {
        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

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
    ///     Gets queued Lavalink player for events.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="voiceChannelId"></param>
    /// <param name="connectToVoiceChannel"></param>
    /// <returns></returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

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