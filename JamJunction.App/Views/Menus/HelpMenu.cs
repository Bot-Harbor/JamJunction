using DSharpPlus.Entities;

namespace JamJunction.App.Views.Menus;

public class HelpMenu
{
    public DiscordSelectComponent Build()
    {
        var options = new List<DiscordSelectComponentOption>
        {
            new("📜 Main Menu", "main"),
            new("🌐 All Features", "all-features"),
            new("🎵 Music Commands Explanation", "music-commands"),
            new("🛠️ Other Commands Explanation", "other-commands"),
            new("📻 Player Controls Explanation", "player-controls"),
        };

        var menu = new DiscordSelectComponent("help-menu", "Browse Help Menu", options);
        return menu;
    }
}