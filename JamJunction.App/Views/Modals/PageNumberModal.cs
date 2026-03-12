using DSharpPlus.Entities;

namespace JamJunction.App.Views.Modals;

/// <summary>
/// Represents the modal used to allow users to jump directly
/// to a specific page in the queue viewer.
/// </summary>
/// <remarks>
/// This modal prompts the user to enter a page number which is
/// then used to display the corresponding queue page.
/// </remarks>
public class PageNumberModal
{
    /// <summary>
    /// Builds a modal interaction that allows the user to jump directly
    /// to a specific page in the queue view.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordInteractionResponseBuilder"/> configured as a modal
    /// containing a text input field where the user can enter the desired
    /// queue page number.
    /// </returns>
    public DiscordInteractionResponseBuilder Build()
    {
        var model = new DiscordInteractionResponseBuilder()
            .WithTitle("Jump To Page")
            .WithCustomId("jump-to-page")
            .AddComponents(
                new TextInputComponent("Page Number", "page-number"));

        return model;
    }
}