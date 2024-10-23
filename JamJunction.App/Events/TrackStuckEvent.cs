using DSharpPlus;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;

namespace JamJunction.App.Events;

public class TrackStuckEvent
{
    private readonly DiscordClient _discordClient;
    private readonly IAudioService _audioService;

    public TrackStuckEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    public async Task TrackStuck(object sender, TrackStuckEventArgs eventargs)
    {
        var guildId = eventargs.Player.GuildId;
        var voiceChannel = eventargs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);
        
        var errorEmbed = new ErrorEmbed();
        await channel.SendMessageAsync(errorEmbed.TrackFailedToLoadError());
        
        await Task.Delay(TimeSpan.FromSeconds(5));
        
        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel, connectToVoiceChannel: true);
        
        var track = eventargs.Track;
        await player.PlayAsync(track);

        await Task.Delay(TimeSpan.FromSeconds(3));
        
        if (player.State == PlayerState.NotPlaying)
        {
            await channel.SendMessageAsync(errorEmbed.CouldNotLoadTrackOnAttemptError());
            Bot.GuildData.Remove(guildId);
        }
    } 
}