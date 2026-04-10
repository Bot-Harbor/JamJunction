using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Views.Embeds;

namespace JamJunction.App.Slash_Commands.Other_Commands;

/// <summary>
/// Slash command used to navigate a user directly to their DM channel with the bot,
/// where their personal playlist of liked songs is stored.
/// </summary>
public class PersonalPlaylistCommand : ApplicationCommandModule
{
    /// <summary>
    /// Responds with a link that opens the user's DM channel with the bot.
    /// </summary>
    /// <param name="context">
    /// The <see cref="InteractionContext"/> containing information about
    /// the command invocation and the user who executed it.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous execution
    /// of the slash command.
    /// </returns>
    [SlashCommand("personal-playlist", "Opens your personal playlist in the bot's DMs.")]
    public async Task PersonalPlaylistCommandAsync(InteractionContext context)
    {
        var audioPlayerEmbed = new AudioPlayerEmbed();
        var dmChannel = await context.Member.CreateDmChannelAsync();

        await context.CreateResponseAsync(
            new DiscordInteractionResponseBuilder()
                .AsEphemeral()
                .AddEmbed(audioPlayerEmbed.PersonalPlaylist())
                .AddComponents(audioPlayerEmbed.PersonalPlaylistButton(dmChannel.Id)));
    }
}
