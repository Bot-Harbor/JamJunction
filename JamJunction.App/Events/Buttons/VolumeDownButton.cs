using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;

namespace JamJunction.App.Events.Buttons;

public class VolumeDownButton : IButton
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "volume down")
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);
                var userVc = member?.VoiceState?.Channel;
                var lava = sender.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();

                if (member != null && (e.Channel.PermissionsFor(member) & Permissions.ManageChannels) != 0)
                {
                    if (!lava.ConnectedNodes.Any())
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

                    await node.ConnectAsync(userVc);

                    var connection = node.GetGuildConnection(e.Guild);

                    if (connection == null)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.LavaLinkErrorEmbedBuilder()));
                    }

                    if (connection != null && connection.CurrentState.CurrentTrack == null)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoAudioTrackErrorEmbedBuilder()));
                    }

                    int volume = 100;
                    
                    if (volume > 100)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.MaxVolumeEmbedBuilder(e)));
                    }
                    else if (volume < 0)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.MinVolumeEmbedBuilder(e)));
                    }
                    else
                    {
                        if (connection != null)
                        {
                            await connection.SetVolumeAsync(volume - 10);

                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    audioEmbed.VolumeDecreaseEmbedBuilder(e)));
                        }
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.NoVolumePermissionEmbedBuilder()));
                }
            }
        }
        catch (Exception exception)
        {
            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.CommandFailedEmbedBuilder()));
        }
    }
}