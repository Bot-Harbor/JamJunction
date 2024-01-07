using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordMessageBuilder CurrentSongEmbedBuilder(LavalinkTrack track)
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
                Width = 135,
                Height = 71,
                Url = "https://haulixdaily.com/wp-content/uploads/2018/" +
                      "08/tumblr_inline_pe4i0bR0o21s24py6_540.png"
            }
        };

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
            ButtonStyle.Success, "volume down", "🔉 Volume -"
        );

        var muteVolumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "mute", "🔇 Mute Volume"
        );

        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volume up", "🔊 Volume +"
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
            volumeDownButton, volumeUpButton, muteVolumeButton, restartButton
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
            Description = $"**Nothing is playing. Ambient mode enabled.**\n" +
                          $"Please use ``/play`` command to queue something.",
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
}