using DSharpPlus;
using DSharpPlus.EventArgs;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the "skip" button shown on an AI song recommendation.
/// </summary>
/// <remarks>
/// This event is triggered when a user dismisses a recommendation embed posted
/// after a track finishes. The handler deletes the recommendation message and
/// clears the stored recommendation state for the guild so the suggestion is
/// discarded without being queued.
/// </remarks>
public class SkipRecommendationButtonEvent : IButton
{
    /// <summary>
    /// Executes the skip-recommendation button interaction logic.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// button interaction, the user who triggered it, and the guild context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "skip-recommendation")
        {
            // Acknowledge the interaction without showing a loading state, then
            // remove the recommendation message so it is dismissed for everyone.
            await btnInteractionArgs.Interaction.CreateResponseAsync(
                InteractionResponseType.DeferredMessageUpdate);

            _ = btnInteractionArgs.Message.DeleteAsync();

            // Clear the stored recommendation state so the next track start does
            // not attempt to delete an already-removed message.
            Bot.GuildData.TryGetValue(btnInteractionArgs.Guild.Id, out var guildData);

            if (guildData != null)
            {
                guildData.RecommendationMessage = null;
                guildData.RecommendationQuery = null;
                guildData.RecommendationSource = null;
            }
        }
    }
}
