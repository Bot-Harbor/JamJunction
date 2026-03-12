using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to display information about the currently playing track.
/// </summary>
/// <remarks>
/// This command retrieves the active Lavalink player and displays
/// details about the track currently being played in the guild.
/// The existing player message is replaced with an updated embed
/// containing the current track information.
/// </remarks>
public class CurrentTrackCommand : ApplicationCommandModule
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

    public CurrentTrackCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Displays information about the track currently playing in the voice channel.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user who executed it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the slash command.
    /// </returns>
    /// <remarks>
    /// This command performs several validations before displaying
    /// the current track:
    /// <list type="bullet">
    /// <item>
    /// Ensures the user is connected to a voice channel.
    /// </item>
    /// <item>
    /// Ensures the bot is connected to a voice channel.
    /// </item>
    /// <item>
    /// Ensures the user is in the same voice channel as the bot.
    /// </item>
    /// <item>
    /// Ensures an active Lavalink player exists and a track is playing.
    /// </item>
    /// </list>
    /// 
    /// If validation succeeds, the existing player embed message is
    /// replaced with an updated embed showing the current track details.
    /// </remarks>
    [SlashCommand("current-track", "Shows details about the current track playing.")]
    public async Task CurrentTrackCommandAsync(InteractionContext context)
    {
        await context.DeferAsync();

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();
        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        var channel = context.Channel;

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
            _ = channel.DeleteMessageAsync(errorMessage);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError()));
            await Task.Delay(10000);
            _ = channel.DeleteMessageAsync(errorMessage);
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
            _ = channel.DeleteMessageAsync(errorMessage);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.PlayerInactiveError()));
            await Task.Delay(10000);
            _ = channel.DeleteMessageAsync(errorMessage);
            return;
        }

        var guildData = Bot.GuildData[guildId];
        
        channel = context.Channel;
        _ = channel.DeleteMessageAsync(guildData.PlayerMessage);

        var playerMessage = await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(audioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));
        guildData.PlayerMessage = playerMessage;
    }
}