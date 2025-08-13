using DSharpPlus;
using DSharpPlus.Entities;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;

namespace JamJunction.App.Events.Player;

public class TrackStuckEvent
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public TrackStuckEvent(DiscordClient discordClient, IAudioService audioService)
    {
        _discordClient = discordClient;
        _audioService = audioService;
    }

    public async Task TrackStuck(object sender, TrackStuckEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var errorEmbed = new ErrorEmbed();

        var errorMessage = await channel.SendMessageAsync(errorEmbed.BuildTrackFailedToLoadError());
        
        await Task.Delay(5000);

        _ = channel.DeleteMessageAsync(errorMessage);
        
        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        var track = eventArgs.Track;
        await player.PlayAsync(track);

        await Task.Delay(3000);

        if (player.State == PlayerState.NotPlaying)
        {
            errorMessage = await channel.SendMessageAsync(errorEmbed.BuildCouldNotLoadTrackOnAttemptError());
            
            await Task.Delay(10000);

            await channel.DeleteMessageAsync(errorMessage);
            
            Bot.GuildData.Remove(guildId);
        }
    }
}