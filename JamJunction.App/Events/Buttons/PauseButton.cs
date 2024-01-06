using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;

namespace JamJunction.App.Events.Buttons;

public class PauseButton : IButton
{
    public static bool PauseCommandInvoked { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        if (e.Interaction.Data.CustomId == "pause")
        {
            var userVc = e.Channel;
            var lava = sender.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (e.Channel != null && (e.Channel.Guild.Permissions & Permissions.ManageChannels) != 0)
            {
                if (!lava.ConnectedNodes.Any())
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(
                            errorEmbed.NoConnectionErrorEmbedBuilder()));
                }

                await node.ConnectAsync(userVc);

                var connection = node.GetGuildConnection(e.Guild);

                if (connection != null)
                {
                    await connection.PauseAsync();
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(audioEmbed.PauseButtonEmbedBuilder(e)));
                }
            }
        }
    }
}