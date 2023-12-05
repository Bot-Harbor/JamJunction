using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Slash_Commands;

public class PingCommand : ApplicationCommandModule
{
    [SlashCommand("ping", "Will pong back to the server.")]
    public async Task PingAsync(InteractionContext context)
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

        await context.CreateResponseAsync(pingEmbed, ephemeral: true);
    }
}