using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Modals.Interfaces;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Events.Modals;

/// <summary>
/// Handles modal submissions used to navigate to a specific queue page.
/// </summary>
/// <remarks>
/// This modal allows users to jump directly to a specific page in the
/// queue view. The submitted page number determines which portion of
/// the Lavalink queue is displayed.
///
/// The queue is paginated in groups of tracks, and the modal updates
/// the existing queue embed message to display the selected page.
/// </remarks>
public class PageNumberModalEvent : IModal
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

    public PageNumberModalEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Gets or sets the voice channel that the interacting user is currently connected to.
    /// </summary>
    /// <remarks>
    /// This value is used to ensure the user is connected to the same voice
    /// channel as the bot before allowing queue navigation actions.
    /// </remarks>
    private DiscordChannel UserVoiceChannel { get; set; }

    /// <summary>
    /// Executes the modal submission logic for queue page navigation.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the modal interaction.
    /// </param>
    /// <param name="modalEventArgs">
    /// The modal submission event arguments containing the user input
    /// and interaction context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the page navigation operation.
    /// </returns>
    /// <remarks>
    /// This method validates the user's voice channel, retrieves the active
    /// Lavalink player, and updates the queue embed to display the page
    /// specified by the user.
    ///
    /// If the requested page does not exist based on the current queue size,
    /// an error message is returned.
    /// </remarks>
    public async Task Execute(DiscordClient sender, ModalSubmitEventArgs modalEventArgs)
    {
        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = modalEventArgs.Interaction.Guild.Id;

        var memberId = modalEventArgs.Interaction.User.Id;
        var member = await modalEventArgs.Interaction.Guild.GetMemberAsync(memberId);

        var channel = modalEventArgs.Interaction;

        await channel.DeferAsync(true);

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
        var bot = await modalEventArgs.Interaction.Guild.GetMemberAsync(botId);
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

        var userId = modalEventArgs.Interaction.User.Id;

        var values = modalEventArgs.Values;
        var pageNumber = values["page-number"];

        var userData = Bot.UserData[userId];

        var loadingMessage = await channel.CreateFollowupMessageAsync(
            new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent("Loading...")));

        await Task.Delay(500);

        _ = channel.DeleteFollowupMessageAsync(loadingMessage.Id);

        var totalTracks = player.Queue.Count;
        switch (pageNumber)
        {
            case "1":
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player)));
                break;
            case "2" when totalTracks > 15:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "2")));
                break;
            case "3" when totalTracks > 30:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "3")));
                break;
            case "4" when totalTracks > 45:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "4")));
                break;
            case "5" when totalTracks > 60:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "5")));
                break;
            case "6" when totalTracks > 75:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "6")));
                break;
            case "7" when totalTracks > 90:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, "7")));
                break;
            default:
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.PageNumberDoesNotExistError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                break;
            }
        }
    }
}