using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
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
        await context.DeferAsync();

        var audioPlayerMenu = new AudioPlayerMenu();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError(context)));
            return;
        }

        await context.FollowUpAsync(new DiscordFollowupMessageBuilder(audioPlayerMenu.Filters()));
    }
}