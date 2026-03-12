using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Views.Embeds;

/// <summary>
/// Represents the embed used to respond to the ping command.
/// </summary>
/// <remarks>
/// This embed displays a simple "Pong" message along with the
/// requesting user's name and a themed image.
/// </remarks>
public class PingEmbed
{
    /// <summary>
    /// Builds a simple "Pong" response embed used for the ping command,
    /// displaying the requesting user's name along with a themed image.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who triggered
    /// the ping command.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the pong response embed.
    /// </returns>
    public DiscordEmbedBuilder Build(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = $"Pong 🏓 ``{context.Member.Username}``",
            ImageUrl = "https://pbs.twimg.com/media/CijH1M7WgAE_3we.jpg",
            Color = DiscordColor.Cyan
        };

        return embed;
    }
}