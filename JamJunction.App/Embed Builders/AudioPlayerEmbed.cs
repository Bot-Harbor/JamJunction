using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players.Queued;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordMessageBuilder TrackInformation(LavalinkTrack track, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var uri = track.Uri!.AbsoluteUri;

        if (track.Uri.AbsoluteUri.Contains("spotify.com")) uri = track.Uri!.AbsoluteUri.Replace("https://", "");

        var embed = new DiscordEmbedBuilder
        {
            Description = $"💿  •  **Title**: {track.Title}\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"🕒  •  **Duration**: {RoundSeconds(track.Duration)}\n" +
                          $"🔗  •  **Url:** {uri}",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = track.ArtworkUri!.AbsoluteUri
            }
        };

        if (track.Uri!.ToString().ToLower().Contains("soundcloud"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Soundcloud",
                IconUrl = "https://static-00.iconduck.com/assets.00/soundcloud-icon-2048x2048-j8bxnm2n.png"
            };

        if (track.Uri!.ToString().ToLower().Contains("youtube"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Youtube",
                IconUrl =
                    "https://cdn4.iconfinder.com/data/icons/social-messaging-ui-color-shapes-2-free/128/social-youtube-circle-512.png"
            };

        if (track.Uri!.ToString().ToLower().Contains("spotify"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Spotify",
                IconUrl =
                    "https://p7.hiclipart.com/preview/158/639/798/spotify-streaming-media-logo-playlist-spotify-app-icon.jpg"
            };

        var playerState = !queuedLavalinkPlayer.IsPaused ? "Off" : "On";
        var queue = queuedLavalinkPlayer.Queue;

        var queueFull = queue.Count >= 25;

        embed.AddField(
            "Player Status",
            $"Volume: `{queuedLavalinkPlayer.Volume * 100}` \n" +
            $"Paused: `{playerState}` \n" +
            $"Repeating Mode: `{queuedLavalinkPlayer.RepeatMode}` ", true);

        embed.AddField(
            "Queue Status",
            $"Number of Tracks: `{queue.Count}` \n" +
            $"Queue Full: `{queueFull}`", true);

        if (queue.Count != 0)
            foreach (var nextTrack in queue.Take(1))
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Next Track: {nextTrack.Track!.Title}"
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
            ButtonStyle.Success, "volume-down", "🔉 Volume -"
        );

        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volume-up", "🔊 Volume +"
        );

        var viewQueueButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "view-queue", "🎵 View Queue"
        );

        var restartButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "restart", "🔄 Restart"
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
        messageBuilder.AddEmbed(embed);

        foreach (var row in componentsRows) messageBuilder.AddComponents(row);

        return messageBuilder;
    }

    public DiscordMessageBuilder TrackInformation(ExtendedLavalinkTrack track,
        QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var uri = track.Uri!.AbsoluteUri;

        if (track.Uri.AbsoluteUri.Contains("spotify.com")) uri = track.Uri!.AbsoluteUri.Replace("https://", "");

        var embed = new DiscordEmbedBuilder
        {
            Description = $"💿  •  **Title**: {track.Title}\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"🕒  •  **Duration**: {RoundSeconds(track.Duration)}\n" +
                          $"🔗  •  **Url:** {uri}",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = track.ArtworkUri!.AbsoluteUri
            }
        };

        if (track.Uri!.ToString().ToLower().Contains("soundcloud"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Soundcloud",
                IconUrl = "https://static-00.iconduck.com/assets.00/soundcloud-icon-2048x2048-j8bxnm2n.png"
            };

        if (track.Uri!.ToString().ToLower().Contains("youtube"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Youtube",
                IconUrl =
                    "https://cdn4.iconfinder.com/data/icons/social-messaging-ui-color-shapes-2-free/128/social-youtube-circle-512.png"
            };

        if (track.Uri!.ToString().ToLower().Contains("spotify"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Spotify",
                IconUrl =
                    "https://p7.hiclipart.com/preview/158/639/798/spotify-streaming-media-logo-playlist-spotify-app-icon.jpg"
            };

        var playerState = !queuedLavalinkPlayer.IsPaused ? "Off" : "On";
        var queue = queuedLavalinkPlayer.Queue;

        var queueFull = queue.Count >= 25;

        embed.AddField(
            "Player Status",
            $"Volume: `{queuedLavalinkPlayer.Volume * 100}` \n" +
            $"Paused: `{playerState}` \n" +
            $"Repeating Mode: `{queuedLavalinkPlayer.RepeatMode}` ", true);


        embed.AddField(
            "Queue Status",
            $"Number of Tracks: `{queue.Count}` \n" +
            $"Queue Full: `{queueFull}`", true);

        if (queue.Count != 0)
            foreach (var nextTrack in queue.Take(1))
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Next Track: {nextTrack.Track!.Title}"
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
            ButtonStyle.Success, "volume-down", "🔉 Volume -"
        );

        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Success, "volume-up", "🔊 Volume +"
        );

        var viewQueueButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "view-queue", "🎵 View Queue"
        );

        var restartButton = new DiscordButtonComponent
        (
            ButtonStyle.Primary, "restart", "🔄 Restart"
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
        messageBuilder.AddEmbed(embed);

        foreach (var row in componentsRows) messageBuilder.AddComponents(row);

        return messageBuilder;
    }

    public DiscordEmbedBuilder TrackAddedToQueue(LavalinkTrack track)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{track!.Title}]({track.Uri}) - By **{track.Author}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder TrackAddedToQueue(ExtendedLavalinkTrack track)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{track!.Title}]({track.Uri}) - By **{track.Author}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder Pause(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{context.Member.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };
        return embed;
    }

    public DiscordEmbedBuilder Pause(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{btnInteractionArgs.User.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };
        return embed;
    }

    public DiscordEmbedBuilder Resume(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{context.Member.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };
        return embed;
    }

    public DiscordEmbedBuilder Resume(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{btnInteractionArgs.User.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };
        return embed;
    }

    public DiscordEmbedBuilder Stop(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⬜   • ``{context.Member.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder Stop(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⬜   • ``{btnInteractionArgs.User.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder QueueSomething()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "**Nothing is playing.**\n" +
                          "Please use the </play:1181715791658360852> command to queue something.",
            Color = DiscordColor.Orange,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Width = 50,
                Height = 50,
                Url = "https://media.lordicon.com/icons/wired/gradient/29-play-pause-circle.gif"
            }
        };
        return embed;
    }

    public DiscordEmbedBuilder Volume(double volume, InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{context.Member.Username}`` changed the volume to ``{volume}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder VolumeDecreased(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔉  •  ``{btnInteractionArgs.User.Username}`` has decreased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder VolumeIncreased(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` has increased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder Restart(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔄   • ``{context.Member.Username}`` restarted the track.",
            Color = DiscordColor.Orange
        };
        return embed;
    }

    public DiscordEmbedBuilder Restart(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔄   • ``{btnInteractionArgs.User.Username}`` restarted the track.",
            Color = DiscordColor.Orange
        };
        return embed;
    }

    public DiscordEmbedBuilder Leave(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔌   • ``{context.Member.Username}`` has disconnected Jam Junction.",
            Color = DiscordColor.DarkRed
        };
        return embed;
    }

    public DiscordEmbedBuilder ViewQueue(InteractionContext context, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var embed = new DiscordEmbedBuilder
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
            embed.Description = "There are no tracks currently in the queue.";
        }
        else
        {
            var i = 1;

            foreach (var queue in queuedLavalinkPlayer.Queue)
            {
                var title = queue.Track!.Title;
                var url = queue.Track.Uri;
                var author = queue.Track.Author;

                embed.AddField(
                    "\u200B",
                    title.Length > 20
                        ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                        : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", inline: true);
            }
        }

        return embed;
    }

    public DiscordEmbedBuilder ViewQueue(ComponentInteractionCreateEventArgs btnInteractionArgs,
        QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = " 🎵  Queue List:",
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = btnInteractionArgs.Guild.IconUrl
            }
        };

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            embed.Description = "There are no tracks currently in the queue.";
        }
        else
        {
            var i = 1;

            foreach (var queue in queuedLavalinkPlayer.Queue)
            {
                var title = queue.Track!.Title;
                var url = queue.Track.Uri;
                var author = queue.Track.Author;

                embed.AddField(
                    "\u200B",
                    title.Length > 20
                        ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                        : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", inline: true);
            }
        }

        return embed;
    }

    public DiscordEmbedBuilder ShuffleQueue(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔀  • ``{context.Member.Username}`` has shuffled the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder ShuffleQueue(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔀  • ``{btnInteractionArgs.User.Username}`` has shuffled the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder Skip(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭️  • ``{context.Member.Username}`` has skipped to the next track.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder Skip(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭️  • ``{btnInteractionArgs.User.Username}`` has skipped to the next track.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder SkipTo(ComponentInteractionCreateEventArgs menuInteractionArgs, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var track = queuedLavalinkPlayer.CurrentItem;
        
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭️  • ``{menuInteractionArgs.User.Username}`` has skipped to ``{track!.Track!.Title}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }
    
    public DiscordEmbedBuilder Seek(InteractionContext context, double seekedPosition)
    {
        var time = TimeSpan.FromSeconds(seekedPosition);

        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🕒   • ``{context.Member.Username}`` changed the track position to ``{time}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder TrackPosition(TimeSpan position)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🕒  • Current Track Position: ``{RoundSeconds(position)}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder Repeat(InteractionContext context, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        DiscordEmbedBuilder embed;

        if  (queuedLavalinkPlayer.RepeatMode == TrackRepeatMode.Track)
        {
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" 🔁  • ``{context.Member.Username}`` has enabled repeat track mode.",
                Color = DiscordColor.Cyan
            };
        }
        else if (queuedLavalinkPlayer.RepeatMode == TrackRepeatMode.Queue)
        {
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" 🔁  • ``{context.Member.Username}`` has enabled repeat queue mode.",
                Color = DiscordColor.Cyan
            };
        }
        else
        {
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" 🔁  • ``{context.Member.Username}`` has turned off repeat mode.",
                Color = DiscordColor.Cyan
            };
        }

        return embed;
    }

    public DiscordEmbedBuilder Filter()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🎵  • Filter applied. (It may take a few seconds to apply).",
            Color = DiscordColor.Cyan
        };
        return embed;
    }
    
    public DiscordEmbedBuilder Remove(ComponentInteractionCreateEventArgs menuInteractionArgs, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var track = queuedLavalinkPlayer.CurrentItem;
        
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🗑️  • ``{menuInteractionArgs.User.Username}`` has removed a track from the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }
    
    private TimeSpan RoundSeconds(TimeSpan timespan)
    {
        return TimeSpan.FromSeconds(Math.Round(timespan.TotalSeconds));
    }
}