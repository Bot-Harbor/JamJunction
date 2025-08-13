using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embeds;
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
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildValidVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildNoPlayerError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildSameVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildNoConnectionError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildNoAudioTrackError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        player!.RepeatMode = repeatingMode switch
        {
            RepeatingMode.None => TrackRepeatMode.None,
            RepeatingMode.RepeatTrack => TrackRepeatMode.Track,
            RepeatingMode.RepeatQueue => TrackRepeatMode.Queue,
            _ => TrackRepeatMode.None
        };

        var guildData = Bot.GuildData[guildId];

        var updatedPlayerMessage = await context.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
        await updatedPlayerMessage.ModifyAsync(audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));

        var repeatMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Repeat(context, player)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(repeatMessage.Id);
    }
}