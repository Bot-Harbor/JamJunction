using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class TrackStuck
{
    public static async Task TrackStuckAsync(LavalinkGuildConnection sender, TrackStuckEventArgs args)
    {
        var errorEmbed = new ErrorEmbed();
        var queue = PlayCommand.Queue;
        var channelId = PlayCommand.ChannelId;
        var channel = sender.Guild.GetChannel(channelId);
        
        var connection = sender;

        await connection.SeekAsync(TimeSpan.FromSeconds(0));
        await channel.SendMessageAsync(
            new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackStuckEmbedBuilder()));
    }
}