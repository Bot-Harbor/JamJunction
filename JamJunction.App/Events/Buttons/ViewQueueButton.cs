using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events.Buttons;

public class ViewQueueButton : IButton
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();

        var message = e.Interaction;

        try
        {
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            var userVc = member?.VoiceState?.Channel;
            var lava = sender.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (!lava.ConnectedNodes!.Any())
            {
                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(
                        errorEmbed.NoConnectionErrorEmbedBuilder()));
            }

            if (userVc == null || userVc.Type != ChannelType.Voice)
            {
                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelBtnErrorEmbedBuilder(e)));
            }

            var connection = node.GetGuildConnection(e.Guild);

            if (connection! == null)
            {
                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.LavaLinkErrorEmbedBuilder()));
            }

            if (connection != null)
            {
                if (e.Interaction.Data.CustomId == "viewqueue")
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(audioEmbed.ViewQueueBuilder(e)));
                }
            }
        }
        catch (Exception exception)
        {
            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(errorEmbed.CommandFailedEmbedBuilder()));
        }
    }
}