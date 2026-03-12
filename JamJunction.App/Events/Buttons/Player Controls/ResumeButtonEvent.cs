using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player_Controls;

/// <summary>
/// Handles the resume button interaction for the audio player.
/// </summary>
/// <remarks>
/// This event is triggered when a user presses the resume button on the
/// player interface. The handler validates the user's voice channel,
/// retrieves the Lavalink player instance, resumes the currently paused
/// track, and updates the player embed message.
/// </remarks>
public class ResumeButtonEvent : IButton
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
    
    /// <summary>
    /// The Discord client used to interact with the Discord API.
    /// </summary>
    /// <remarks>
    /// This client provides access to guilds, channels, users, and events
    /// within Discord. It is commonly used to retrieve guild information,
    /// resolve voice states, and perform actions such as sending or deleting messages.
    /// </remarks>
    private readonly DiscordClient _discordClient;

    public ResumeButtonEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Gets or sets the voice channel that the interacting user is currently connected to.
    /// </summary>
    /// <remarks>
    /// This property is used to ensure that the user issuing the resume
    /// interaction is in the same voice channel as the bot before modifying
    /// playback state.
    /// </remarks>
    private DiscordChannel UserVoiceChannel { get; set; }

    /// <summary>
    /// Executes the resume button interaction logic.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="btnInteractionArgs">
    /// The interaction event arguments containing information about the
    /// button interaction, the user who triggered it, and the guild context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method performs several validation checks before resuming playback:
    /// - Ensures the user is connected to a voice channel.
    /// - Ensures the bot is connected to a voice channel.
    /// - Ensures the user is in the same voice channel as the bot.
    /// - Ensures the Lavalink player exists and is currently active.
    ///
    /// Once validated, playback of the current track is resumed and the
    /// player interface embed is updated to reflect the resumed state.
    /// </remarks>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "resume")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync();

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await btnInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoPlayerError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
            var player =
                await lavalinkPlayer.GetPlayerAsync(guildId, UserVoiceChannel, false);

            if (player == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoConnectionError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.PlayerInactiveError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            await player!.ResumeAsync();

            var guildData = Bot.GuildData[guildId];

            try
            {
                await channel.EditFollowupMessageAsync(guildData.PlayerMessage.Id,
                    new DiscordWebhookBuilder(
                        audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, resumeDisabled: true)));
            }
            catch (Exception)
            {
                guildData.PlayerMessage =
                    await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder(
                            audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, resumeDisabled: true)));
            }

            var resumeMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    audioPlayerEmbed.Resume(btnInteractionArgs)));

            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(resumeMessage.Id);
        }
    }
}