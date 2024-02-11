using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embed_Builders;

public class HelpEmbed
{
    public DiscordMessageBuilder HelpEmbedBuilder(InteractionContext context)
    {
        var userIcon = context.User.GetAvatarUrl(ImageFormat.Png);
        var userName = context.Client.CurrentUser.Username;
        var botIcon = context.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png);
        var serverCount = context.Client.Guilds.Count;
        var shardCount = context.Client.ShardCount;
        var ping = context.Client.Ping;
        var botVersion = context.Client.VersionString.Substring(0, 5);

        var helpEmbed = new DiscordEmbedBuilder
        {
            Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = $"{userName}",
                Url = "https://github.com/Bot-Harbor/JamJunction",
                IconUrl = userIcon
            },
            Title = "\ud83d\udcdd Getting Started",
            Color = DiscordColor.White,
            Description =
                "Your go-to music bot for you and your friends! Type one of the commands below to get started. " +
                "Most of the music commands will require you to have ``Manage Channels Permission``. " +
                $"Jam Junction powered by [DSharpPlus {botVersion}]" +
                "(https://dsharpplus.github.io/DSharpPlus/index.html), " +
                $"[Lavalink {botVersion}](https://github.com/lavalink-devs/Lavalink), " +
                "and [Docker](https://www.docker.com/).",

            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = botIcon
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = "*Bot Info  •  " +
                       $"Total Servers: {serverCount}  •  " +
                       $"Shard: {shardCount}  •  " +
                       $"Ping: {ping}  •  " +
                       $"Version: {botVersion}"
            }
        };

        helpEmbed.AddField
        (
            "🎶  **Music Commands**",
            "\u25b6\ufe0f  </play:1181715791658360852>\n" +
            "\u23f8\ufe0f  </pause:1185357127468986450>\n" +
            "\u25b6\ufe0f  </resume:1185412430055084052>\n" +
            "\ud83d\udd34  </stop:1185428654155636738>\n" +
            "\ud83d\udd00  </shuffle:1200625616244981821>\n" +
            "\ud83d\udd0a  </volume:1185357127468986451>\n" +
            "\ud83d\udd07  </mute:1196919352222564453>\n" +
            "\ud83d\udd0a  </unmute:1196933203806662706>\n" +
            "\ud83c\udfb6  </viewqueue:1200604461941391370>\n" +
            "\ud83d\udd01  </restart:1186037012642418698>\n" +
            "\u231b  </seek:1186000603273510952>\n" +
            "\ud83c\udfb5  </currentsong:1201998662625153065>\n" +
            "\u23ed\ufe0f  </skip:1204215826773835778>\n" +
            "\ud83d\udd0c  </leave:1192206662468108438>\n",
            true
        );

        helpEmbed.AddField
        (
            "🛠️  **Other Commands**",
            "\ud83c\udd98  </help:1204525562954121257>\n" +
            "\ud83c\udfd3  </ping:1181709713256239204>\n" +
            "\ud83d\uddbc\ufe0f  </caption:1182083902752444498>\n",
            true
        );

        var addBotBtn = new DiscordLinkButtonComponent
        (
            "https://discord.com/api/oauth2/authorize?client_id=11817003345617" +
            "96227&permissions=8&scope=bot+applications.commands",
            "Add To A Server"
        );

        var viewRepoBtn = new DiscordLinkButtonComponent
        (
            "https://github.com/Bot-Harbor/JamJunction",
            "View Repository"
        );

        var messageBuilder =
            new DiscordMessageBuilder(new DiscordMessageBuilder().AddEmbed(helpEmbed)
                .AddComponents(addBotBtn, viewRepoBtn));

        return messageBuilder;
    }
}