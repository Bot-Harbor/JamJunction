using System.Collections.ObjectModel;
using System.Data;
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

        var helpEmbed = new DiscordEmbedBuilder
        {
            Author = new DiscordEmbedBuilder.EmbedAuthor()
            {
                Name = $"{userName}",
                Url = "https://github.com/Bot-Harbor/JamJunction",
                IconUrl = userIcon
            },
            Title = $"📝 Getting Started",
            Color = DiscordColor.White,
            Description =
                "Your go-to music bot for you and your friends! Type one of the commands below to get started. " +
                $"Jam Junction powered by [DSharpPlus 4.4.5]" +
                $"(https://dsharpplus.github.io/DSharpPlus/index.html), " +
                $"[Lavalink 4.4.5](https://dsharpplus.github.io/DSharpPlus/articles/audio/lavalink/setup.html), " +
                $"and [Docker](https://www.docker.com/).",

            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = botIcon
            },
            Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = ($"*Bot Info  •  " +
                        $"Total Servers: {serverCount}  •  " +
                        $"Shard: {shardCount}  •  " +
                        $"Ping: {ping}  •  " +
                        $"Version: 1.0.0")
            },
        };

        helpEmbed.AddField
        (
            "🎶  **Music Commands**",
            $"▶️  </play:1181715791658360852>\n" +
            $"⏸️  </pause:1185357127468986450>\n" +
            $"▶️  </resume:1185412430055084052>\n" +
            $"🔴  </stop:1185428654155636738>\n" +
            $"🔀  </shuffle:1200625616244981821>\n" +
            $"🔊  </volume:1185357127468986451>\n" +
            $"🔇  </mute:1196919352222564453>\n" +
            $"🔊  </unmute:1196933203806662706>\n" +
            $"🎶  </viewqueue:1200604461941391370>\n" +
            $"🔁  </restart:1186037012642418698>\n" +
            $"⌛  </seek:1186000603273510952>\n" +
            $"🎵  </currentsong:1201998662625153065>\n" +
            $"⏭️  </skip:1204215826773835778>\n" +
            $"🔌  </leave:1192206662468108438>\n",
            inline: true
        );

        helpEmbed.AddField
        (
            "🛠️  **Other Commands**",
            $"🆘  </help:1204525562954121257>\n" +
            $"🏓  </ping:1181709713256239204>\n" +
            $"🖼️  </caption:1182083902752444498>\n",
            inline: true
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