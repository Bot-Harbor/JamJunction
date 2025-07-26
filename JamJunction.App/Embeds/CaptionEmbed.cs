using DSharpPlus.Entities;
using Color = JamJunction.App.Embeds.Enums.Color;

namespace JamJunction.App.Embeds;

public class CaptionEmbed
{
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