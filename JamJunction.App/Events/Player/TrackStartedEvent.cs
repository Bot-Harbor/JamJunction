using DSharpPlus;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using JamJunction.App.Views.Embeds;
using Lavalink4NET;
using Lavalink4NET.Events.Players;

namespace JamJunction.App.Events.Player;

public class TrackStartedEvent
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public TrackStartedEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    public async Task TrackStarted(object sender, TrackStartedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var track = eventArgs.Track;

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (guildData.FirstSongInQueue)
        {
            guildData.FirstSongInQueue = false;
            return;
        }

        var playerMessage = guildData.PlayerMessage;
        _ = channel.DeleteMessageAsync(playerMessage);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        playerMessage = await channel.SendMessageAsync(audioPlayerEmbed.TrackInformation(track, player, true));
        guildData.PlayerMessage = playerMessage;
    }
}