using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Modals.Interfaces;

/// <summary>
/// Defines the contract for handling Discord modal submission interactions.
/// </summary>
/// <remarks>
/// Classes implementing this interface represent a modal interaction
/// within Jam Junction. When a user submits a modal form, the corresponding
/// implementation processes the submitted data and performs the required
/// logic.
/// </remarks>

public interface IModal
{
    /// <summary>
    /// Executes the logic associated with the modal submission interaction.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the modal event.
    /// </param>
    /// <param name="modalEventArgs">
    /// The <see cref="ModalSubmitEventArgs"/> containing information about the
    /// modal submission, including the input values and user context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method is called when a user submits a modal form in Discord.
    /// The submitted values can be accessed through the interaction data
    /// contained in <paramref name="modalEventArgs"/>.
    /// </remarks>
    Task Execute(DiscordClient sender, ModalSubmitEventArgs modalEventArgs);
}