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

                    if (connection != null)
                    {
                        var guildId = e.Guild.Id;
                        var audioPlayerController = Bot.GuildAudioPlayers[guildId];

                        if (audioPlayerController.PauseInvoked)
                        {
                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    errorEmbed.NoVolumeWhilePausedEmbedBuilder(e)));
                        }
                        else
                        {
                            if (audioPlayerController.Volume == 100)
                            {
                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        errorEmbed.MaxVolumeEmbedBuilder(e)));
                            }
                            else
                            {
                                var userVolume = audioPlayerController.Volume;
                                var adjustedVolume = userVolume + 10;

                                adjustedVolume = Math.Min(adjustedVolume, 100);

                                await connection.SetVolumeAsync(adjustedVolume);
                                audioPlayerController.Volume = adjustedVolume;

                                audioPlayerController.MuteInvoked = false;
                                
                                await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                    new DiscordInteractionResponseBuilder().AddEmbed(
                                        audioEmbed.VolumeIncreaseEmbedBuilder(e)));   
                            }
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