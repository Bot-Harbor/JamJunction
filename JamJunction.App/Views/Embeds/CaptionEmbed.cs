using DSharpPlus.Entities;
using Color = JamJunction.App.Views.Embeds.Enums.Color;

namespace JamJunction.App.Views.Embeds;

/// <summary>
/// Provides functionality for building caption embeds that include
/// a title, image attachment, and configurable color.
/// </summary>
/// <remarks>
/// This embed is typically used by the caption command to display
/// a user-provided caption along with an uploaded image.
/// </remarks>
public class CaptionEmbed
{
    /// <summary>
    /// Builds an embed containing a caption and image, while converting a custom
    /// <see cref="Color"/> value into the corresponding <see cref="DiscordColor"/>.
    /// </summary>
    /// <param name="caption">
    /// The title or caption text that will appear at the top of the embed.
    /// </param>
    /// <param name="image">
    /// The <see cref="DiscordAttachment"/> containing the image to display in the embed.
    /// </param>
    /// <param name="color">
    /// The custom <see cref="Color"/> value used to determine the embed's color.
    /// This value is mapped to the equivalent <see cref="DiscordColor"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> containing the caption, image, and
    /// corresponding Discord embed color.
    /// </returns>
    public DiscordEmbedBuilder Build(string caption, DiscordAttachment image, Color color)
    {
        var discordColor = color switch
        {
            Color.Aquamarine => DiscordColor.Aquamarine,
            Color.Azure => DiscordColor.Azure,
            Color.Black => DiscordColor.Black,
            Color.Blue => DiscordColor.Blue,
            Color.Bplurple => DiscordColor.Blurple,
            Color.Brown => DiscordColor.Brown,
            Color.Chartreuse => DiscordColor.Chartreuse,
            Color.Cyan => DiscordColor.Cyan,
            Color.DarkBlue => DiscordColor.DarkBlue,
            Color.DarkGray => DiscordColor.DarkGray,
            Color.DarkGreen => DiscordColor.DarkGreen,
            Color.DarkRed => DiscordColor.DarkRed,
            Color.Gold => DiscordColor.Gold,
            Color.Goldenrod => DiscordColor.Goldenrod,
            Color.Gray => DiscordColor.Gray,
            Color.Green => DiscordColor.Green,
            Color.LightGray => DiscordColor.LightGray,
            Color.Orange => DiscordColor.Orange,
            Color.Pink => DiscordColor.HotPink,
            Color.Purple => DiscordColor.Purple,
            Color.Red => DiscordColor.Red,
            Color.SapGreen => DiscordColor.SapGreen,
            Color.Teal => DiscordColor.Teal,
            Color.White => DiscordColor.White,
            Color.Yellow => DiscordColor.Yellow,
            _ => DiscordColor.Cyan
        };

        var captionEmbed = new DiscordEmbedBuilder
        {
            Title = caption,
            ImageUrl = image.Url,
            Color = discordColor
        };

        return captionEmbed;
    }
}