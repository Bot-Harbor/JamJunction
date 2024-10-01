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
        var voiceChannelId = eventargs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);
        
        var track = eventargs.Track;
        var player = await GetPlayerAsync(guildId, voiceChannelId, connectToVoiceChannel: true);

        var guildController = Bot.GuildData[guildId];
        var textChannelId = guildController.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        if (!guildController.FirstSongInQueue)
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            await channel.SendMessageAsync(audioPlayerEmbed.SongEmbedBuilder(track, player));   
        }
    }

    private async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(ulong guildId, ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

        var result = await _audioService.Players
            .RetrieveAsync(guildId, voiceChannelId,
                playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

        return result.Player;
    }
}