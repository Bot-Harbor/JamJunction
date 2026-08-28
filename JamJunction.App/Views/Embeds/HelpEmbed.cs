using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Menus;

namespace JamJunction.App.Views.Embeds;

/// <summary>
/// Represents the help menu embed used to display available commands,
/// bot information, and useful links for Jam Junction.
/// </summary>
/// <remarks>
/// This embed provides users with an overview of music commands,
/// additional bot commands, and links for adding the bot,
/// viewing it on Top.gg, or leaving a review.
/// </remarks>
public class HelpEmbed
{
    /// <summary>
    /// Builds the help message containing bot information, available commands,
    /// and useful links such as adding the bot to a server or viewing it on Top.gg.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> that triggered the help command.
    /// This provides access to the requesting user, guild information,
    /// and the bot client state.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the help embed
    /// and interactive link buttons for adding the bot, viewing it on Top.gg,
    /// and leaving a review.
    /// </returns>
    public DiscordFollowupMessageBuilder Build(InteractionContext context)
    {
        var botIcon = context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png);
        var serverCount = context.Client.Guilds.Count;
        var shardCount = context.Client.ShardCount;
        var ping = context.Client.Ping;
        var dSharpPlusVersion = context.Client.VersionString.Substring(0, 5);

        var helpEmbed = new DiscordEmbedBuilder
        {
            Title = "🛟 Help Menu",
            Color = DiscordColor.Purple,
            Description =
                "Welcome to the help menu! This is your go-to place for " +
                "everything you need to start using the bot. Browse through " +
                "the menu to explore the different sections and features available, or " +
                "type </play:1181715791658360852> to get started! " +
                $"\n\nJam Junction is powered by [DSharpPlus {dSharpPlusVersion}]" +
                "(https://github.com/DSharpPlus/DSharpPlus), " +
                "[Lavalink4NET 4.2.0](https://github.com/angelobreuer/Lavalink4NET), " +
                "and [Docker](https://www.docker.com/).",

            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = botIcon
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Bot Info:   " +
                       "Version: 2.8.5  •  " +
                       $"Ping: {ping}  •  " + 
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}" +
                       "\n\nMade With ❤️",
            }
        };
        
        var helpMenu = new HelpMenu();
        
        var joinServerBtn = new DiscordLinkButtonComponent
        (
            "https://discord.gg/z5J6FwkuJ",
            "🎧 Join Our Server"
        );

        var leaveReviewBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227#reviews",
            "⭐ Leave A Review"
        );
        
        var buttons = new List<DiscordComponent>
        {
            joinServerBtn, leaveReviewBtn
        };
        

        var followUpMessage = new DiscordFollowupMessageBuilder();

        followUpMessage.IsEphemeral = true;

        followUpMessage.AddEmbed(helpEmbed);
        
        followUpMessage.AddComponents(helpMenu.Build());
        followUpMessage.AddComponents(buttons);

        return followUpMessage;
    }
    
    
    /// <summary>
    /// Builds the help message containing bot information, available commands,
    /// and useful links such as adding the bot to a server or viewing it on Top.gg.
    /// </summary>
    /// <param name="client">
    /// The <see cref="DiscordClient"/> providing bot info, ping, and version data.
    /// </param>
    /// <returns>
    /// A <see cref="DiscordMessageBuilder"/> containing the help embed
    /// and interactive link buttons for adding the bot, viewing it on Top.gg,
    /// and leaving a review.
    /// </returns>
    public DiscordInteractionResponseBuilder BuildSection(DiscordEmbedBuilder embed)
    {
        var helpMenu = new HelpMenu();

        var joinServerBtn = new DiscordLinkButtonComponent(
            "https://discord.gg/z5J6FwkuJ",
            "🎧 Join Our Server"
        );
        var leaveReviewBtn = new DiscordLinkButtonComponent(
            "https://top.gg/bot/1181700334561796227#reviews",
            "⭐ Leave A Review"
        );

        return new DiscordInteractionResponseBuilder()
            .AddEmbed(embed)
            .AddComponents(helpMenu.Build())
            .AddComponents(new List<DiscordComponent> { joinServerBtn, leaveReviewBtn });
    }
    
    public DiscordFollowupMessageBuilder Build(DiscordClient client)
    {
        var botIcon = client.CurrentUser.GetAvatarUrl(ImageFormat.Png);
        var serverCount = client.Guilds.Count;
        var shardCount = client.ShardCount;
        var ping = client.Ping;
        var dSharpPlusVersion = client.VersionString.Substring(0, 5);
 
        var helpEmbed = new DiscordEmbedBuilder
        {
            Title = "🛟 Help Menu",
            Color = DiscordColor.Purple,
            Description =
                "Welcome to the help menu! This is your go-to place for " +
                "everything you need to start using the bot. Browse through " +
                "the menu to explore the different sections and features available, or " +
                "type </play:1181715791658360852> to get started! " +
                $"\n\nJam Junction is powered by [DSharpPlus {dSharpPlusVersion}]" +
                "(https://github.com/DSharpPlus/DSharpPlus), " +
                "[Lavalink4NET 4.2.0](https://github.com/angelobreuer/Lavalink4NET), " +
                "and [Docker](https://www.docker.com/).",

            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = botIcon
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Bot Info:   " +
                       "Version: 2.8.4  •  " +
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}  •  " +
                       $"Ping: {ping}" + 
                       "\n\nMade With ❤️",
            }
        };
        
        var helpMenu = new HelpMenu();
        
        var joinServerBtn = new DiscordLinkButtonComponent
        (
            "https://discord.gg/z5J6FwkuJ",
            "🎧 Join Our Server"
        );
        
        var leaveReviewBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227#reviews",
            "⭐ Leave A Review"
        );
        
        var buttons = new List<DiscordComponent>
        {
            joinServerBtn, leaveReviewBtn
        };
        

        var followUpMessage = new DiscordFollowupMessageBuilder();

        followUpMessage.IsEphemeral = true;

        followUpMessage.AddEmbed(helpEmbed);
        
        followUpMessage.AddComponents(helpMenu.Build());
        followUpMessage.AddComponents(buttons);

        return followUpMessage;
    }
    
     public DiscordEmbedBuilder BuildMain(DiscordClient client)
    {
        var botIcon = client.CurrentUser.GetAvatarUrl(ImageFormat.Png);
        var serverCount = client.Guilds.Count;
        var shardCount = client.ShardCount;
        var ping = client.Ping;
        var dSharpPlusVersion = client.VersionString.Substring(0, 5);

        return new DiscordEmbedBuilder
        {
            Title = "🛟 Help Menu",
            Color = DiscordColor.Purple,
            Description =
                "Welcome to the help menu! This is your go-to place for " +
                "everything you need to start using the bot. Browse through " +
                "the menu to explore the different sections and features available, or " +
                "type </play:1181715791658360852> to get started! " +
                $"\n\nJam Junction is powered by [DSharpPlus {dSharpPlusVersion}]" +
                "(https://github.com/DSharpPlus/DSharpPlus), " +
                "[Lavalink4NET 4.2.0](https://github.com/angelobreuer/Lavalink4NET), " +
                "and [Docker](https://www.docker.com/).",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = botIcon },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "Bot Info:   " +
                       "Version: 2.8.4  •  " +
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}  •  " +
                       $"Ping: {ping}" +
                       "\n\nMade With ❤️",
            }
        };
    }

    public DiscordEmbedBuilder BuildAllFeatures()
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "🌐 All Features",
            Color = DiscordColor.Purple,
            Description = "Here is everything Jam Junction has to offer."
        };

        embed.AddField(
            "🎵 Playback",
            "• 6 supported platforms: Spotify, YouTube, YouTube Music, Deezer, SoundCloud, Apple Music\n" +
            "• Search by keyword or paste a URL\n" +
            "• Queue next to insert a track at the front\n" +
            "• Step back to the previously played track\n" +
            "• Synced lyrics for the current track\n" +
            "• Seek to any position using hours, minutes, and seconds"
        );

        embed.AddField(
            "📋 Queue",
            "• Up to 100 tracks per queue\n" +
            "• Paginated queue view with navigation controls\n" +
            "• Skip to any track directly from the menu\n" +
            "• Remove any track from the queue\n" +
            "• Shuffle the remaining queue at any time"
        );

        embed.AddField(
            "🔁 Repeat Modes",
            "• None — playback stops when the queue ends\n" +
            "• Repeat Track — loops the current track\n" +
            "• Repeat Queue — loops the entire queue"
        );

        embed.AddField(
            "🎛️ Audio Filters",
            "• 🌙 Nightcore  • 🎧 8D  • 🌊 Vaporwave\n" +
            "• 🎤 Karaoke  • 🕒 Slow Motion  • 🔄 Reset"
        );

        embed.AddField(
            "❤️ Personal Playlist",
            "• Like any track to save it directly to your DMs\n" +
            "• Access your playlist anytime via the 📋 button or </personal-playlist:1496305132093050952>"
        );

        embed.AddField(
            "🤖 AI Recommendations",
            "• When a track ends, get an AI-powered suggestion for what to play next\n" +
            "• Suggestions match the mood and come from the same platform you were listening on\n" +
            "• Tap ➕ Add to Queue to drop the suggested track straight into the queue"
        );

        embed.AddField(
            "🔊 Player Controls",
            "• Pause, resume, skip, previous, stop, restart\n" +
            "• Volume up/down in 10% increments\n" +
            "• Seek autofill, help, and playlist buttons built into the player"
        );

        embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text = "\n\nMade With ❤️"
        };

        return embed;
    }

    public DiscordEmbedBuilder BuildMusicCommands()
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "🎵 Music Commands",
            Color = DiscordColor.Purple,
            Description = "All music-related slash commands available in Jam Junction."
        };

        embed.AddField(
            "Playback",
            "</play:1181715791658360852> — Queue a track.\n" +
            "</pause:1185357127468986450> — Pauses the current track.\n" +
            "</resume:1185412430055084052> — Resumes the current track.\n" +
            "</stop:1185428654155636738> — Stops the playback.\n" +
            "</restart:1186037012642418698> — Restarts the current track.\n" +
            "</previous-track:1496305132093050951> — Returns to the previously played track."
        );

        embed.AddField(
            "Queue",
            "</skip:1204215826773835778> — Skips to the next track in the queue.\n" +
            "</shuffle:1200625616244981821> — Shuffles the queue.\n" +
            "</view-queue:1292956075032576070> — Displays what is currently in the queue.\n" +
            "</current-track:1300139412553859085> — Shows details about the current track playing.\n" +
            "</lyrics:1527411578109169766> — Shows synced lyrics for the current track playing."
        );

        embed.AddField(
            "Positioning & Volume",
            "</seek:1186000603273510952> — Sets the position of the track.\n" +
            "</position:1215802163658358795> — Gets the current track position.\n" +
            "</volume:1185357127468986451> — Adjust the volume 0-100."
        );

        embed.AddField(
            "Other",
            "</repeating-mode:1319060173561659555> — Change the repeating mode.\n" +
            "</leave:1192206662468108438> — Disconnects the player."
        );
        
        embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text = "\n\nMade With ❤️"
        };

        return embed;
    }

    public DiscordEmbedBuilder BuildOtherCommands()
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "🛠️ Other Commands",
            Color = DiscordColor.Purple,
            Description = "General-purpose commands outside of music playback."
        };

        embed.AddField(
            "Utility",
            "</help:1204525562954121257> — Gives information about the bot & available commands.\n" +
            "</personal-playlist:1496305132093050952> — Opens your personal playlist in the bot's DMs.\n" +
            "</ping:1181709713256239204> — Will pong back to the server.\n" +
            "</caption:1182083902752444498> — Give any image a caption."
        );
        
        embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text = "\n\nMade With ❤️"
        };

        return embed;
    }

    public DiscordEmbedBuilder BuildPlayerControls()
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "📻 Player Controls",
            Color = DiscordColor.Purple,
            Description = "Every button on the audio player and what it does."
        };

        embed.AddField(
            "Row 1 — Playback",
            "⏸ Pause  •  ⏮ Previous Track  •  ▶ Resume  •  ⏭ Skip  •  ⏹ Stop"
        );

        embed.AddField(
            "Row 2 — Controls",
            "☰ View Queue  •  🔉 Volume Down  •  🔊 Volume Up  •  ↻ Restart  •  ⇄ Repeat"
        );

        embed.AddField(
            "Row 3 — Extras",
            "⇌ Shuffle  •  ❤️ Like (saves track to DMs)  •  🛟 Help  •  📋 Playlist  •  🔍 Seek"
        );

        embed.AddField(
            "Notes",
            "• ⏭ and ☰ and ⇌ are disabled when the queue is empty\n" +
            "• 🔉 is disabled at minimum volume, 🔊 at maximum\n" +
            "• ⏮ is disabled when there is no playback history"
        );
        
        embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text = "\n\nMade With ❤️"
        };

        return embed;
    }
}