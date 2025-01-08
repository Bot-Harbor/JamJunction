using DSharpPlus.Entities;
using Lavalink4NET.Players.Queued;

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

    public DiscordMessageBuilder Queue(QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 1;
        foreach (var queue in queuedLavalinkPlayer.Queue)
            options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                $"By {queue.Track!.Author}"));

        var menu = new DiscordSelectComponent("queue-menu", "Select track to skip to", options);

        var builder = new DiscordMessageBuilder()
            .WithContent(" ")
            .AddComponents(menu);

        return builder;
    }

    public DiscordMessageBuilder Remove(QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 0;
        foreach (var queue in queuedLavalinkPlayer.Queue)
            options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                $"By {queue.Track!.Author}"));

        var menu = new DiscordSelectComponent("remove", "Select track to remove", options);

        var builder = new DiscordMessageBuilder()
            .WithContent(" ")
            .AddComponents(menu);

        return builder;
    }
}