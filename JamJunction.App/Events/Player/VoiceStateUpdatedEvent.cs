using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Player;

/// <summary>
/// Handles Discord voice state update events related to the Lavalink player.
/// </summary>
/// <remarks>
/// This event monitors voice channel membership changes to determine
/// whether the bot should automatically disconnect from a voice channel.
///
/// If all users leave the voice channel except the bot, the Lavalink
/// player is disconnected and playback is stopped. This prevents the
/// bot from remaining in empty voice channels.
/// </remarks>
public class VoiceStateUpdatedEvent
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

    public VoiceStateUpdatedEvent(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Executes logic when a user's voice state changes.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the event.
    /// </param>
    /// <param name="args">
    /// The <see cref="VoiceStateUpdateEventArgs"/> containing information
    /// about the user's previous and updated voice state.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous voice state
    /// handling operation.
    /// </returns>
    /// <remarks>
    /// This method performs two main actions:
    /// <list type="bullet">
    /// <item>
    /// Removes cached user interaction data if the user leaves the voice channel.
    /// </item>
    /// <item>
    /// Automatically disconnects the Lavalink player when the bot becomes
    /// the only remaining member in the voice channel.
    /// </item>
    /// </list>
    /// </remarks>
    public async Task VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args)
    {
        var guild = args.Guild;
        var voiceChannel = args.Before?.Channel;

        if (voiceChannel == null)
            return;

        var userUpdated = args.User.Id;
        if (Bot.UserData.ContainsKey(userUpdated)) Bot.UserData.Remove(userUpdated);

        var users = voiceChannel.Users;
        if (users.Count == 1 && users.First().Id == sender.CurrentUser.Id)
        {
            var guildId = guild.Id;
            var voiceChannelId = voiceChannel.Id;

            var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
            var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannelId);

            await player.DisconnectAsync();
        }
    }
}