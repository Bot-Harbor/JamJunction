using DSharpPlus.SlashCommands;
using PingEmbed = JamJunction.App.Views.Embeds.PingEmbed;

namespace JamJunction.App.Slash_Commands.Other_Commands;

/// <summary>
/// Slash command used to check if the bot is responsive.
/// </summary>
/// <remarks>
/// This command replies with a "pong" style response to confirm that
/// the bot is online and able to communicate with the Discord server.
/// The response is sent using an embed for consistent formatting.
/// </remarks>
public class PingCommand : ApplicationCommandModule
{
    /// <summary>
    /// Responds to the ping command with a pong message.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the requesting user.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the ping command.
    /// </returns>
    /// <remarks>
    /// This command builds a response using the <see cref="PingEmbed"/> class
    /// and sends it as an ephemeral interaction response so only the user
    /// who invoked the command can see it.
    /// </remarks>
    [SlashCommand("ping", "Will pong back to the server.")]
    public async Task PingAsync(InteractionContext context)
    {
        var pingEmbed = new PingEmbed();
        await context.CreateResponseAsync(pingEmbed.Build(context), true);
    }
}