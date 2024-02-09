using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events.Buttons;

public class VolumeUpButton : IButton
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "volumeup")
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

                    if (!VolumeCommand.VolumeCommandInvoked)
                    {
                        if (connection != null)
                        {
                            if (PlayCommand.DefaultVolume == 100)
                            {
                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        errorEmbed.MaxVolumeEmbedBuilder(e)));
                            }

                            var defaultVolume = PlayCommand.DefaultVolume;
                            var adjustedDefaultVolume = defaultVolume + 10;

                            adjustedDefaultVolume = Math.Min(adjustedDefaultVolume, 100);

                            await connection.SetVolumeAsync(adjustedDefaultVolume);
                            PlayCommand.DefaultVolume = adjustedDefaultVolume;
                            
                            MuteButton.MuteButtonInvoked = false;

                            // Remove test case
                            Console.WriteLine(adjustedDefaultVolume);

                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    audioEmbed.VolumeIncreaseEmbedBuilder(e)));
                        }
                    }
                    else
                    {
                        if (connection != null)
                        {
                            if (VolumeCommand.Volume == 100)
                            {
                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        errorEmbed.MaxVolumeEmbedBuilder(e)));
                            }
                            
                            var userVolume = VolumeCommand.Volume;
                            var adjustedVolume = userVolume + 10;
                            
                            adjustedVolume = Math.Min(adjustedVolume, 100);

                            await connection.SetVolumeAsync(adjustedVolume);
                            VolumeCommand.Volume = adjustedVolume;
                            
                            MuteButton.MuteButtonInvoked = false;
                            
                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    audioEmbed.VolumeIncreaseEmbedBuilder(e)));
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
                new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(errorEmbed.CommandFailedEmbedBuilder()));
        }
    }
}