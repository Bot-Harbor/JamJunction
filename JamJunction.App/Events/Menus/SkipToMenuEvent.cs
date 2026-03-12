using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Events.Menus;

/// <summary>
/// Handles queue navigation interactions from the skip-to select menu.
/// </summary>
/// <remarks>
/// This menu allows users to skip directly to a specific track in the
/// Lavalink queue. The selected track index is retrieved from the menu
/// values and the player skips forward to that position.
///
/// After the skip operation completes, a confirmation message is sent
/// indicating the new track that is currently playing.
/// </remarks>
public class SkipToMenuEvent : IMenu
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

    public SkipToMenuEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Gets or sets the voice channel that the interacting user is currently connected to.
    /// </summary>
    /// <remarks>
    /// This value is used to verify that the user is connected to the same
    /// voice channel as the bot before allowing queue navigation actions.
    /// </remarks>
    private DiscordChannel UserVoiceChannel { get; set; }

    /// <summary>
    /// Executes the skip-to queue menu interaction.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="menuInteractionArgs">
    /// The interaction event arguments containing the selected queue position
    /// and interaction context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution of the
    /// skip-to operation.
    /// </returns>
    /// <remarks>
    /// This method validates the user's voice channel, retrieves the active
    /// Lavalink player, and skips playback to the selected queue index.
    ///
    /// If the selected track does not exist, an error message is returned
    /// and the queue view is refreshed.
    /// </remarks>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "queue-menu")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = menuInteractionArgs.Guild.Id;

            var memberId = menuInteractionArgs.User.Id;
            var member = await menuInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = menuInteractionArgs.Interaction;

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
            var bot = await menuInteractionArgs.Guild.GetMemberAsync(botId);
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

            try
            {
                foreach (var value in menuInteractionArgs.Values)
                {
                    await player.SkipAsync(Convert.ToInt32(value));
                    _ = channel.DeleteFollowupMessageAsync(menuInteractionArgs.Message.Id);

                    var message = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(audioPlayerEmbed.SkipTo(menuInteractionArgs,
                            player)));

                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(message.Id);
                    break;
                }
            }
            catch (Exception)
            {
                var userData = Bot.UserData[menuInteractionArgs.User.Id];

                await channel.DeleteFollowupMessageAsync(userData.ViewQueueMessage.Id);

                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.TrackDoesNotExistError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
            }
        }
    }
}