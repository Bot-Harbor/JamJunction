﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PauseCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public PauseCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("pause", "Pauses the current track.")]
    public async Task PauseCommandAsync(InteractionContext context)
    {
        await context.DeferAsync();

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError(context)));
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoPlayerError(context)));
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError(context)));
            return;
        }

        if (player!.CurrentTrack == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoAudioTrackError(context)));
            return;
        }

        if (player.IsPaused)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.AlreadyPausedError(context)));
            return;
        }

        await player!.PauseAsync();

        await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Pause(context)));
    }
}