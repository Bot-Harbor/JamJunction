using DSharpPlus;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
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
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (guildData.FirstSongInQueue)
        {
            guildData.FirstSongInQueue = false;
            return;
        }

        var message = guildData.Message;
        _ = channel.DeleteMessageAsync(message);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        message = await channel.SendMessageAsync(audioPlayerEmbed.TrackInformation(track, player, true));
        guildData.Message = message;
    }
}