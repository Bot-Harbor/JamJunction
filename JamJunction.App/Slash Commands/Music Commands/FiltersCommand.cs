using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using JamJunction.App.Views.Menus;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

/// <summary>
/// Slash command used to display available audio filters for the player.
/// </summary>
/// <remarks>
/// This command was previously used to allow users to select audio filters
/// for the Lavalink player through a menu interface. It is no longer actively
/// used but remains in the codebase for archival and reference purposes.
/// </remarks>
public class FiltersCommand : ApplicationCommandModule
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

    public FiltersCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    /// <summary>
    /// Displays the audio filter selection menu for the Lavalink player.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user executing it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the filters command.
    /// </returns>
    /// <remarks>
    /// This command validates the user's voice channel and ensures that
    /// a Lavalink player is active before displaying the filter selection menu.
    /// The menu allows users to apply audio filters such as Nightcore,
    /// Vaporwave, or Karaoke to the current playback session.
    /// </remarks>
    [SlashCommand("filters", "Change filter for the player.")]
    public async Task FiltersCommandAsync(InteractionContext context)
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

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent(" ")
            .AddComponents(audioPlayerMenu.BuildFilters())));
    }
}