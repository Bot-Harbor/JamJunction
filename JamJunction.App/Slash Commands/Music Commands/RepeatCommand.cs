using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to change the repeating behavior of the music player.
/// </summary>
/// <remarks>
/// This command allows users to configure how the Lavalink player repeats tracks.
/// Users can disable repeating, repeat the current track, or repeat the entire queue.
/// </remarks>
public class RepeatCommand : ApplicationCommandModule
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

    public RepeatCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Changes the repeating mode for the Lavalink player.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user executing the command.
    /// </param>
    /// <param name="repeatingMode">
    /// The selected <see cref="RepeatingMode"/> which determines how the
    /// player should repeat tracks.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the repeat command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before applying
    /// the repeating mode:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// </list>
    /// 
    /// Once validated, the command updates the player's repeat mode:
    /// <list type="bullet">
    /// <item><see cref="RepeatingMode.None"/> disables repeating.</item>
    /// <item><see cref="RepeatingMode.RepeatTrack"/> repeats the current track.</item>
    /// <item><see cref="RepeatingMode.RepeatQueue"/> repeats the entire queue.</item>
    /// </list>
    /// 
    /// After updating the repeat mode, the player embed message is refreshed
    /// and a confirmation embed is displayed.
    /// </remarks>
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

        player!.RepeatMode = repeatingMode switch
        {
            RepeatingMode.None => TrackRepeatMode.None,
            RepeatingMode.RepeatTrack => TrackRepeatMode.Track,
            RepeatingMode.RepeatQueue => TrackRepeatMode.Queue,
            _ => TrackRepeatMode.None
        };

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

        var repeatMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Repeat(context, player)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(repeatMessage.Id);
    }
}