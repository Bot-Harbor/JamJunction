using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class ViewQueueCommand : ApplicationCommandModule
{
    [SlashCommand("viewqueue", "Displays what is currently in the queue.")]
    public async Task ViewQueueCommandAsync(InteractionContext context)
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

            if (connection != null) await context.CreateResponseAsync(audioEmbed.ViewQueueBuilder(context));
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), true);
        }
    }
}