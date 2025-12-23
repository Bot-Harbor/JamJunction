using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Menus;
using Lavalink4NET.Integrations.Lavasrc;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using SpotifyAPI.Web;
using YoutubeExplode.Playlists;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Views.Embeds;

public class AudioPlayerEmbed
{
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
            Description = $"💿  •  **Title**: [{track.Title}]({uri})\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"{slider}",
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
                IconUrl = "https://cdn-icons-png.flaticon.com/512/145/145809.png"
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
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Next Track: {nextTrack.Track!.Title}"
                };

        var pauseButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "pause", "⏸", pauseDisabled
        );

        var resumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "resume", "▶", resumeDisabled
        );

        var skipButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "skip", "⏭"
        );

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

    public DiscordMessageBuilder TrackInformation(ExtendedLavalinkTrack track,
        QueuedLavalinkPlayer queuedLavalinkPlayer)
    {
        var uri = track.Uri!.AbsoluteUri;

        var slider = GenerateSlider(queuedLavalinkPlayer.Position!.Value.Position, track.Duration);

        var embed = new DiscordEmbedBuilder
        {
            Description = $"💿  •  **Title**: [{track.Title}]({uri})\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"{slider}",
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
                IconUrl = "https://cdn-icons-png.flaticon.com/512/145/145809.png"
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
                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Next Track: {nextTrack.Track!.Title}"
                };

        var pauseButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "pause", "⏸"
        );

        var resumeButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "resume", "▶", true
        );

        var skipButton = new DiscordButtonComponent
        (
            ButtonStyle.Secondary, "skip", "⏭"
        );

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

    private string FormatTime(TimeSpan time)
    {
        return time.Hours > 0
            ? time.ToString(@"hh\:mm\:ss")
            : time.ToString(@"mm\:ss");
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

    public DiscordEmbedBuilder PlaylistAddedToQueue(Playlist playlist)
    {
        var channelTitle = playlist.Author!.ChannelTitle;

        var embed = new DiscordEmbedBuilder
        {
            Title = "Added To The Queue 🎵",
            Description = $"ılı   •  [{playlist.Title}]({playlist.Url}) - By **{channelTitle}**",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

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
            Description = $"⏹   • ``{context.Member.Username}`` stopped the player.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder Stop(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏹   • ``{btnInteractionArgs.User.Username}`` stopped the player.",
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
            Description = $"🔉  •  ``{btnInteractionArgs.User.Username}`` decreased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

    public DiscordEmbedBuilder VolumeIncreased(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  ``{btnInteractionArgs.User.Username}`` increased the volume.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

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

    private TimeSpan RoundSeconds(TimeSpan timespan)
    {
        return TimeSpan.FromSeconds(Math.Round(timespan.TotalSeconds));
    }

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

    public DiscordEmbedBuilder EnableRepeat(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $" ⇄  • ``{btnInteractionArgs.User.Username}`` enabled repeat track mode.",
            Color = DiscordColor.Cyan
        };

        return embed;
    }

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

    public DiscordEmbedBuilder BuildFilter(ComponentInteractionCreateEventArgs menuInteractionArgs, string filter)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🗑️  • ``{menuInteractionArgs.User.Username}`` changed the filter to ``{filter}``.",
            Color = DiscordColor.Cyan
        };
        return embed;
    }

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