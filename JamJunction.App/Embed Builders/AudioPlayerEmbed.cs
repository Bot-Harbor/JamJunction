using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Players.Queued;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordMessageBuilder SongEmbedBuilder(LavalinkTrack track, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var currentSongEmbed = new DiscordEmbedBuilder
        {
            Description = $"💿  •  **Now playing**: {track.Title}\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"⌛  •  **Song Duration** (HH:MM:SS): {RoundSeconds(track.Duration)}\n" +
                          $"🔴  •  **Is a Livestream**: {track.IsLiveStream}\n" +
                          $"🔗  •  **Url**: {track.Uri}",
            Color = DiscordColor.Teal,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = track.ArtworkUri!.AbsoluteUri
            }
        };

        var queue = queuedLavalinkPlayer.Queue;

        if (queue.Count == 0)
        {
            currentSongEmbed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Queue is empty..."
            };
        }
        else
        {
            foreach (var nextSong in queue.Take(1))
                currentSongEmbed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Next Song: {nextSong.Track!.Title}"
                };
        }

        var pauseButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "pause", "⏸ Pause"
        );

        var resumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "resume", "▶️ Resume"
        );

        var skipButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "skip", "⏭ Skip"
        );

        var stopButton = new DiscordButtonComponent
        (
            ButtonStyle.Danger, "stop", "⬜ Stop"
        );

        var volumeDownButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volumedown", "🔉 Volume -"
        );
        
        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volumeup", "🔊 Volume +"
        );

        var viewQueueButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "viewqueue", "🎵 View Queue"
        );

        var restartButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "restart", "🔁 Restart"
        );

        var shuffleButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "shuffle", "🔀 Shuffle"
        );

        var buttons = new List<DiscordComponent>
        {
            pauseButton, resumeButton, skipButton, stopButton, shuffleButton,
            volumeDownButton, volumeUpButton, viewQueueButton, restartButton
        };

        var componentsRows = new List<List<DiscordComponent>>();
        var currentRow = new List<DiscordComponent>();

        foreach (var button in buttons)
        {
            if (currentRow.Count == 5)
            {
                componentsRows.Add(currentRow);
                currentRow = new List<DiscordComponent>();
            }

            currentRow.Add(button);
        }

        if (currentRow.Count > 0) componentsRows.Add(currentRow);

        var messageBuilder = new DiscordMessageBuilder();
        messageBuilder.AddEmbed(currentSongEmbed);

        foreach (var row in componentsRows) messageBuilder.AddComponents(row);

        return messageBuilder;
    }
    
    public DiscordEmbedBuilder QueueEmbedBuilder(LavalinkTrack track)
    {
        var queueEmbed = new DiscordEmbedBuilder
        {
            Description = $"✅  •  **{track.Title}** has been added to the queue.",
            Color = DiscordColor.Green
        };

        return queueEmbed;
    }

    public DiscordEmbedBuilder PauseEmbedBuilder(InteractionContext context)
    {
        var pauseEmbed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{context.Member.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };

        return pauseEmbed;
    }

    public DiscordEmbedBuilder PauseEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var pauseButtonEmbed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{btnInteractionArgs.User.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };

        return pauseButtonEmbed;
    }

    public DiscordEmbedBuilder ResumeEmbedBuilder(InteractionContext context)
    {
        var resumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{context.Member.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };

        return resumeEmbed;
    }

    public DiscordEmbedBuilder ResumeEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var resumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{btnInteractionArgs.User.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };

        return resumeEmbed;
    }

    public DiscordEmbedBuilder StopEmbedBuilder(InteractionContext context)
    {
        var stopEmbed = new DiscordEmbedBuilder
        {
            Description = $"⬜   • ``{context.Member.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };

        return stopEmbed;
    }

    public DiscordEmbedBuilder StopEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var stopEmbed = new DiscordEmbedBuilder
        {
            Description = $"⬜   • ``{btnInteractionArgs.User.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };

        return stopEmbed;
    }

    public DiscordEmbedBuilder QueueSomethingEmbedBuilder()
    {
        var queueSomethingEmbed = new DiscordEmbedBuilder
        {
            Description = "**Nothing is playing.**\n" +
                          "Please use the ``/play`` command to queue something.",
            Color = DiscordColor.Orange,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Width = 50,
                Height = 50,
                Url = "https://media.lordicon.com/icons/wired/gradient/29-play-pause-circle.gif"
            }
        };

        return queueSomethingEmbed;
    }

    public DiscordEmbedBuilder VolumeEmbedBuilder(double volume, InteractionContext context)
    {
        var volumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{context.Member.Username}`` changed the volume to ``{volume}``.",
            Color = DiscordColor.Teal
        };

        return volumeEmbed;
    }

    public DiscordEmbedBuilder VolumeDecreaseEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var volumeDecreaseEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔉  •  ``{btnInteractionArgs.User.Username}`` has decreased the volume.",
            Color = DiscordColor.Teal
        };

        return volumeDecreaseEmbed;
    }

    public DiscordEmbedBuilder VolumeIncreaseEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var volumeIncreaseEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` has increased the volume.",
            Color = DiscordColor.Teal
        };

        return volumeIncreaseEmbed;
    }

    public DiscordEmbedBuilder MuteEmbedBuilder(InteractionContext context)
    {
        var muteEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{context.Member.Username}`` has muted the volume.",
            Color = DiscordColor.Teal
        };

        return muteEmbed;
    }

    public DiscordEmbedBuilder MuteEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var muteEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` has muted the volume.",
            Color = DiscordColor.Teal
        };

        return muteEmbed;
    }

    public DiscordEmbedBuilder UnmuteEmbedBuilder(InteractionContext context)
    {
        var unmuteEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{context.Member.Username}`` has unmuted the volume.",
            Color = DiscordColor.Teal
        };

        return unmuteEmbed;
    }

    public DiscordEmbedBuilder UnmuteEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var unmuteEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` has unmuted the volume.",
            Color = DiscordColor.Teal
        };

        return unmuteEmbed;
    }

    public DiscordEmbedBuilder SeekEmbedBuilder(InteractionContext context, double time)
    {
        var seekEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"⌛   • ``{context.Member.Username}`` changed the song position to ``{time}`` seconds.",
            Color = DiscordColor.Teal
        };

        return seekEmbed;
    }

    public DiscordEmbedBuilder RestartEmbedBuilder(InteractionContext context)
    {
        var restartEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"🔁   • ``{context.Member.Username}`` restarted the song.",
            Color = DiscordColor.Orange
        };

        return restartEmbed;
    }

    public DiscordEmbedBuilder RestartEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var restartEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"🔁   • ``{btnInteractionArgs.User.Username}`` restarted the song.",
            Color = DiscordColor.Orange
        };

        return restartEmbed;
    }

    public DiscordEmbedBuilder LeaveEmbedBuilder(InteractionContext context)
    {
        var leaveEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"🔌   • ``{context.Member.Username}`` has disconnected Jam Junction.",
            Color = DiscordColor.DarkRed
        };

        return leaveEmbed;
    }

    public DiscordEmbedBuilder ViewQueueBuilder(InteractionContext context, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var viewQueue = new DiscordEmbedBuilder
        {
            Title = " 🎵  Queue List:",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = context.Guild.IconUrl
            }
        };

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            viewQueue.Description = "There are no songs currently in the queue.";
        }
        else
        {
            var i = 1;

            viewQueue.Description = "• Only shows first **25** songs due to Discord's API rate limit";

            foreach (var queue in queuedLavalinkPlayer.Queue)
            {
                viewQueue.AddField($"{i++}. {queue.Track!.Title}",
                    $"**Song Duration** (HH:MM:SS): {RoundSeconds(queue.Track!.Duration)}");
            }
        }

        return viewQueue;
    }

    public DiscordEmbedBuilder ViewQueueBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        return new DiscordEmbedBuilder();
    }

    public DiscordEmbedBuilder ShuffleQueueBuilder(InteractionContext context)
    {
        var shuffleQueue = new DiscordEmbedBuilder
        {
            Description =
                $"🔀  • ``{context.Member.Username}`` has shuffled the queue.",
            Color = DiscordColor.Cyan
        };

        return shuffleQueue;
    }

    public DiscordEmbedBuilder ShuffleQueueBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var shuffleQueue = new DiscordEmbedBuilder
        {
            Description =
                $"🔀  • ``{btnInteractionArgs.User.Username}`` has shuffled the queue.",
            Color = DiscordColor.Cyan
        };

        return shuffleQueue;
    }
    
    public DiscordEmbedBuilder SkipEmbedBuilder(InteractionContext context)
    {
        var skipEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭️  • ``{context.Member.Username}`` has skipped to the next song.",
            Color = DiscordColor.Cyan
        };

        return skipEmbed;
    }
    
    public DiscordEmbedBuilder SkipEmbedBuilder(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var skipEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭️  • ``{btnInteractionArgs.User.Username}`` has skipped to the next song.",
            Color = DiscordColor.Cyan
        };

        return skipEmbed;
    }

    public DiscordEmbedBuilder SongPositionBuilder(TimeSpan timeSpan)
    {
        var songPosition = new DiscordEmbedBuilder
        {
            Description = $"🕒  • Current Song Position (HH:MM:SS): ``{RoundSeconds(timeSpan)}``",
            Color = DiscordColor.White
        };

        return songPosition;
    }

    private TimeSpan RoundSeconds(TimeSpan timespan)
    {
        return TimeSpan.FromSeconds(Math.Round(timespan.TotalSeconds));
    }
}