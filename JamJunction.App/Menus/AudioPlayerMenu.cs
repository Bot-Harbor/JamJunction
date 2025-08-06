using DSharpPlus.Entities;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Menus;

public class AudioPlayerMenu
{
    public DiscordSelectComponent BuildFilters()
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
        return menu;
    }
    
    public DiscordSelectComponent BuildSkipTo(QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 1;

        switch (pageNumber)
        {
            case "1":
            {
                foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            }
            case "2":
                id = 16;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "3":
                id = 31;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "4":
                id = 46;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "5":
                id = 61;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "6":
                id = 76;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "7":
                id = 91;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
        }
        
        
        var menu = new DiscordSelectComponent("queue-menu", "Select track to skip to", options);
        return menu;
    }

    public DiscordSelectComponent BuildRemove(QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 0;
        
        switch (pageNumber)
        {
            case "1":
            {
                foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            }
            case "2":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "3":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "4":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "5":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "6":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
            case "7":
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                break;
        }

        var menu = new DiscordSelectComponent("remove", "Select track to remove", options);
        return menu;
    }
}