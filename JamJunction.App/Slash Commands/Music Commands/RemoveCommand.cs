using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using JamJunction.App.Views.Menus;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command previously used to remove tracks from the queue.
/// </summary>
/// <remarks>
/// This command is no longer actively used in the bot and is kept
/// only for archival or reference purposes. The functionality was
/// replaced by interactive queue controls that allow users to remove
/// tracks directly through menus.
/// </remarks>
public class RemoveCommand : ApplicationCommandModule
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

    public RemoveCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Displays a menu allowing the user to remove a track from the queue.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user executing the command.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the remove command.
    /// </returns>
    /// <remarks>
    /// This command performs several validation checks before displaying
    /// the removal menu:
    /// <list type="bullet">
    /// <item>Ensures the user is connected to a voice channel.</item>
    /// <item>Ensures the bot is currently connected to a voice channel.</item>
    /// <item>Ensures the user and bot are in the same voice channel.</item>
    /// <item>Ensures an active Lavalink player connection exists.</item>
    /// <item>Ensures there are tracks available in the queue.</item>
    /// </list>
    /// 
    /// If validation succeeds, the command displays an interactive menu
    /// allowing the user to select and remove tracks from the current queue.
    /// </remarks>
    [SlashCommand("remove", "Removes a track from the queue.")]
    public async Task RemoveCommandAsync(InteractionContext context)
    {
        await context.DeferAsync(true);

        var errorEmbed = new ErrorEmbed();
        var audioPlayerMenu = new AudioPlayerMenu();

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

        if (player.Queue.IsEmpty)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.RemoveError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent(" ")
            .AddComponents(audioPlayerMenu.BuildRemove(player))));
    }
}