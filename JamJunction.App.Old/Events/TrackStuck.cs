using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Events;

public class TrackStuck
{
    public static Task TrackIsStuck(LavalinkGuildConnection sender, TrackStuckEventArgs args)
    {
        var errorEmbed = new ErrorEmbed();

        var guildId = sender.Guild.Id;
        var audioPlayerController = Bot.GuildAudioPlayers[guildId];

        var channelId = audioPlayerController.ChannelId;
        var channel = sender.Guild.GetChannel(channelId);

        sender.SeekAsync(TimeSpan.FromSeconds(0));
        channel.SendMessageAsync(
            new DiscordMessageBuilder().AddEmbed(errorEmbed.TrackStuckEmbedBuilder()));

        return Task.CompletedTask;
    }
}