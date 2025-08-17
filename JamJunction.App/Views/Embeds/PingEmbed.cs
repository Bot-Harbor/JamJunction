using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Views.Embeds;

public class PingEmbed
{
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