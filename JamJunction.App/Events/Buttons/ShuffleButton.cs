using System.Collections;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events.Buttons;

public class ShuffleButton : ShuffleQueueCommand, IButton
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "shuffle")
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);

                if (member != null && (e.Channel.PermissionsFor(member) & Permissions.ManageChannels) != 0)
                {
                    if (PlayCommand.Queue.Count != 0)
                    {
                        ShuffleQueue(PlayCommand.Queue);

                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(audioEmbed.ShuffleQueueBuilder(e)));
                    }
                    else
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.QueueIsEmptyEmbedBuilder(e)));
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.NoShufflePermissionEmbedBuilder()));
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