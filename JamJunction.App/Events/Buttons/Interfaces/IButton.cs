using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Buttons.Interfaces;

/// <summary>
/// Defines the contract for handling Discord button interactions.
/// </summary>
/// <remarks>
/// Classes implementing this interface represent a specific button action
/// within the Jam Junction interaction system. When a button component is
/// triggered in Discord, the associated implementation will execute the
/// appropriate logic.
/// </remarks>
public interface IButton
{
    /// <summary>
    /// Executes the logic associated with the button interaction.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction event.
    /// </param>
    /// <param name="e">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing information
    /// about the button interaction, including the user, message, and custom ID.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}