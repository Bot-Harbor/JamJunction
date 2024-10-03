using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;

namespace JamJunction.App;

public class LavalinkPlayerHandler
{
    private readonly IAudioService _audioService;

    public LavalinkPlayerHandler(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Gets queued Lavalink player using interaction context.
    /// </summary>
    /// <param name="interactionContext"></param>
    /// <param name="guildId"></param>
    /// <param name="voiceChannel"></param>
    /// <param name="connectToVoiceChannel"></param>
    /// <returns></returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(InteractionContext interactionContext, ulong guildId,
        DiscordChannel voiceChannel, bool connectToVoiceChannel = true)
    {
        try
        {
            var retrieveOptions = new PlayerRetrieveOptions(
                ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

            var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

            var result = await _audioService.Players
                .RetrieveAsync(guildId, voiceChannel.Id,
                    playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);
            
            if (!result.IsSuccess)
            {
                var errorEmbed = new ErrorEmbed();
                
                var errorMessage = result.Status;

                if (errorMessage == PlayerRetrieveStatus.BotNotConnected)
                {
                    await interactionContext.FollowUpAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.LavaLinkErrorEmbedBuilder()));
                }
                else
                {
                    await interactionContext.FollowUpAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.UnknownErrorEmbedBuilder()));
                }
            }

            return result.Player;
        }
        catch (Exception)
        { 
            return null;
        }
    }

    /// <summary>
    /// Gets queued Lavalink player without using interaction context.
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="voiceChannelId"></param>
    /// <param name="connectToVoiceChannel"></param>
    /// <returns></returns>
    public async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

        var result = await _audioService.Players
            .RetrieveAsync(guildId, voiceChannelId,
                playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

        return result.Player;
    }
}