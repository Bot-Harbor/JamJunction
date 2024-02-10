using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class ConnectCommand : ApplicationCommandModule
{
    [SlashCommand("connect", "Connects to guild.")]
    public async Task ConnectAsync(InteractionContext context)
    {
        var errorEmbed = new ErrorEmbed();

        try
        {
            var userVc = context.Member?.VoiceState?.Channel;
            var lava = context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            
            if (!lava.ConnectedNodes!.Any())
            {
                await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());
            }

            if (userVc == null || userVc.Type != ChannelType.Voice)
            {
                await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));
            }

            await node.ConnectAsync(userVc);

            var connection = node.GetGuildConnection(context.Guild);

            if (connection! == null)
            {
                await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
            }

            if (connection != null)
            {
                await context.CreateResponseAsync("Congrats");
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}
