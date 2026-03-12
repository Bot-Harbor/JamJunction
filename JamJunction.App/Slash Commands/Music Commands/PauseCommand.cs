using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to pause the currently playing track.
/// </summary>
/// <remarks>
/// This command pauses the active Lavalink player in the guild's voice channel.
/// If playback is already paused or no track is playing, an error message is returned.
/// </remarks>
public class PauseCommand : ApplicationCommandModule
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

    public PauseCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Pauses the currently playing track in the voice channel.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing the command invocation
    /// details and the user executing the command.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the pause command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before pausing playback:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// <item>Ensures the player is not already paused.</item>
    /// </list>
    /// 
    /// If all checks pass, the player pauses playback and the player message
    /// embed is updated to reflect the paused state.
    /// </remarks>
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

        if (player.IsPaused)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.AlreadyPausedError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await player!.PauseAsync();

        var guildData = Bot.GuildData[guildId];

        try
        {
            var updatedPlayerMessage = await context.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
            _ = updatedPlayerMessage.ModifyAsync(
                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, pauseDisabled: true,
                    resumeDisabled: false));
        }
        catch (Exception)
        {
            guildData.PlayerMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder(
                    audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, pauseDisabled: true,
                        resumeDisabled: false)));
        }

        var pauseMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Pause(context)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(pauseMessage.Id);
    }
}