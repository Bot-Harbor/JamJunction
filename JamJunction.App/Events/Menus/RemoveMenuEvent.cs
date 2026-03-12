using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;

namespace JamJunction.App.Events.Menus;

/// <summary>
/// Handles queue removal interactions from the track removal select menu.
/// </summary>
/// <remarks>
/// This menu allows users to remove a specific track from the current
/// Lavalink queue. The selected track index is retrieved from the menu
/// values and removed from the player's queue.
///
/// After removal, the player embed is updated to reflect the modified
/// queue state and a confirmation message is displayed to the user.
/// </remarks>
public class RemoveMenuEvent : IMenu
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

    public RemoveMenuEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Gets or sets the voice channel that the interacting user is currently connected to.
    /// </summary>
    /// <remarks>
    /// This value is used to verify that the user is connected to the same
    /// voice channel as the bot before allowing queue modifications.
    /// </remarks>
    private DiscordChannel UserVoiceChannel { get; set; }

    /// <summary>
    /// Executes the remove track menu interaction.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="DiscordClient"/> instance that triggered the interaction.
    /// </param>
    /// <param name="menuInteractionArgs">
    /// The interaction event arguments containing the selected menu values
    /// and user interaction context.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method validates the user's voice channel, retrieves the active
    /// Lavalink player, and removes the selected track from the queue.
    ///
    /// If the track index is invalid or no longer exists, an error message
    /// is displayed to the user.
    /// </remarks>
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "remove")
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

            var guildData = Bot.GuildData[guildId];

            try
            {
                foreach (var value in menuInteractionArgs.Values)
                {
                    var removedTrack = player.Queue[Convert.ToInt32(value)].Track;

                    await player.Queue.RemoveAtAsync(Convert.ToInt32(value));
                    _ = channel.DeleteFollowupMessageAsync(menuInteractionArgs.Message.Id);

                    try
                    {
                        var updatedPlayerMessage = await channel.Channel.GetMessageAsync(guildData.PlayerMessage.Id);
                        _ = updatedPlayerMessage.ModifyAsync(
                            audioPlayerEmbed.TrackInformation(player.CurrentTrack, player));
                    }
                    catch (Exception)
                    {
                        guildData.PlayerMessage = await channel.CreateFollowupMessageAsync(
                            new DiscordFollowupMessageBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));
                    }

                    var removeMenuMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(audioPlayerEmbed.Remove(menuInteractionArgs,
                            removedTrack)));

                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(removeMenuMessage.Id);
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