using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class RepeatCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public RepeatCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("repeating-mode", "Change the repeating mode.")]
    public async Task RepeatCommandAsync(InteractionContext context,
        [Option("mode", "Select the repeating mode.")]
        RepeatingMode repeatingMode = default)
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

        player!.RepeatMode = repeatingMode switch
        {
            RepeatingMode.None => TrackRepeatMode.None,
            RepeatingMode.RepeatTrack => TrackRepeatMode.Track,
            RepeatingMode.RepeatQueue => TrackRepeatMode.Queue,
            _ => TrackRepeatMode.None
        };

        await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Repeat(context, player)));
    }
}