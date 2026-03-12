using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Modals.Interfaces;

namespace JamJunction.App.Events.Modals;

/// <summary>
/// Handles the execution of modal submission interaction events.
/// </summary>
/// <remarks>
/// This class acts as a dispatcher that routes modal submission interactions
/// to the appropriate <see cref="IModal"/> implementation. It allows modal
/// interaction logic to remain modular and separated from the main event
/// handling system.
/// </remarks>
public class ModalHandler
{
    /// <summary>
    /// Executes the specified modal interaction handler.
    /// </summary>
    /// <param name="modal">
    /// The <see cref="IModal"/> implementation responsible for processing
    /// the modal submission.
    /// </param>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the modal event.
    /// </param>
    /// <param name="modalEventArgs">
    /// The <see cref="ModalSubmitEventArgs"/> containing information about
    /// the submitted modal data and user interaction context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the modal interaction logic.
    /// </returns>
    /// <remarks>
    /// This method delegates the modal submission processing to the provided
    /// modal handler, allowing each modal interaction to implement its own
    /// logic independently.
    /// </remarks>
    public Task Execute(IModal modal, DiscordClient sender, ModalSubmitEventArgs modalEventArgs)
    {
        return modal.Execute(sender, modalEventArgs);
    }
}