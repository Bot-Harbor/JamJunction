using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Menus;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using SpotifyAPI.Web;
using YoutubeExplode.Playlists;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Views.Embeds;

/// <summary>
/// Provides embed builders used for displaying audio player information,
/// playback actions, queue updates, and player status messages.
/// </summary>
/// <remarks>
/// These embeds are used throughout the Jam Junction audio player system
/// to present track details, queue information, playback controls,
/// and user interaction responses.
/// </remarks>
public class AudioPlayerEmbed
{
    /// <summary>
    /// Builds the main audio player message displaying the currently playing track,
    /// playback progress slider, player status information, queue details, and
    /// interactive control buttons.
    /// </summary>
    /// <param name="track">
    /// The current <see cref="LavalinkTrack"/> that is being played.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance that manages playback,
    /// queue state, filters, and player controls.
    /// </param>
    /// <param name="isStartedFromEvent">
    /// Indicates whether the track playback was triggered from a track started event.
    /// When true, the progress slider begins at the start of the track.
    /// </param>
    /// <param name="trackIsRestarted">
    /// Indicates whether the current track has been restarted. If true,
    /// the progress slider resets to the beginning.
    /// </param>
    /// <param name="pauseDisabled">
    /// Determines whether the pause button should be disabled in the player controls.
    /// </param>
    /// <param name="resumeDisabled">
    /// Determines whether the resume button should be disabled in the player controls.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the track information embed
    /// and all interactive player components.
    /// </returns>
    public DiscordMessageBuilder TrackInformation(LavalinkTrack track, QueuedLavalinkPlayer queuedLavalinkPlayer,
        bool isStartedFromEvent = false, bool trackIsRestarted = false, bool pauseDisabled = false,
        bool resumeDisabled = true)
    {
        var uri = track.Uri!.AbsoluteUri;

        string slider;

        if (isStartedFromEvent)
            slider = GenerateSlider(TimeSpan.Zero, track.Duration);
        else if (trackIsRestarted)
            slider = GenerateSlider(TimeSpan.Zero, track.Duration);
        else
            slider = GenerateSlider(queuedLavalinkPlayer.Position!.Value.Position, track.Duration);

        var embed = new DiscordEmbedBuilder
        {
            Color = DiscordColor.Cyan,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = track.ArtworkUri!.AbsoluteUri
            }
        };

        if (track.Title.Length > 35)
        {
            embed.Description = $"💿  •  **Title**: [{track.Title.Substring(0, 35)}...]({uri})\n" +
                                $"🎙️  •  **Artist**: {track.Author}\n" +
                                $"{slider}";
        }
        else
        {
            embed.Description = $"💿  •  **Title**: [{track.Title}]({uri})\n" +
                                $"🎙️  •  **Artist**: {track.Author}\n" +
                                $"{slider}";
        }

        if (track.Uri!.ToString().ToLower().Contains("spotify.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Spotify",
                IconUrl =
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/8/84/Spotify_icon.svg/3840px-Spotify_icon.svg.png"
            };

        if (track.Uri!.ToString().ToLower().Contains("youtube.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: YouTube",
                IconUrl =
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/YouTube_full-color_icon_%282017%29.svg/3840px-YouTube_full-color_icon_%282017%29.svg.png"
            };


        if (track.Uri!.ToString().ToLower().Contains("deezer.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Deezer",
                IconUrl =
                    "https://companieslogo.com/img/orig/DEEZR.PA-dbdcf2cf.png?t=1721547851"
            };

        if (track.Uri!.ToString().ToLower().Contains("soundcloud.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: SoundCloud",
                IconUrl = "https://cdn-icons-png.flaticon.com/512/145/145809.png"
            };
        
        if (track.Uri!.ToString().ToLower().Contains("music.youtube.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: YouTube Music",
                IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Youtube_Music_icon.svg/960px-Youtube_Music_icon.svg.png"
            };
        
        if (track.Uri!.ToString().ToLower().Contains("music.apple.com"))
            embed.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Platform: Apple Music",
                IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5f/Apple_Music_icon.svg/500px-Apple_Music_icon.svg.png"
            };

        var playerState = !queuedLavalinkPlayer.IsPaused ? "Off" : "On";
        var queue = queuedLavalinkPlayer.Queue;

        var queueFull = queue.Count >= 100;

        embed.AddField(
            "Player Controls Status",
            $"Volume: `{Math.Round(queuedLavalinkPlayer.Volume * 100)}` \n" +
            $"Paused: `{playerState}` \n" +
            $"Repeating Mode: `{queuedLavalinkPlayer.RepeatMode}` \n" +
            $"Applied Filter: `{AppliedFilter(queuedLavalinkPlayer)}`", true);

        embed.AddField(
            "Queue Status",
            $"Number of Tracks: `{queue.Count}` \n" +
            $"Queue Full: `{queueFull}`", true);

        if (queue.Count != 0)
            foreach (var nextTrack in queue.Take(1))
                if (nextTrack.Track!.Title.Length > 50)
                {
                    embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Next Track: {nextTrack.Track!.Title.Substring(0, 50)}...\n\nMade With ❤️"
                    };
                }
                else
                {
                    embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Next Track: {nextTrack.Track!.Title}\n\nMade With ❤️"
                    };
                }
        else
        {
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Made With ❤️",
            };
        }

        var pauseButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "pause", "⏸", pauseDisabled
        );

        var resumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "resume", "▶", resumeDisabled
        );

        DiscordButtonComponent skipButton;

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            skipButton = new DiscordButtonComponent
            (
                ButtonStyle.Secondary, "skip", "⏭", disabled: true
            );
        }
        else
        {
            skipButton = new DiscordButtonComponent
            (
                ButtonStyle.Secondary, "skip", "⏭"
            );
        }

        var stopButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "stop", "⏹"
        );

        var volumeDownButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "volume-down", "🔉"
        );

        var volumeUpButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "volume-up", "🔊"
        );

        var viewQueueButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "view-queue", "☰"
        );

        var restartButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "restart", "↻"
        );

        var shuffleButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "shuffle", "⇌"
        );

        var repeatButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "repeat", "⇄"
        );

        var buttons = new List<DiscordComponent>
        {
            pauseButton, resumeButton, skipButton, stopButton, viewQueueButton,
            volumeDownButton, volumeUpButton, restartButton, repeatButton, shuffleButton
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

        var audioPlayerMenu = new AudioPlayerMenu();
        messageBuilder.AddComponents(audioPlayerMenu.BuildFilters());

        foreach (var row in componentsRows) messageBuilder.AddComponents(row);

        return messageBuilder;
    }

    /// <summary>
    /// Determines which audio filter is currently applied to the Lavalink player.
    /// </summary>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> whose active audio filters are inspected.
    /// </param>
    /// <returns>
    /// A string representing the name of the currently applied filter,
    /// such as Nightcore, Vaporwave, Slow Motion, Karaoke, or 8D.
    /// Returns "None" if no filter is applied.
    /// </returns>
    private string AppliedFilter(QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        if (queuedLavalinkPlayer.Filters.Timescale != null)
            switch (queuedLavalinkPlayer.Filters.Timescale.Speed)
            {
                case 1.25f when
                    queuedLavalinkPlayer.Filters.Timescale.Pitch == 1.2f &&
                    queuedLavalinkPlayer.Filters.Timescale.Rate == 1.0f:
                    return "Nightcore";
                case 0.8f when
                    queuedLavalinkPlayer.Filters.Timescale.Pitch == 0.85f &&
                    queuedLavalinkPlayer.Filters.Timescale.Rate == 1.0f:
                    return "Vaporwave";
                case 0.5f:
                    return "Slow Motion";
            }

        if (queuedLavalinkPlayer.Filters.Karaoke != null) return "Karaoke";

        if (queuedLavalinkPlayer.Filters.Rotation != null) return "8D";

        return "None";
    }

    /// <summary>
    /// Generates a visual progress slider representing the current playback
    /// position relative to the total track duration.
    /// </summary>
    /// <param name="position">
    /// The current playback position of the track.
    /// </param>
    /// <param name="duration">
    /// The total duration of the track.
    /// </param>
    /// <param name="barLength">
    /// The number of characters used to render the progress bar. Defaults to 20.
    /// </param>
    /// <returns>
    /// A formatted string containing the current time, progress slider,
    /// and total duration of the track.
    /// </returns>
    private string GenerateSlider(TimeSpan position, TimeSpan duration, int barLength = 20)
    {
        if (duration.TotalSeconds == 0)
            return "`00:00` 🔘 `00:00`";

        var progress = position.TotalSeconds / duration.TotalSeconds;
        var progressIndex = (int)(progress * barLength);

        if (progressIndex >= barLength)
            progressIndex = barLength - 1;

        var bar = new StringBuilder();

        for (var i = 0; i < barLength; i++) bar.Append(i == progressIndex ? "🔘" : "─");

        return $"`{FormatTime(position)}` {bar} `{FormatTime(duration)}`";
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> into a formatted time string.
    /// </summary>
    /// <param name="time">
    /// The time value to format.
    /// </param>
    /// <returns>
    /// A string formatted as mm:ss or hh:mm:ss depending on the length of the time.
    /// </returns>
    private string FormatTime(TimeSpan time)
    {
        return time.Hours > 0
            ? time.ToString(@"hh\:mm\:ss")
            : time.ToString(@"mm\:ss");
    }
    
    /// <summary>
    /// Builds an embed message indicating that a track was added to the queue.
    /// </summary>
    /// <param name="track">
    /// The <see cref="LavalinkTrack"/> that was added to the playback queue.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued track.
    /// </returns>
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

    /// <summary>
    /// Builds an embed message indicating that a Spotify album was added to the queue.
    /// </summary>
    /// <param name="fullAlbum">
    /// The Spotify <see cref="FullAlbum"/> containing the album metadata.
    /// </param>
    /// <param name="albumUrl">
    /// The URL linking to the Spotify album.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued album.
    /// </returns>
    public DiscordEmbedBuilder AlbumAddedToQueue(FullAlbum fullAlbum, string albumUrl)
    {
        var artistName = fullAlbum.Artists.FirstOrDefault()!.Name;

        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{fullAlbum.Name}]({albumUrl}) - By **{artistName}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a Deezer album was added to the queue.
    /// </summary>
    /// <param name="trackLoadResult">
    /// The <see cref="TrackLoadResult"/> returned from Lavalink containing
    /// the loaded playlist or album information.
    /// </param>
    /// <param name="albumUrl">
    /// The URL linking to the Deezer album.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued album.
    /// </returns>
    public DiscordEmbedBuilder AlbumAddedToQueue(TrackLoadResult trackLoadResult, string albumUrl)
    {
        var albumName = trackLoadResult.Playlist!.Name;
        var authorName = trackLoadResult.Track!.Author;

        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{albumName}]({albumUrl}) - By **{authorName}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a Spotify playlist was added to the queue.
    /// </summary>
    /// <param name="fullPlaylist">
    /// The Spotify <see cref="FullPlaylist"/> containing the playlist metadata.
    /// </param>
    /// <param name="playlistUrl">
    /// The URL linking to the Spotify playlist.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued playlist.
    /// </returns>
    public DiscordEmbedBuilder PlaylistAddedToQueue(FullPlaylist fullPlaylist, string playlistUrl)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{fullPlaylist.Name}]({playlistUrl}) - By **{fullPlaylist.Owner!.DisplayName}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a YouTube or YouTube Music playlist was added to the queue.
    /// </summary>
    /// <param name="playlist">
    /// The <see cref="Playlist"/> object containing the playlist title and URL.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued playlist.
    /// </returns>
    public DiscordEmbedBuilder PlaylistAddedToQueue(Playlist playlist)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{playlist.Title}]({playlist.Url})",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a Deezer or SoundCloud playlist was added to the queue.
    /// </summary>
    /// <param name="playlist">
    /// The <see cref="TrackLoadResult"/> containing the playlist metadata.
    /// </param>
    /// <param name="playlistUrl">
    /// The URL linking to the playlist.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queued playlist.
    /// </returns>
    public DiscordEmbedBuilder PlaylistAddedToQueue(TrackLoadResult playlist, string playlistUrl)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{playlist.Playlist!.Name}]({playlistUrl})",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a track was paused using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who paused the track.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the pause action.
    /// </returns>
    public DiscordEmbedBuilder Pause(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{context.Member.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a track was paused using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the button.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the pause action.
    /// </returns>
    public DiscordEmbedBuilder Pause(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  • ``{btnInteractionArgs.User.Username}`` paused the track.",
            Color = DiscordColor.Yellow
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a track was resumed using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who resumed the track.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the resume action.
    /// </returns>
    public DiscordEmbedBuilder Resume(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{context.Member.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };
        return embed;
    }
    
    /// <summary>
    /// Builds an embed message indicating that a track was resumed using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the button.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the resume action.
    /// </returns>
    public DiscordEmbedBuilder Resume(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"▶  • ``{btnInteractionArgs.User.Username}`` resumed the track.",
            Color = DiscordColor.Green
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the audio player was stopped using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who stopped the player.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the stop action.
    /// </returns> 
    public DiscordEmbedBuilder Stop(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏹   • ``{context.Member.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };
        return embed;
    }
    
    /// <summary>
    /// Builds an embed message indicating that the audio player was stopped using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the button.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the stop action.
    /// </returns>
    public DiscordEmbedBuilder Stop(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏹   • ``{btnInteractionArgs.User.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message informing users that no track is currently playing
    /// and prompts them to use the play command to add music to the queue.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> displaying the "nothing playing" message.
    /// </returns>
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

    /// <summary>
    /// Builds an embed message indicating that a user changed the player's volume using a slash command.
    /// </summary>
    /// <param name="volume">
    /// The new volume level applied to the audio player.
    /// </param>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who executed the command.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the volume change action.
    /// </returns>
    public DiscordEmbedBuilder Volume(double volume, InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{context.Member.Username}`` changed the volume to ``{volume}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume was decreased using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the volume decrease.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the volume decrease action.
    /// </returns>
    public DiscordEmbedBuilder VolumeDecreased(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔉  •  ``{btnInteractionArgs.User.Username}`` decreased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume was increased using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the volume increase.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the volume increase action.
    /// </returns>
    public DiscordEmbedBuilder VolumeIncreased(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` increased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user restarted the currently playing track
    /// using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who restarted the track.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the restart action.
    /// </returns>
    public DiscordEmbedBuilder Restart(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"↻   • ``{context.Member.Username}`` restarted the track.",
            Color = DiscordColor.Orange
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user restarted the currently playing track
    /// using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the restart.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the restart action.
    /// </returns>
    public DiscordEmbedBuilder Restart(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"↻   • ``{btnInteractionArgs.User.Username}`` restarted the track.",
            Color = DiscordColor.Orange
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user disconnected the bot from the voice channel.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who disconnected the bot.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the disconnect action.
    /// </returns>
    public DiscordEmbedBuilder Leave(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔌   • ``{context.Member.Username}`` disconnected Jam Junction.",
            Color = DiscordColor.DarkRed
        };
        return embed;
    }

    /// <summary>
    /// Builds a message displaying the current music queue for the guild.
    /// Shows up to 15 tracks per page along with controls for skipping to
    /// a specific track or removing tracks from the queue.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> that triggered the request. 
    /// Used to access guild information such as the server icon.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance containing the
    /// current playback queue and player state.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the queue embed and
    /// any interactive components such as skip, remove, and pagination buttons.
    /// </returns>
    public DiscordMessageBuilder ViewQueue(InteractionContext context, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var messageBuilder = new DiscordMessageBuilder();

        var embed = new DiscordEmbedBuilder
        {
            Title = "☰  Queue List:",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = context.Guild.IconUrl
            }
        };

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            embed.Color = DiscordColor.Red;
            embed.Description = "There are no tracks currently in the queue.";
            messageBuilder.AddEmbed(embed);
        }
        else
        {
            embed.Color = DiscordColor.Cyan;

            var i = 1;

            foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
            {
                var title = queue.Track!.Title;
                var url = queue.Track.Uri;
                var author = queue.Track.Author;

                embed.AddField(
                    "\u200B",
                    title.Length > 20
                        ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                        : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
            }

            messageBuilder.AddEmbed(embed);

            var audioPlayerMenu = new AudioPlayerMenu();
            messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer));
            messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer));

            if (queuedLavalinkPlayer.Queue.Count > 15)
            {
                var beginningButton = new DiscordButtonComponent
                (
                    ButtonStyle.Secondary, "beginning", "<<", true
                );

                var backButton = new DiscordButtonComponent
                (
                    ButtonStyle.Secondary, "back", "<", true
                );

                var pageNumberButton = new DiscordButtonComponent
                (
                    ButtonStyle.Secondary, "page-number", "1"
                );

                var nextButton = new DiscordButtonComponent
                (
                    ButtonStyle.Secondary, "next", ">"
                );

                var endButton = new DiscordButtonComponent
                (
                    ButtonStyle.Secondary, "end", ">>"
                );

                var buttons = new List<DiscordComponent>
                {
                    beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                foreach (var row in componentsRows) messageBuilder.AddComponents(row);
            }
        }

        return messageBuilder;
    }

    /// <summary>
    /// Builds a paginated message displaying the current music queue when
    /// triggered from a button interaction. The queue is divided into pages
    /// of 15 tracks and allows users to navigate between pages.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the
    /// interaction data for the button press, including the user and guild.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance that holds the
    /// current queue and playback information.
    /// </param>
    /// <param name="pageNumber">
    /// The page number of the queue to display. Each page contains up to
    /// 15 tracks. Defaults to page 1.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the queue embed and
    /// pagination controls used to navigate between queue pages.
    /// </returns>
    public DiscordMessageBuilder ViewQueue(ComponentInteractionCreateEventArgs btnInteractionArgs,
        QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var messageBuilder = new DiscordMessageBuilder();

        var embed = new DiscordEmbedBuilder
        {
            Title = "☰  Queue List:",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = btnInteractionArgs.Interaction.Guild.IconUrl
            }
        };

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            embed.Color = DiscordColor.Red;
            embed.Description = "There are no tracks currently in the queue.";
            messageBuilder.AddEmbed(embed);
        }
        else
        {
            embed.Color = DiscordColor.Cyan;

            var i = 1;

            var userId = btnInteractionArgs.Interaction.User.Id;
            var userData = Bot.UserData[userId];

            switch (pageNumber)
            {
                case "1":
                {
                    userData.CurrentPageNumber = "1";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer));

                    if (queuedLavalinkPlayer.Queue.Count > 15)
                    {
                        var beginningButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "beginning", "<<", true
                        );

                        var backButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "back", "<", true
                        );

                        var pageNumberButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                        );

                        var nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        var endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );

                        var buttons = new List<DiscordComponent>
                        {
                            beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                        foreach (var row in componentsRows) messageBuilder.AddComponents(row);
                    }

                    break;
                }
                case "2":
                {
                    i = 16;

                    userData.CurrentPageNumber = "2";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "2"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "2"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 31)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "3":
                {
                    i = 31;

                    userData.CurrentPageNumber = "3";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "3"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "3"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 46)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "4":
                {
                    i = 46;

                    userData.CurrentPageNumber = "4";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "4"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "4"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 61)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "5":
                {
                    i = 61;

                    userData.CurrentPageNumber = "5";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "5"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "5"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 76)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "6":
                {
                    i = 76;

                    userData.CurrentPageNumber = "6";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "6"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "6"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 91)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "7":
                {
                    i = 91;

                    userData.CurrentPageNumber = "7";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "7"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "7"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );

                    var nextButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "next", ">", true
                    );

                    var endButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "end", ">>", true
                    );

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
            }
        }

        return messageBuilder;
    }

    /// <summary>
    /// Builds a paginated message displaying the current music queue when triggered
    /// from a modal submission. The queue is divided into pages of 15 tracks and
    /// allows users to navigate between pages while also providing controls to
    /// skip to or remove tracks.
    /// </summary>
    /// <param name="modalEventArgs">
    /// The <see cref="ModalSubmitEventArgs"/> containing the modal interaction data,
    /// including the user and guild where the queue is being viewed.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance that contains the current
    /// playback queue and player state.
    /// </param>
    /// <param name="pageNumber">
    /// The page number of the queue to display. Each page contains up to 15 tracks.
    /// Defaults to page 1 if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the queue embed along with
    /// interactive components such as pagination buttons and queue management menus.
    /// </returns>
    public DiscordMessageBuilder ViewQueue(ModalSubmitEventArgs modalEventArgs,
        QueuedLavalinkPlayer queuedLavalinkPlayer, string pageNumber = "1")
    {
        var messageBuilder = new DiscordMessageBuilder();

        var embed = new DiscordEmbedBuilder
        {
            Title = "☰  Queue List:",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = modalEventArgs.Interaction.Guild.IconUrl
            }
        };

        if (queuedLavalinkPlayer.Queue.IsEmpty)
        {
            embed.Color = DiscordColor.Red;
            embed.Description = "There are no tracks currently in the queue.";
            messageBuilder.AddEmbed(embed);
        }
        else
        {
            embed.Color = DiscordColor.Cyan;

            var i = 1;

            var userId = modalEventArgs.Interaction.User.Id;
            var userData = Bot.UserData[userId];

            switch (pageNumber)
            {
                case "1":
                {
                    userData.CurrentPageNumber = "1";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer));

                    if (queuedLavalinkPlayer.Queue.Count > 15)
                    {
                        var beginningButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "beginning", "<<", true
                        );

                        var backButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "back", "<", true
                        );

                        var pageNumberButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                        );

                        var nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        var endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );

                        var buttons = new List<DiscordComponent>
                        {
                            beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                        foreach (var row in componentsRows) messageBuilder.AddComponents(row);
                    }

                    break;
                }
                case "2":
                {
                    i = 16;

                    userData.CurrentPageNumber = "2";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(15).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "2"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "2"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 31)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "3":
                {
                    i = 31;

                    userData.CurrentPageNumber = "3";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(30).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "3"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "3"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 46)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "4":
                {
                    i = 46;

                    userData.CurrentPageNumber = "4";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(45).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "4"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "4"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 61)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "5":
                {
                    i = 61;

                    userData.CurrentPageNumber = "5";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(60).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "5"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "5"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 76)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "6":
                {
                    i = 76;

                    userData.CurrentPageNumber = "6";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(75).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "6"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "6"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );


                    DiscordButtonComponent nextButton;
                    DiscordButtonComponent endButton;

                    if (queuedLavalinkPlayer.Queue.Count < 91)
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">", true
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>", true
                        );
                    }
                    else
                    {
                        nextButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "next", ">"
                        );

                        endButton = new DiscordButtonComponent
                        (
                            ButtonStyle.Secondary, "end", ">>"
                        );
                    }

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
                case "7":
                {
                    i = 91;

                    userData.CurrentPageNumber = "7";

                    foreach (var queue in queuedLavalinkPlayer.Queue.Skip(90).Take(15))
                    {
                        var title = queue.Track!.Title;
                        var url = queue.Track.Uri;
                        var author = queue.Track.Author;

                        embed.AddField(
                            "\u200B",
                            title.Length > 20
                                ? $"`{i++}.` [{queue.Track!.Title.Substring(0, 20)}...]({url}) - By **{author}**"
                                : $"`{i++}.` [{queue.Track!.Title}]({url}) - By **{author}**", true);
                    }

                    messageBuilder.AddEmbed(embed);

                    var audioPlayerMenu = new AudioPlayerMenu();

                    messageBuilder.AddComponents(audioPlayerMenu.BuildSkipTo(queuedLavalinkPlayer,
                        "7"));
                    messageBuilder.AddComponents(audioPlayerMenu.BuildRemove(queuedLavalinkPlayer, "7"));

                    var beginningButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "beginning", "<<"
                    );

                    var backButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "back", "<"
                    );

                    var pageNumberButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "page-number", $"{pageNumber}"
                    );

                    var nextButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "next", ">", true
                    );

                    var endButton = new DiscordButtonComponent
                    (
                        ButtonStyle.Secondary, "end", ">>", true
                    );

                    var buttons = new List<DiscordComponent>
                    {
                        beginningButton, backButton, pageNumberButton, nextButton, endButton
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

                    foreach (var row in componentsRows) messageBuilder.AddComponents(row);

                    break;
                }
            }
        }

        return messageBuilder;
    }

    /// <summary>
    /// Builds an embed message indicating that the queue was shuffled using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who shuffled the queue.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the shuffle action.
    /// </returns>
    public DiscordEmbedBuilder ShuffleQueue(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⇌  • ``{context.Member.Username}`` shuffled the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the queue was shuffled using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the shuffle.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the shuffle action.
    /// </returns>
    public DiscordEmbedBuilder ShuffleQueue(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⇌  • ``{btnInteractionArgs.User.Username}`` shuffled the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the current track was skipped using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who skipped the track.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the skip action.
    /// </returns>
    public DiscordEmbedBuilder Skip(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭  • ``{context.Member.Username}`` skipped to the next track.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the current track was skipped using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who triggered the skip.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the skip action.
    /// </returns>
    public DiscordEmbedBuilder Skip(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭  • ``{btnInteractionArgs.User.Username}`` skipped to the next track.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user skipped directly to a specific track in the queue.
    /// </summary>
    /// <param name="menuInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who selected the track.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance containing the current queue and track information.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the skip-to-track action.
    /// </returns>
    public DiscordEmbedBuilder SkipTo(ComponentInteractionCreateEventArgs menuInteractionArgs,
        QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var track = queuedLavalinkPlayer.CurrentItem;

        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"⏭  • ``{menuInteractionArgs.User.Username}`` skipped to ``{track!.Track!.Title}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user changed the playback position of the current track.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who changed the track position.
    /// </param>
    /// <param name="seekedPosition">
    /// The new playback position in seconds where the track was moved.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the seek action.
    /// </returns>
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

    /// <summary>
    /// Builds an embed message displaying the current playback position of the track.
    /// </summary>
    /// <param name="position">
    /// The current playback position of the track.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the current track position.
    /// </returns>
    public DiscordEmbedBuilder TrackPosition(TimeSpan position)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🕒  • Current Track Position: ``{RoundSeconds(position)}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Rounds a <see cref="TimeSpan"/> value to the nearest second.
    /// </summary>
    /// <param name="timespan">
    /// The time value to round.
    /// </param>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing the rounded time value.
    /// </returns>
    private TimeSpan RoundSeconds(TimeSpan timespan)
    {
        return TimeSpan.FromSeconds(Math.Round(timespan.TotalSeconds));
    }

    /// <summary>
    /// Builds an embed message indicating the current repeat mode after it was changed using a slash command.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the user who changed the repeat mode.
    /// </param>
    /// <param name="queuedLavalinkPlayer">
    /// The <see cref="QueuedLavalinkPlayer"/> instance that contains the current repeat mode state.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> describing the updated repeat mode.
    /// </returns>
    public DiscordEmbedBuilder Repeat(InteractionContext context, QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        DiscordEmbedBuilder embed;

        if (queuedLavalinkPlayer.RepeatMode == TrackRepeatMode.Track)
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" ⇄  • ``{context.Member.Username}`` enabled repeat track mode.",
                Color = DiscordColor.Cyan
            };
        else if (queuedLavalinkPlayer.RepeatMode == TrackRepeatMode.Queue)
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" ⇄  • ``{context.Member.Username}`` enabled repeat queue mode.",
                Color = DiscordColor.Cyan
            };
        else
            embed = new DiscordEmbedBuilder
            {
                Description =
                    $" ⇄  • ``{context.Member.Username}`` disabled repeat mode.",
                Color = DiscordColor.Cyan
            };

        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that repeat track mode was enabled using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who enabled repeat track mode.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the repeat track mode action.
    /// </returns>
    public DiscordEmbedBuilder EnableTrackRepeat(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $" ⇄  • ``{btnInteractionArgs.User.Username}`` enabled repeat track mode.",
            Color = DiscordColor.Cyan
        };

        return embed;
    }
    /// <summary>
    /// Builds an embed message indicating that repeat queue mode was enabled using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who enabled repeat queue mode.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the repeat queue mode action.
    /// </returns>
    public DiscordEmbedBuilder EnableQueueRepeat(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $" ⇄  • ``{btnInteractionArgs.User.Username}`` enabled repeat queue mode.",
            Color = DiscordColor.Cyan
        };

        return embed;
    }
    
    /// <summary>
    /// Builds an embed message indicating that repeat mode was disabled using a button interaction.
    /// </summary>
    /// <param name="btnInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who disabled repeat mode.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the repeat disable action.
    /// </returns>
    public DiscordEmbedBuilder DisableRepeat(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $" ⇄  • ``{btnInteractionArgs.User.Username}`` disabled repeat mode.",
            Color = DiscordColor.Cyan
        };

        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a user applied or changed an audio filter.
    /// </summary>
    /// <param name="menuInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who selected the filter.
    /// </param>
    /// <param name="filter">
    /// The name of the filter that was applied.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the filter change action.
    /// </returns>
    public DiscordEmbedBuilder BuildFilter(ComponentInteractionCreateEventArgs menuInteractionArgs, string filter)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"၊||၊  • ``{menuInteractionArgs.User.Username}`` changed the filter to ``{filter}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a track was removed from the queue.
    /// </summary>
    /// <param name="menuInteractionArgs">
    /// The <see cref="ComponentInteractionCreateEventArgs"/> containing the user who removed the track.
    /// </param>
    /// <param name="removedTrack">
    /// The <see cref="LavalinkTrack"/> that was removed from the queue.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the track removal action.
    /// </returns>
    public DiscordEmbedBuilder Remove(ComponentInteractionCreateEventArgs menuInteractionArgs,
        LavalinkTrack removedTrack)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🗑️  • ``{menuInteractionArgs.User.Username}`` removed ``{removedTrack.Title}`` from the queue.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }
}