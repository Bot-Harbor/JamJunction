using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Legacy.Embed_Builders;

public class PingEmbed
{
    public DiscordEmbedBuilder PingEmbedBuilder(InteractionContext context)
    {
        var pingEmbed = new DiscordEmbedBuilder
        {
            Title = $"Pong 🏓 ``{context.Member.Username}``",
            ImageUrl = "https://pbs.twimg.com/media/CijH1M7WgAE_3we.jpg",
            Color = DiscordColor.Orange
        };

        return pingEmbed;
    }
}