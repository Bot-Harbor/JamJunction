using System.Threading.Channels;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace JamJunction.App.Events;

public class TrackStartedEvent
{
    private readonly DiscordClient _discordClient;
    private readonly IAudioService _audioService;

    public TrackStartedEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    public async Task TrackStarted(object sender, TrackStartedEventArgs eventargs)
    {
        var guildId = eventargs.Player.GuildId;
        var voiceChannel = eventargs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var track = eventargs.Track;

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel, connectToVoiceChannel: true);

        if (guildData.FirstSongInQueue)
        {
            await player.SetVolumeAsync((float) 0.5);
            guildData.FirstSongInQueue = false;
            
            return;
        }

        var audioPlayerEmbed = new AudioPlayerEmbed();
        await channel.SendMessageAsync(audioPlayerEmbed.SongInformation(track, player));
    }
}