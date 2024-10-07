using DSharpPlus;
using JamJunction.App.Embed_Builders;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using Microsoft.Extensions.Options;

namespace JamJunction.App.Events;

public class TrackEndedEvent
{
    private readonly DiscordClient _discordClient;
    private readonly IAudioService _audioService;

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
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel, connectToVoiceChannel: true);

        if (eventargs.Reason == TrackEndReason.Stopped)
        {
            Bot.GuildData.Remove(guildId);
            return;
        }
        
        if (player.State == PlayerState.NotPlaying)
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            await channel.SendMessageAsync(audioPlayerEmbed.QueueSomethingEmbedBuilder());
            Bot.GuildData.Remove(guildId);
        }
    }
}