using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Embeds;

namespace JamJunction.App.Slash_Commands.Other_Commands;

/// <summary>
/// Slash command used to display information about the bot and its available commands.
/// </summary>
/// <remarks>
/// This command provides users with an overview of the bot's functionality,
/// including descriptions of available commands and how to interact with them.
/// The information is displayed using an embed message.
/// </remarks>
public class HelpCommand : ApplicationCommandModule
{
    /// <summary>
    /// Sends a help message containing information about the bot and its commands.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user requesting help.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the help command.
    /// </returns>
    /// <remarks>
    /// This command builds a help embed using the <see cref="HelpEmbed"/> class
    /// and sends it as an ephemeral interaction response. The response is only
    /// visible to the user who invoked the command.
    /// </remarks>
    [SlashCommand("help", "Gives information about the bot & available commands.")]
    public async Task HelpCommandAsync(InteractionContext context)
    {
        var helpEmbed = new HelpEmbed();
        await context.CreateResponseAsync(
            new DiscordInteractionResponseBuilder(helpEmbed.Build(context)).AsEphemeral()
        );
    }
}