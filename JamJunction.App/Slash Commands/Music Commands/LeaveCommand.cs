using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class LeaveCommand : ApplicationCommandModule
{
    [SlashCommand("leave", "Disconnects the player.")]
    public async Task LeaveCommandAsync(InteractionContext context)
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
                if (!lava.ConnectedNodes.Any())
                {
                    await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());
                }

                if (userVc == null || userVc.Type != ChannelType.Voice)
                {
                    await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));
                }

                await node.ConnectAsync(userVc);

                var connection = node.GetGuildConnection(context.Guild);

                if (connection == null)
                {
                    await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
                }

                if (connection != null && connection.CurrentState.CurrentTrack == null)
                {
                    await context.CreateResponseAsync(errorEmbed.NoAudioTrackErrorEmbedBuilder());
                }

                if (connection != null)
                {
                    await connection.DisconnectAsync();
                    await context.CreateResponseAsync(audioEmbed.LeaveEmbedBuilder(context));
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoLeavePermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}