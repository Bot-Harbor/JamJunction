using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Views.Embeds;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the playlist button interaction for the audio player.
/// </summary>
/// <remarks>
/// This event is triggered when a user presses the playlist button on the
/// player interface. The handler opens a DM channel with the bot and
/// responds with a link button so the user can navigate directly to it.
/// </remarks>
public class PlaylistButtonEvent : IButton
{
    /// <summary>
    /// Executes the playlist button interaction logic.
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
        if (btnInteractionArgs.Interaction.Data.CustomId == "playlist")
        {
            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            await btnInteractionArgs.Interaction.DeferAsync(true);

            var audioPlayerEmbed = new AudioPlayerEmbed();
            var dmChannel = await member.CreateDmChannelAsync();

            await btnInteractionArgs.Interaction.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder()
                    .AsEphemeral()
                    .AddEmbed(audioPlayerEmbed.PersonalPlaylist())
                    .AddComponents(audioPlayerEmbed.PersonalPlaylistButton(dmChannel.Id)));
        }
    }
}
