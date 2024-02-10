using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class TrackStuck
{
    public static Task TrackIsStuck(LavalinkGuildConnection sender, TrackStuckEventArgs args)
    {
        var guildId = sender.Guild.Id;
        var errorEmbed = new ErrorEmbed();
        var channel = sender.Channel.Guild.GetChannel(guildId);
        var connection = sender;

        connection.SeekAsync(TimeSpan.FromSeconds(0));
        channel.SendMessageAsync(
            new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackStuckEmbedBuilder()));

        return Task.CompletedTask;
    }
}