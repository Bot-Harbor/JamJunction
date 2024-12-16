using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Menu_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class FiltersCommand : ApplicationCommandModule
{
    [SlashCommand("filters", "Change filter for the player.")]
    public async Task FiltersCommandAsync(InteractionContext context)
    {
        await context.DeferAsync();

        var audioPlayerMenu = new AudioPlayerMenu();
        var errorEmbed = new ErrorEmbed();

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