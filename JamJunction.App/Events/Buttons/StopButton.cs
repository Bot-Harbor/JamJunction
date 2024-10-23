﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons;

public class StopButton : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;
    private DiscordChannel UserVoiceChannel { get; set; }

    public StopButton(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "stop")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync();

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError(btnInteractionArgs)));

                    return;
                }
            }
            catch (Exception)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError(btnInteractionArgs)));

                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await btnInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoPlayerError(btnInteractionArgs)));

                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError(btnInteractionArgs)));

                return;
            }

            var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
            var player =
                await lavalinkPlayer.GetPlayerAsync(guildId, UserVoiceChannel, connectToVoiceChannel: false);

            if (player == null)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoConnectionError(btnInteractionArgs)));

                return;
            }

            if (player!.CurrentTrack == null)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoAudioTrackError(btnInteractionArgs)));

                return;
            }

            await player!.StopAsync();

            await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    audioPlayerEmbed.Stop(btnInteractionArgs)));
        }
    }
}