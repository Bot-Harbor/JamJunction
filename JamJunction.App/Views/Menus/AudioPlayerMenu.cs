using DSharpPlus.Entities;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Views.Menus;

/// <summary>
/// Provides helper methods for building interactive menu components
/// used by the audio player, such as filter selection, queue navigation,
/// skipping tracks, and removing tracks from the queue.
/// </summary>
/// <remarks>
/// These menus are used within the Jam Junction audio player interface
/// to allow users to interact with the queue and player settings through
/// Discord select components.
/// </remarks>
public class AudioPlayerMenu
{
    /// <summary>
    /// Builds a select menu containing the available audio filters that can be
    /// applied to the current audio player.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordSelectComponent"/> containing filter options such as
    /// Nightcore, 8D, Vaporwave, Karaoke, and Slow Motion.
    /// </returns>
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

    /// <summary>
    /// Builds a select menu allowing users to skip directly to a specific track
    /// within the queue. The menu displays up to 15 tracks per page.
    /// </summary>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> containing the current playback queue.
    /// </param>
    /// <param name="pageNumber">
    /// The queue page to display. Each page contains up to 15 tracks.
    /// Defaults to page 1.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordSelectComponent"/> populated with track options that
    /// allow users to skip directly to the selected track.
    /// </returns>
    public DiscordSelectComponent BuildSkipTo(QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 1;

        switch (pageNumber)
        {
            case "1":
            {
                foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }

                break;
            }
            case "2":
                id = 16;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }

                break;
            case "3":
                id = 31;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "4":
                id = 46;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "5":
                id = 61;
                
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "6":
                id = 76;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "7":
                id = 91;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
        }


        var menu = new DiscordSelectComponent("queue-menu", "Select track to skip to", options);
        return menu;
    }

    /// <summary>
    /// Builds a select menu allowing users to remove tracks from the queue.
    /// The menu displays up to 15 tracks per page.
    /// </summary>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> containing the current playback queue.
    /// </param>
    /// <param name="pageNumber">
    /// The queue page to display. Each page contains up to 15 tracks.
    /// Defaults to page 1.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordSelectComponent"/> populated with track options that
    /// allow users to remove the selected track from the queue.
    /// </returns>
    public DiscordSelectComponent BuildRemove(QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var options = new List<DiscordSelectComponentOption>();

        var id = 0;

        switch (pageNumber)
        {
            case "1":
            {
                foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }

                break;
            }
            case "2":
                id = 15;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "3":
                id = 30;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "4":
                id = 45;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "5":
                id = 60;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "6":
                id = 75;
                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
            case "7":
                id = 90;

                foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                {
                    if (queue.Track!.Title.Length > 50)
                    {
                        options.Add(new DiscordSelectComponentOption($"{queue.Track!.Title.Substring(0, 50)}...",
                            id++.ToString(),
                            $"By {queue.Track!.Author}"));
                        continue;
                    }

                    options.Add(new DiscordSelectComponentOption(queue.Track!.Title, id++.ToString(),
                        $"By {queue.Track!.Author}"));
                }
                
                break;
        }

        var menu = new DiscordSelectComponent("remove", "Select track to remove", options);
        return menu;
    }
}