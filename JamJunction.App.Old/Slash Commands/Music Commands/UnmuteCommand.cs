using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class UnmuteCommand : ApplicationCommandModule
{
    [SlashCommand("unmute", "Unmute the volume.")]
    public async Task UnmuteCommandAsync(InteractionContext context)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();

        try
        {
            var userVc = context.Member?.VoiceState?.Channel;
            var lava = context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (!lava.ConnectedNodes!.Any())
                await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());

            if (userVc == null || userVc.Type != ChannelType.Voice)
                await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));

            var connection = node.GetGuildConnection(context.Guild);

            if (connection! == null) await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());

            if (connection != null && connection.CurrentState.CurrentTrack == null)
                await context.CreateResponseAsync(errorEmbed.NoAudioTrackErrorEmbedBuilder());

            if (connection != null)
            {
                var guildId = context.Guild.Id;
                var audioPlayerController = Bot.GuildAudioPlayers[guildId];

                if (audioPlayerController.PauseInvoked)
                {
                    await context.CreateResponseAsync(errorEmbed.NoUnMuteWhilePausedEmbedBuilder(context));
                }
                else
                {
                    await connection.SetVolumeAsync(audioPlayerController.Volume);
                    audioPlayerController.MuteInvoked = false;
                    await context.CreateResponseAsync(audioEmbed.UnmuteEmbedBuilder(context));
                }
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), true);
        }
    }
}