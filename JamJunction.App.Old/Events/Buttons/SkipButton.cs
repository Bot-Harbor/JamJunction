﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;

namespace JamJunction.App.Events.Buttons;

public class SkipButton : IButton
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "skip")
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);
                var userVc = member?.VoiceState?.Channel;
                var lava = sender.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();

                if (member != null && (e.Channel.PermissionsFor(member) & Permissions.ManageChannels) != 0)
                {
                    if (!lava.ConnectedNodes!.Any())
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoConnectionErrorEmbedBuilder()));

                    if (userVc == null || userVc.Type != ChannelType.Voice)
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.ValidVoiceChannelBtnErrorEmbedBuilder(e)));

                    var connection = node.GetGuildConnection(e.Guild);

                    if (connection! == null)
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.LavaLinkErrorEmbedBuilder()));

                    if (connection != null && connection.CurrentState.CurrentTrack == null)
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoAudioTrackErrorEmbedBuilder()));

                    if (connection != null)
                    {
                        var guildId = e.Guild.Id;
                        var audioPlayerController = Bot.GuildAudioPlayers[guildId];

                        if (audioPlayerController.Queue.Count != 0)
                        {
                            var queue = audioPlayerController.Queue;

                            audioPlayerController.CurrentSongData = queue.Peek();

                            var nextTrackInQueue = audioPlayerController.Queue.Dequeue();

                            await connection.PlayAsync(nextTrackInQueue);

                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder(
                                    audioEmbed.SongEmbedBuilder(e)));
                        }
                        else
                        {
                            await connection.StopAsync();
                            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                new DiscordInteractionResponseBuilder().AddEmbed(
                                    audioEmbed.QueueSomethingEmbedBuilder()));
                        }
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.NoSkipPermissionEmbedBuilder()));
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