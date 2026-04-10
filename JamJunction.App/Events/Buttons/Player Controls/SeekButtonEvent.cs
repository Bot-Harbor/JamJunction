using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Views.Embeds;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the seek button interaction for the audio player.
/// </summary>
/// <remarks>
/// This event is triggered when a user presses the seek button on the
/// player interface. The handler responds with an ephemeral message
/// containing a clickable slash command mention that auto-fills the
/// seek command in the user's Discord text box.
/// </remarks>
public class SeekButtonEvent : IButton
{
    /// <summary>
    /// Executes the seek button interaction logic.
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
        if (btnInteractionArgs.Interaction.Data.CustomId == "seek-autofill")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();

            await btnInteractionArgs.Interaction.DeferAsync(true);

            await btnInteractionArgs.Interaction.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder()
                    .AsEphemeral()
                    .AddEmbed(audioPlayerEmbed.SeekAutofill()));
        }
    }
}
