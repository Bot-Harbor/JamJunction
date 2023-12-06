using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.EmbedBuilders;

public class PingEmbed
{
    public DiscordEmbedBuilder PingEmbedBuilder(InteractionContext context)
    {
        var pingEmbed = new DiscordEmbedBuilder()
        {
            Title = $"Pong 🏓 ``{context.Member.Username}``",
            ImageUrl = "https://pbs.twimg.com/media/CijH1M7WgAE_3we.jpg",
            Color = DiscordColor.Orange,
        };

        var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

        pingEmbed.WithFooter
        (
            $"Time Stamp: {easternTime.ToString($"MMMM dd, yyyy h:mm tt")}"
        );

        return pingEmbed;
    }
}