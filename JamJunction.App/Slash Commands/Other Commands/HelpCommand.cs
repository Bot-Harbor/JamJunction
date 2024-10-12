using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Other_Commands;

public class HelpCommand : ApplicationCommandModule
{
    [SlashCommand("help", "Gives information about the bot & available commands.")]
    public async Task HelpCommandAsync(InteractionContext context)
    {
        var helpEmbed = new HelpEmbed();
        await context.CreateResponseAsync(
            new DiscordInteractionResponseBuilder(helpEmbed.Help(context)).AsEphemeral()
        );
    }
}