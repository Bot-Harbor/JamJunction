using DSharpPlus.Entities;

namespace JamJunction.App.Menu_Builders;

public class AudioPlayerMenu
{
    public DiscordMessageBuilder Filters()
    {
        var options = new List<DiscordSelectComponentOption>
        {
            new("🔄 Reset", "reset"),
            new("🌙 Nightcore", "nightcore"),
            new("8️⃣ 8D", "8d"),
            new("🌊 Vaporwave", "vapor-wave"),
            new("🎤 Karaoke", "karaoke"),
            new("🕒 Slow Motion", "slow-motion")
        };

        var menu = new DiscordSelectComponent("filters-menu", "Select filter to apply", options);

        var builder = new DiscordMessageBuilder()
            .WithContent(" ")
            .AddComponents(menu);

        return builder;
    }
}