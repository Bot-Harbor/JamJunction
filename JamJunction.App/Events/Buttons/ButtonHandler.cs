using DSharpPlus;
using DSharpPlus.EventArgs;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons;

/// <summary>
/// Handles the execution of button interaction events.
/// </summary>
/// <remarks>
/// This class acts as a dispatcher for button interactions by invoking
/// the appropriate <see cref="IButton"/> implementation associated with
/// the interaction. It allows button event logic to remain modular and
/// organized using the command pattern.
/// </remarks>
public class ButtonHandler
{
    /// <summary>
    /// Executes the specified button interaction handler.
    /// </summary>
    /// <param name="button">
    /// The <see cref="IButton"/> implementation responsible for handling
    /// the interaction.
    /// </param>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// button interaction, including the user and message context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the button interaction logic.
    /// </returns>
    /// <remarks>
    /// This method delegates execution to the provided button handler,
    /// allowing each button action to be implemented independently.
    /// </remarks>
    public Task Execute(IButton button, DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        return button.Execute(sender, btnInteractionArgs);
    }
}