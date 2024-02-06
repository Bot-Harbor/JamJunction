using System.ComponentModel;
using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordMessageBuilder SongEmbedBuilder(LavalinkTrack track, InteractionContext context)
    {
        var currentSongEmbed = new DiscordEmbedBuilder()
        {
            Description = $"💿  •  **Now playing**: {track.Title}\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"🔗  •  **Link:** {track.Uri.AbsoluteUri}\n" +
                          $"⌛  •  **Song Duration** (HH:MM:SS): {track.Length}",
            Color = DiscordColor.Teal,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
            {
                Url = context.Guild.IconUrl
            }
        };

        var nextSongs = PlayCommand.Queue.Skip(1);

        foreach (var nextSong in nextSongs.Take(1))
        {
            currentSongEmbed.Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = $"Next Song: {nextSong.Title}"
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
            ButtonStyle.Success, "volumedown", "🔉 -"
        );

        var muteVolumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "mute", "🔇 Mute"
        );

        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volumeup", "🔊 +"
        );

        var viewQueueButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "viewqueue", "🎶 View Queue"
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
            volumeDownButton, volumeUpButton, muteVolumeButton, viewQueueButton, restartButton
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

        if (currentRow.Count > 0)
        {
            componentsRows.Add(currentRow);
        }

        var messageBuilder = new DiscordMessageBuilder();
        messageBuilder.AddEmbed(currentSongEmbed);

        foreach (var row in componentsRows)
        {
            messageBuilder.AddComponents(row);
        }

        return messageBuilder;
    }

    public DiscordEmbedBuilder QueueEmbedBuilder(LavalinkTrack track)
    {
        var queueEmbed = new DiscordEmbedBuilder()
        {
            Description = $"✅  •  **{track.Title}** has been added to the queue.",
            Color = DiscordColor.Green,
        };

        return queueEmbed;
    }

    public DiscordEmbedBuilder PauseEmbedBuilder(InteractionContext context)
    {
        var pauseEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟡  • ``{context.Member.Username}`` paused the track!",
            Color = DiscordColor.Yellow
        };

        return pauseEmbed;
    }

    public DiscordEmbedBuilder PauseEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var pauseButtonEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟡  • ``{e.User.Username}`` paused the track!",
            Color = DiscordColor.Yellow
        };

        return pauseButtonEmbed;
    }

    public DiscordEmbedBuilder ResumeEmbedBuilder(InteractionContext context)
    {
        var resumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟢  • ``{context.Member.Username}`` resumed the track!",
            Color = DiscordColor.Green
        };

        return resumeEmbed;
    }

    public DiscordEmbedBuilder ResumeEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var resumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟢  • ``{e.User.Username}`` resumed the track!",
            Color = DiscordColor.Green
        };

        return resumeEmbed;
    }

    public DiscordEmbedBuilder StopEmbedBuilder(InteractionContext context)
    {
        var stopEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔴   • ``{context.Member.Username}`` stopped the player!",
            Color = DiscordColor.Red
        };

        return stopEmbed;
    }

    public DiscordEmbedBuilder StopEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var stopEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔴   • ``{e.User.Username}`` stopped the player!",
            Color = DiscordColor.Red
        };

        return stopEmbed;
    }

    public DiscordEmbedBuilder QueueSomethingEmbedBuilder()
    {
        var queueSomethingEmbed = new DiscordEmbedBuilder()
        {
            Description = $"**Nothing is playing.**\n" +
                          $"Please use the ``/play`` command to queue something.",
            Color = DiscordColor.Orange,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
            {
                Width = 50,
                Height = 50,
                Url = "https://lordicon.com/icons/wired/gradient/29-play-pause-circle.gif"
            }
        };

        return queueSomethingEmbed;
    }

    public DiscordEmbedBuilder VolumeEmbedBuilder(int volume, InteractionContext context)
    {
        var volumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{context.Member.Username}`` changed the volume to ``{volume}``!",
            Color = DiscordColor.Teal
        };

        return volumeEmbed;
    }

    public DiscordEmbedBuilder VolumeDecreaseEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var volumeDecreaseEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{e.User.Username}`` has decreased the volume!",
            Color = DiscordColor.Teal
        };

        return volumeDecreaseEmbed;
    }

    public DiscordEmbedBuilder VolumeIncreaseEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var volumeIncreaseEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{e.User.Username}`` has increased the volume!",
            Color = DiscordColor.Teal
        };

        return volumeIncreaseEmbed;
    }

    public DiscordEmbedBuilder MuteEmbedBuilder(InteractionContext context)
    {
        var muteEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{context.Member.Username}`` has muted the volume!",
            Color = DiscordColor.Teal
        };

        return muteEmbed;
    }

    public DiscordEmbedBuilder MuteEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var muteEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{e.User.Username}`` has muted the volume!",
            Color = DiscordColor.Teal
        };

        return muteEmbed;
    }

    public DiscordEmbedBuilder UnmuteEmbedBuilder(InteractionContext context)
    {
        var unmuteEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{context.Member.Username}`` has unmuted the volume!",
            Color = DiscordColor.Teal
        };

        return unmuteEmbed;
    }

    public DiscordEmbedBuilder UnmuteEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var unmuteEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{e.User.Username}`` has unmuted the volume!",
            Color = DiscordColor.Teal
        };

        return unmuteEmbed;
    }

    public DiscordEmbedBuilder SeekEmbedBuilder(InteractionContext context, double time)
    {
        var seekEmbed = new DiscordEmbedBuilder()
        {
            Description =
                $"⌛   • ``{context.Member.Username}`` changed the song position to ``{time}`` seconds!",
            Color = DiscordColor.Teal
        };

        return seekEmbed;
    }

    public DiscordEmbedBuilder RestartEmbedBuilder(InteractionContext context)
    {
        var restartEmbed = new DiscordEmbedBuilder()
        {
            Description =
                $"⌛   • ``{context.Member.Username}`` restarted the song!",
            Color = DiscordColor.Orange
        };

        return restartEmbed;
    }

    public DiscordEmbedBuilder RestartEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var restartEmbed = new DiscordEmbedBuilder()
        {
            Description =
                $"⌛   • ``{e.User.Username}`` restarted the song!",
            Color = DiscordColor.Orange
        };

        return restartEmbed;
    }

    public DiscordEmbedBuilder LeaveEmbedBuilder(InteractionContext context)
    {
        var leaveEmbed = new DiscordEmbedBuilder()
        {
            Description =
                $"🔌   • ``{context.Member.Username}`` has disconnected Jam Junction!",
            Color = DiscordColor.DarkRed
        };

        return leaveEmbed;
    }

    public DiscordEmbedBuilder ViewQueueBuilder(InteractionContext context)
    {
        var songQueue = PlayCommand.Queue.Skip(1);
        
        var viewQueue = new DiscordEmbedBuilder()
        {
            Title = " 🎵  Queue List:",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
            {
                Url = context.Guild.IconUrl
            }
        };
        
        if (!songQueue.ToList().Any())
        {
            viewQueue.Description = "There are no songs currently in the queue.";
        }
        else
        {
            var i = 1;

            viewQueue.Description = "• Only shows first **25** songs due to Discord's API rate limit";

            foreach (var song in songQueue.Take(25))
            {
                viewQueue.AddField($"{i++}. {song.Title}",
                    $"**Song Duration** (HH:MM:SS): {song.Length}");
            }
        }

        return viewQueue;
    }

    public DiscordEmbedBuilder ViewQueueBuilder(ComponentInteractionCreateEventArgs e)
    {
        var songQueue = PlayCommand.Queue.Skip(1);

        var viewQueue = new DiscordEmbedBuilder()
        {
            Title = " 🎵  Queue List:",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
            {
                Url = e.Guild.IconUrl
            }
        };

        if (!songQueue.ToList().Any())
        {
            viewQueue.Description = "There are no songs currently in the queue.";
        }
        else
        {
            var i = 1;

            viewQueue.Description = "• Only shows first **25** songs due to Discord's API rate limit";

            foreach (var song in songQueue.Take(25))
            {
                viewQueue.AddField($"{i++}. {song.Title}",
                    $"**Song Duration** (HH:MM:SS): {song.Length}");
            }
        }

        return viewQueue;
    }

    public DiscordEmbedBuilder ShuffleQueueBuilder(InteractionContext context)
    {
        var shuffleQueue = new DiscordEmbedBuilder()
        {
            Description =
                $"🔀  • ``{context.Member.Username}`` has shuffled the queue!",
            Color = DiscordColor.Cyan
        };

        return shuffleQueue;
    }

    public DiscordEmbedBuilder ShuffleQueueBuilder(ComponentInteractionCreateEventArgs e)
    {
        var shuffleQueue = new DiscordEmbedBuilder()
        {
            Description =
                $"🔀  • ``{e.User.Username}`` has shuffled the queue!",
            Color = DiscordColor.Cyan
        };

        return shuffleQueue;
    }
}