using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to display the current playback position of the active track.
/// </summary>
/// <remarks>
/// This command retrieves the current playback position from the Lavalink player
/// and displays it to the user. The player embed message is also refreshed to
/// reflect the most up-to-date playback state.
/// </remarks>
public class PositionCommand : ApplicationCommandModule
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

    public PositionCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Retrieves the current playback position of the track that is currently playing.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about the command
    /// invocation and the user executing the command.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the position command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before retrieving the playback position:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures a track is currently playing.</item>
    /// </list>
    /// 
    /// If validation succeeds, the command retrieves the current playback timestamp
    /// from the Lavalink player and sends a message showing the track's position.
    /// The main player embed is also refreshed to keep it synchronized with playback.
    /// </remarks>
    [SlashCommand("position", "Gets the current track position.")]
    public async Task PositionAsync(InteractionContext context)
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

        var position = player.Position!.Value.Position;

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

        var positionMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.TrackPosition(position)));

        await Task.Delay(10000);
        _ = context.DeleteFollowupAsync(positionMessage.Id);
    }
}