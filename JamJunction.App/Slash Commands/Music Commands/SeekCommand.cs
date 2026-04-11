using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to change the playback position of the current track.
/// </summary>
/// <remarks>
/// This command allows users to seek to a specific timestamp within the
/// currently playing track. The position is provided in seconds and must
/// fall within the duration of the track.
/// </remarks>
public class SeekCommand : ApplicationCommandModule
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

    public SeekCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Sets the playback position of the current track.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user executing the command.
    /// </param>
    /// <param name="hour">
    /// Changes the current position of the track in hours.
    /// </param>
    /// <param name="minute">
    /// Changes the current position of the track in hours.
    /// </param>
    /// <param name="second">
    /// Changes the current position of the track in seconds.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the seek command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before seeking the track:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// <item>Ensures the provided time value is a valid integer.</item>
    /// <item>Ensures the provided time does not exceed the track duration.</item>
    /// </list>
    /// If validation succeeds, the Lavalink player seeks to the requested
    /// timestamp and the player embed is updated to reflect the new position.
    /// </remarks>
    [SlashCommand("seek", "Sets the position of the track.")]
    public async Task SeekCommandAsync(InteractionContext context,
        [Option("hour", "Change the current position of the track in hours.")]
        double hour = 0,
        [Option("minute", "Change the current position of the track in minutes.")]
        double minute = 0,
        [Option("second", "Change the current position of the track in seconds.")]
        double second = 0)
    {
        await context.DeferAsync(true);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.ValidVoiceChannelError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.NoPlayerError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.SameVoiceChannelError());
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
                errorEmbed.NoConnectionError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.PlayerInactiveError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var time = new TimeSpan
        (
            (int)Math.Round(hour), 
            (int)Math.Round(minute),
            (int)Math.Round(second)
        );

        var duration = player.CurrentTrack.Duration;

        if (time > duration)
        {
            var errorMessage = await context.FollowUpAsync(
                errorEmbed.SeekLargerThanDurationError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await player.SeekAsync(time);

        var guildData = Bot.GuildData[guildId];

        try
        {
            var updatedPlayerMessage = await context.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
            _ = updatedPlayerMessage.ModifyAsync(audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
        }
        catch (Exception)
        {
            guildData.PlayerMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));
        }

        var seekMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Seek(context, time)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(seekMessage.Id);
    }
}