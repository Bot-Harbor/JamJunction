using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Menus.Interfaces;

namespace JamJunction.App.Events.Menus;

/// <summary>
/// Handles the execution of select menu interaction events.
/// </summary>
/// <remarks>
/// This class acts as a dispatcher that routes menu interactions to the
/// appropriate <see cref="IMenu"/> implementation. It allows menu logic
/// to remain modular and organized by separating interaction handling
/// from the core bot event system.
/// </remarks>
public class MenuHandler
{
    /// <summary>
    /// Executes the specified menu interaction handler.
    /// </summary>
    /// <param name="menu">
    /// The <see cref="IMenu"/> implementation responsible for processing
    /// the interaction.
    /// </param>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// select menu interaction, including the selected values and user context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the menu interaction logic.
    /// </returns>
    /// <remarks>
    /// This method delegates the interaction processing to the provided
    /// menu handler, allowing each menu to implement its own logic
    /// independently.
    /// </remarks>
    public Task Execute(IMenu menu, DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        return menu.Execute(sender, btnInteractionArgs);
    }
}