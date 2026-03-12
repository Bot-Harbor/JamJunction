using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Menus.Interfaces;

/// <summary>
/// Defines the contract for handling Discord select menu interactions.
/// </summary>
/// <remarks>
/// Classes implementing this interface represent a specific select menu
/// interaction within Jam Junction. When a user selects an option from
/// a menu component, the corresponding implementation will execute
/// the required logic.
/// </remarks>
public interface IMenu
{
    /// <summary>
    /// Executes the logic associated with the menu interaction.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction event.
    /// </param>
    /// <param name="e">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing information
    /// about the select menu interaction, including the selected values and user context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method is called when a user selects an option from a Discord
    /// select menu component.
    /// </remarks>
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}