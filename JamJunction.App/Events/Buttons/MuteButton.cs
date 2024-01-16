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
    public static bool MuteButtonInvoked { get; set; }

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
                            if (PauseCommand.PauseCommandInvoked || PauseButton.PauseCommandInvoked)
                            {
                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        errorEmbed.NoMuteWhilePausedEmbedBuilder(e)));
                            }
                            else
                            {
                                await connection.SetVolumeAsync(0);

                                
                                MuteCommand.MuteCommandInvoked = true;
                                MuteButtonInvoked = true;

                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        audioEmbed.MuteEmbedBuilder(e)));
                            }
                        }
                        else
                        {
                            if (!VolumeCommand.VolumeCommandInvoked)
                            {
                                if (PauseCommand.PauseCommandInvoked || PauseButton.PauseCommandInvoked)
                                {
                                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                        new DiscordInteractionResponseBuilder().AddEmbed(
                                            errorEmbed.NoUnMuteWhilePausedEmbedBuilder(e)));
                                }
                                else
                                {
                                    await connection.SetVolumeAsync(PlayCommand.DefaultVolume);

                                    MuteCommand.MuteCommandInvoked = false;
                                    MuteButtonInvoked = false;

                                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                        new DiscordInteractionResponseBuilder().AddEmbed(
                                            audioEmbed.UnmuteEmbedBuilder(e)));
                                }
                            }
                            else
                            {
                                if (PauseCommand.PauseCommandInvoked || PauseButton.PauseCommandInvoked)
                                {
                                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                        new DiscordInteractionResponseBuilder().AddEmbed(
                                            errorEmbed.NoUnMuteWhilePausedEmbedBuilder(e)));
                                }
                                else
                                {
                                    await connection.SetVolumeAsync(VolumeCommand.Volume);

                                    MuteCommand.MuteCommandInvoked = false;
                                    MuteButtonInvoked = false;

                                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                        new DiscordInteractionResponseBuilder().AddEmbed(
                                            audioEmbed.UnmuteEmbedBuilder(e)));
                                }
                            }
                        }
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed
                            .NoVolumePermissionEmbedBuilder()));
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