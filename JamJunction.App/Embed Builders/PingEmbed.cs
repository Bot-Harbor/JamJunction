using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.EmbedBuilders;

public class PingEmbed
{
    public DiscordEmbedBuilder Ping(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = $"Pong 🏓 ``{context.Member.Username}``",
            ImageUrl = "https://pbs.twimg.com/media/CijH1M7WgAE_3we.jpg",
            Color = DiscordColor.Orange
        };

        return embed;
    }
}