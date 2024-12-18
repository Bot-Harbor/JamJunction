using DSharpPlus.Entities;

namespace JamJunction.App.Menu_Builders;

public class AudioPlayerMenu
{
    public DiscordMessageBuilder Filters()
    {
        var options = new List<DiscordSelectComponentOption>
        {
            new DiscordSelectComponentOption("🔄 Reset", "reset"),
            new DiscordSelectComponentOption("🌙 Nightcore", "nightcore"),
            new DiscordSelectComponentOption("8️⃣ 8D", "8d"), 
            new DiscordSelectComponentOption("🌊 Vaporwave", "vapor-wave"),
            new DiscordSelectComponentOption("🎤 Karaoke", "karaoke"),
            new DiscordSelectComponentOption("🕒 Slow Motion", "slow-motion"),
        };
        
        var menu = new DiscordSelectComponent("filters-menu", "Select filter to apply", options);
        
        var builder = new DiscordMessageBuilder()
            .WithContent($" ")
            .AddComponents(menu);
        
        return builder;
    }
}