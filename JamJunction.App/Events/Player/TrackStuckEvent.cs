using DSharpPlus;
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
        await channel.SendMessageAsync(errorEmbed.BuildTrackFailedToLoadError());

        await Task.Delay(TimeSpan.FromSeconds(5));

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        var track = eventArgs.Track;
        await player.PlayAsync(track);

        await Task.Delay(TimeSpan.FromSeconds(3));

        if (player.State == PlayerState.NotPlaying)
        {
            await channel.SendMessageAsync(errorEmbed.BuildCouldNotLoadTrackOnAttemptError());
            Bot.GuildData.Remove(guildId);
        }
    }
}