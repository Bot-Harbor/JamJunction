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
            Color = DiscordColor.Cyan,
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
                       "Version: 2.7.4  •  " +
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}  •  " +
                       $"Ping: {ping}" + 
                       "\n\nMade With ❤️",
            }
        };
        
        var helpMenu = new HelpMenu();
        
        var addBotBtn = new DiscordLinkButtonComponent
        (
            "https://discord.com/oauth2/authorize?client_id=1181700334561796227\n",
            "🤖 Add To A Server"
        );

        var viewTopggBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227",
            "🎩 View On Top.gg"
        );

        var leaveReviewBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227#reviews",
            "⭐ Leave A Review"
        );
        
        var buttons = new List<DiscordComponent>
        {
            addBotBtn, viewTopggBtn, leaveReviewBtn
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
            Color = DiscordColor.Cyan,
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
                       "Version: 2.7.4  •  " +
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}  •  " +
                       $"Ping: {ping}" + 
                       "\n\nMade With ❤️",
            }
        };
        
        var helpMenu = new HelpMenu();
        
        var addBotBtn = new DiscordLinkButtonComponent
        (
            "https://discord.com/oauth2/authorize?client_id=1181700334561796227\n",
            "🤖 Add To A Server"
        );

        var viewTopggBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227",
            "🎩 View On Top.gg"
        );

        var leaveReviewBtn = new DiscordLinkButtonComponent
        (
            "https://top.gg/bot/1181700334561796227#reviews",
            "⭐ Leave A Review"
        );
        
        var buttons = new List<DiscordComponent>
        {
            addBotBtn, viewTopggBtn, leaveReviewBtn
        };
        

        var followUpMessage = new DiscordFollowupMessageBuilder();

        followUpMessage.IsEphemeral = true;

        followUpMessage.AddEmbed(helpEmbed);
        
        followUpMessage.AddComponents(helpMenu.Build());
        followUpMessage.AddComponents(buttons);

        return followUpMessage;
    }
}