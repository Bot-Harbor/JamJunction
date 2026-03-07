using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Legacy.Embed_Builders;

namespace JamJunction.App.Legacy.Slash_Commands.Other_Commands;

public class HelpCommand : ApplicationCommandModule
{
    [SlashCommand("help", "Gives information about the bot & available commands.")]
    public async Task HelpCommandAsync(InteractionContext context)
    {
        try
        {
            var helpEmbed = new HelpEmbed();

            await context.CreateResponseAsync(
                new DiscordInteractionResponseBuilder(helpEmbed.HelpEmbedBuilder(context)).AsEphemeral()
            );
        }
        catch (FormatException)
        {
            var errorEmbed = new ErrorEmbed();

            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), true);
        }
    }
}