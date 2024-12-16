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
            new DiscordSelectComponentOption("🔊 Trebblebass", "trebblebass"),
            new DiscordSelectComponentOption("🌸 Soft", "soft"),
            new DiscordSelectComponentOption("🎵 Pop", "pop"),
            new DiscordSelectComponentOption("🔊 Bassboost", "bassboost"),
            new DiscordSelectComponentOption("🌊 Vaporwave", "vaporwave"),
            new DiscordSelectComponentOption("🎤 Karaoke", "karaoke"),
            new DiscordSelectComponentOption("🕒 Slow Motion", "slow-motion"),
            new DiscordSelectComponentOption("😈 Devil", "devil"),
        };
        
        var menu = new DiscordSelectComponent("filters-menu", "Select filter to apply", options);
        
        var builder = new DiscordMessageBuilder()
            .WithContent($" ")
            .AddComponents(menu);
        
        return builder;
    }
}