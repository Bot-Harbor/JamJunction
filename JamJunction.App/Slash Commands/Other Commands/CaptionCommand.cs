using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Embeds;
using Color = JamJunction.App.Views.Embeds.Enums.Color;

namespace JamJunction.App.Slash_Commands.Other_Commands;

/// <summary>
/// Slash command used to generate a captioned image embed.
/// </summary>
/// <remarks>
/// This command allows users to upload an image and attach a custom caption to it.
/// The caption and image are displayed inside a styled embed with a selectable color.
/// </remarks>
public class CaptionCommand : ApplicationCommandModule
{
    /// <summary>
    /// Creates an embed containing a caption and an uploaded image.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user executing the command.
    /// </param>
    /// <param name="caption">
    /// The caption text that will be displayed above the image.
    /// </param>
    /// <param name="image">
    /// The image attachment that will be displayed in the embed.
    /// </param>
    /// <param name="color">
    /// The embed color selected by the user.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the caption command.
    /// </returns>
    /// <remarks>
    /// This command builds a custom embed using the provided caption,
    /// image attachment, and color. The embed is generated using the
    /// <see cref="CaptionEmbed"/> builder and returned as the interaction response.
    /// </remarks>
    [SlashCommand("caption", "Give any image a caption.")]
    public async Task CaptionAsync
    (
        InteractionContext context,
        [Option("caption", "The caption you want the image to have.")]
        string caption,
        [Option("image", "The image you want to upload.")]
        DiscordAttachment image,
        [Option("color", "Pick the color you want the message embed to have.")]
        Color color
    )
    {
        var captionEmbed = new CaptionEmbed();
        await context.CreateResponseAsync(captionEmbed.Build(caption, image, color));
    }
}