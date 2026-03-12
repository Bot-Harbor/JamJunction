using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to restart the currently playing track.
/// </summary>
/// <remarks>
/// This command resets the playback position of the active track to the beginning
/// and resumes playback. It is useful when users want to replay the current track
/// without re-adding it to the queue.
/// </remarks>
public class RestartCommand : ApplicationCommandModule
{
    /// <summary>
    /// Provides access to the Lavalink audio service used for managing
    /// audio playback and retrieving player instances.
    /// </summary>
    /// <remarks>
    /// This service is used to interact with Lavalink through Lavalink4NET,
    /// allowing the application to control music playback, queues, filters,
    /// and other audio-related functionality.
    /// </remarks>
    private readonly IAudioService _audioService;

    public RestartCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Restarts the currently playing track from the beginning.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about the command
    /// invocation and the user executing the command.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the restart command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before restarting playback:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// </list>
    /// 
    /// If validation succeeds, the command resets the track's playback position
    /// to the beginning and resumes playback. The player embed message is then
    /// refreshed to reflect the restarted track.
    /// </remarks>
    [SlashCommand("restart", "Restarts the current track.")]
    public async Task RestartCommandAsync(InteractionContext context)
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
                    errorEmbed.ValidVoiceChannelError()));
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
                    errorEmbed.NoPlayerError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError()));
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
                    errorEmbed.NoConnectionError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.PlayerInactiveError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await player!.SeekAsync(TimeSpan.FromSeconds(0));
        await player.ResumeAsync();

        var guildData = Bot.GuildData[guildId];

        try
        {
            var updatedPlayerMessage = await context.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
            _ = updatedPlayerMessage.ModifyAsync(
                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, trackIsRestarted: true));
        }
        catch (Exception)
        {
            guildData.PlayerMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder(
                    audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, trackIsRestarted: true)));
        }

        var restartMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Restart(context)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(restartMessage.Id);
    }
}