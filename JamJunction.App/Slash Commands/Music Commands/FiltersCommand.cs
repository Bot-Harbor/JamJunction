using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using JamJunction.App.Menu_Builders;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class FiltersCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public FiltersCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

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
                    errorEmbed.ValidVoiceChannelError(context)));
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
                    errorEmbed.NoPlayerError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));
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
                    errorEmbed.NoConnectionError(context)));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent(" ")
            .AddComponents(audioPlayerMenu.Filters())));
    }
}