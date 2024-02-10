using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class MuteCommand : ApplicationCommandModule
{
    [SlashCommand("mute", "Mute the volume.")]
    public async Task MuteCommandAsync(InteractionContext context)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();

        var guildId = context.Guild.Id;
        var audioPlayerController = Bot.GuildAudioPlayers[guildId];

        try
        {
            var userVc = context.Member?.VoiceState?.Channel;
            var lava = context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (context.Member != null && (context.Member.Permissions & Permissions.ManageChannels) != 0)
            {
                if (!lava.ConnectedNodes!.Any())
                {
                    await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());
                }

                if (userVc == null || userVc.Type != ChannelType.Voice)
                {
                    await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));
                }

                var connection = node.GetGuildConnection(context.Guild);

                if (connection! == null)
                {
                    await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
                }

                if (connection != null && connection.CurrentState.CurrentTrack == null)
                {
                    await context.CreateResponseAsync(errorEmbed.NoAudioTrackErrorEmbedBuilder());
                }
                
                if (connection != null)
                {
                    if (audioPlayerController.PauseInvoked)
                    {
                        await context.CreateResponseAsync(errorEmbed.NoMuteWhilePausedEmbedBuilder(context));
                    }
                    else
                    {
                        await connection.SetVolumeAsync(0);
                        audioPlayerController.MuteInvoked = true;
                        await context.CreateResponseAsync(audioEmbed.MuteEmbedBuilder(context));
                    }
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoVolumePermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}