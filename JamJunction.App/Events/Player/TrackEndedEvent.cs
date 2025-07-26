using DSharpPlus;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Protocol.Payloads.Events;

namespace JamJunction.App.Events.Player;

public class TrackEndedEvent
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public TrackEndedEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    public async Task TrackEnded(object sender, TrackEndedEventArgs eventargs)
    {
        var guildId = eventargs.Player.GuildId;
        var voiceChannel = eventargs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (eventargs.Reason == TrackEndReason.Stopped)
        {
            _ = channel.DeleteMessageAsync(guildData.Message);
            Bot.GuildData.Remove(guildId);
            return;
        }

        if (player.State == PlayerState.NotPlaying)
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            await channel.DeleteMessageAsync(guildData.Message);

            var message = await channel.SendMessageAsync(audioPlayerEmbed.QueueSomething());

            await Task.Delay(10000);

            _ = channel.DeleteMessageAsync(message);
            Bot.GuildData.Remove(guildId);
        }
    }
}