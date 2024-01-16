using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events.Buttons;

public class MuteButton : IButton
{
    public static bool MuteButtonInvoked { get; set; } = false;

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "mute")
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);
                var userVc = member?.VoiceState?.Channel;
                var lava = sender.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();

                if (member != null && (e.Channel.PermissionsFor(member) & Permissions.ManageChannels) != 0)
                {
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

                    if (connection != null && connection.CurrentState.CurrentTrack == null)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoAudioTrackErrorEmbedBuilder()));
                    }


                    if (connection != null)
                    {
                        if (!MuteButtonInvoked)
                        {
                            await connection.SetVolumeAsync(0);

                            MuteButtonInvoked = true;

                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    audioEmbed.MuteEmbedBuilder(e))); // Add custom mute embed
                        }
                        else
                        {
                            if (!VolumeCommand.VolumeCommandInvoked)
                            {
                                await connection.SetVolumeAsync(PlayCommand.DefaultVolume);

                                MuteButtonInvoked = false;

                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        audioEmbed.UnmuteEmbedBuilder(e))); // Add custom unmute embed
                            }
                            else
                            {
                                await connection.SetVolumeAsync(VolumeCommand.Volume);

                                MuteButtonInvoked = false;

                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        audioEmbed.UnmuteEmbedBuilder(e))); // Add custom unmute embed
                            }
                        }
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed
                            .NoVolumePermissionEmbedBuilder())); //Change permission for mute permission
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