using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class StopCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public StopCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("stop", "Stops the playback.")]
    public async Task StopCommandAsync(InteractionContext context)
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
                    errorEmbed.ValidVoiceChannelError(context)));
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
                    errorEmbed.NoPlayerError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));
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
                    errorEmbed.NoConnectionError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoAudioTrackError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await player!.StopAsync();

        var message = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Stop(context)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(message.Id);
    }
}