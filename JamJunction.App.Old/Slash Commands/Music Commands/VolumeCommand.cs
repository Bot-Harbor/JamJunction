using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class VolumeCommand : ApplicationCommandModule
{
    [SlashCommand("volume", "Adjust the volume 0-100. Default volume is 50.")]
    public async Task VolumeCommandAsync(InteractionContext context,
        [Option("level", "How loud do you want the music to be?")]
        double volume)
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
                    await context.CreateResponseAsync(errorEmbed.NoVolumeWhilePausedEmbedBuilder(context));
                }
                else
                {
                    if (volume > 100)
                    {
                        await context.CreateResponseAsync(errorEmbed.MaxVolumeEmbedBuilder(context));
                    }
                    else if (volume < 0)
                    {
                        await context.CreateResponseAsync(errorEmbed.MinVolumeEmbedBuilder(context));
                    }
                    else
                    {
                        await connection.SetVolumeAsync(Convert.ToInt32(volume));
                        await context.CreateResponseAsync(audioEmbed.VolumeEmbedBuilder(Convert.ToInt32(volume),
                            context));

                        audioPlayerController.Volume = Convert.ToInt32(volume);
                        audioPlayerController.MuteInvoked = false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), true);
        }
    }
}