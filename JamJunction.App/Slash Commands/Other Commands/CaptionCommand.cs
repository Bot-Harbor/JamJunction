using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Enums;

namespace JamJunction.App.Slash_Commands.Other_Commands;

public class CaptionCommand : ApplicationCommandModule
{
    [SlashCommand("caption", "Give any image a caption.")]
    public async Task CaptionAsync
    (
        InteractionContext context,
        [Option("caption", "he caption you want the image to have.")]
        string caption,
        [Option("image", "The image you want to upload.")]
        DiscordAttachment image,
        [Option("color", "Pick the color you want the message embed to have.")] Color color
    )
    {
        try
        {
            var captionEmbed = new CaptionEmbed();

            await context.CreateResponseAsync(captionEmbed.CaptionEmbedBuilder(caption, image, color));
        }
        catch (FormatException)
        {
            var errorEmbed = new ErrorEmbed();

            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}