using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class StopCommand : ApplicationCommandModule
{
    [SlashCommand("stop", "Stops the playback.")]
    public async Task StopCommandAsync(InteractionContext context)
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
                    await connection.StopAsync();
                    
                    VolumeCommand.VolumeCommandInvoked = false;
                    PlayCommand.FirstTrackOnConnection = true;
                    PlayCommand.DefaultVolume = 50;
                    PauseCommand.PauseCommandInvoked = false;
                    PauseButton.PauseCommandInvoked = false;
                    MuteCommand.MuteCommandInvoked = false;
                    MuteButton.MuteButtonInvoked = false;
                    PlayCommand.Queue.Clear();

                    await context.CreateResponseAsync(audioEmbed.StopEmbedBuilder(context));
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoStopPermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}