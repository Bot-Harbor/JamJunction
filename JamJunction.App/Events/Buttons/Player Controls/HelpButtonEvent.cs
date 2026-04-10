using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Views.Embeds;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the help button interaction for the audio player.
/// </summary>
/// <remarks>
/// This event is triggered when a user presses the help button on the
/// player interface. The handler responds with an ephemeral help embed
/// containing available commands and bot information.
/// </remarks>
public class HelpButtonEvent : IButton
{
    /// <summary>
    /// Executes the help button interaction logic.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// button interaction and the user who triggered it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "help")
        {
            var helpEmbed = new HelpEmbed();

            await btnInteractionArgs.Interaction.DeferAsync(true);

            await btnInteractionArgs.Interaction.CreateFollowupMessageAsync(
                helpEmbed.Build(sender, btnInteractionArgs.User));
        }
    }
}
