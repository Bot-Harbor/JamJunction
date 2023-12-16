using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.EmbedBuilders;

namespace JamJunction.App.Slash_Commands.OtherCommands;

public class PingCommand : ApplicationCommandModule
{
    [SlashCommand("ping", "Will pong back to the server.")]
    public async Task PingAsync(InteractionContext context)
    {
        try
        {
            var pingEmbed = new PingEmbed();

            await context.CreateResponseAsync(pingEmbed.PingEmbedBuilder(context), ephemeral: true);
        }
        catch (FormatException)
        {
            var errorEmbed = new ErrorEmbed();
            
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}