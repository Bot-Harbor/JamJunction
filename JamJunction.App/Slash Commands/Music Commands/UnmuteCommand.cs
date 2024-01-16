using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class UnmuteCommand : ApplicationCommandModule
{
    public static bool MuteCommandInvoked { get; set; }

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
                    if (!VolumeCommand.VolumeCommandInvoked)
                    {
                        if (PauseCommand.PauseCommandInvoked || PauseButton.PauseCommandInvoked)
                        {
                            await context.CreateResponseAsync(errorEmbed.NoUnMuteWhilePausedEmbedBuilder(context));
                        }
                        else
                        {
                            await connection.SetVolumeAsync(PlayCommand.DefaultVolume);
                            MuteCommand.MuteCommandInvoked = false;
                            MuteButton.MuteButtonInvoked = false;
                            await context.CreateResponseAsync(audioEmbed.UnmuteEmbedBuilder(context));
                        }
                    }
                    else
                    {
                        if (PauseCommand.PauseCommandInvoked || PauseButton.PauseCommandInvoked)
                        {
                            await context.CreateResponseAsync(errorEmbed.NoUnMuteWhilePausedEmbedBuilder(context));
                        }
                        else
                        {
                            await connection.SetVolumeAsync(VolumeCommand.Volume);
                            MuteCommand.MuteCommandInvoked = false;
                            MuteButton.MuteButtonInvoked = false;
                            await context.CreateResponseAsync(audioEmbed.UnmuteEmbedBuilder(context));
                        }
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