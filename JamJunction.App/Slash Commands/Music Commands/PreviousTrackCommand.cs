using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to return to the previously played track.
/// </summary>
/// <remarks>
/// This command retrieves the most recent track from the player's history
/// and begins playing it again, reinserting the current track at the front
/// of the queue so it is not lost.
/// </remarks>
public class PreviousTrackCommand : ApplicationCommandModule
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

    public PreviousTrackCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Returns to the previously played track in the playback history.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user who executed it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the previous track command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before going back:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// <item>Ensures there is at least one track in the playback history.</item>
    /// </list>
    ///
    /// If validation succeeds, the current track is reinserted at the front
    /// of the queue and the most recent track from history is played.
    /// </remarks>
    [SlashCommand("previous-track", "Returns to the previously played track.")]
    public async Task PreviousTrackCommandAsync(InteractionContext context)
    {
        await context.DeferAsync(true);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.ValidVoiceChannelError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.NoPlayerError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState!.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.SameVoiceChannelError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player = await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.NoConnectionError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (player.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.PlayerInactiveError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (!player.Queue.HasHistory || player.Queue.History.Count == 0)
        {
            var errorMessage = await context.FollowUpAsync(errorEmbed.NoPreviousTrackError());
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var previousIndex = player.Queue.History.Count - 1;
        var previousTrack = player.Queue.History[previousIndex].Track;
        
        await player.Queue.InsertAsync(0, new TrackQueueItem(player.CurrentTrack));
        
        await player.Queue.History.RemoveAtAsync(previousIndex);

        await player.PlayAsync(previousTrack!, false);

        var guildData = Bot.GuildData[guildId];

        try
        {
            var updatedPlayerMessage = await context.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
            await updatedPlayerMessage.ModifyAsync(
                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
        }
        catch (Exception)
        {
            guildData.PlayerMessage = await context.Channel.SendMessageAsync(
                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
        }

        var previousMessage = await context.FollowUpAsync(
            new DSharpPlus.Entities.DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.PreviousTrack()));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(previousMessage.Id);
    }
}
