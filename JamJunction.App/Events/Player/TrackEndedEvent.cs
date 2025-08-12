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

    public async Task TrackEnded(object sender, TrackEndedEventArgs eventArgs)
    {
        var guildId = eventArgs.Player.GuildId;
        var voiceChannel = eventArgs.Player.VoiceChannelId;
        var guild = await _discordClient.GetGuildAsync(guildId);

        var guildData = Bot.GuildData[guildId];
        var textChannelId = guildData.TextChannelId;
        var channel = guild.GetChannel(textChannelId);

        var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
        var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannel);

        if (eventArgs.Reason == TrackEndReason.Stopped)
        {
            _ = channel.DeleteMessageAsync(guildData.PlayerMessage);
            
            foreach (var userData in Bot.UserData.Values)
            {
                if (userData.GuildId == guildId)
                {
                    var userToRemove = Bot.UserData.FirstOrDefault(x =>
                        x.Value.GuildId == guildId).Key;
                    Bot.UserData.Remove(userToRemove);
                }
            }
            
            Bot.GuildData.Remove(guildId);
            return;
        }

        if (player.State == PlayerState.NotPlaying)
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            await channel.DeleteMessageAsync(guildData.PlayerMessage);

            var queueSomethingMessage = await channel.SendMessageAsync(audioPlayerEmbed.QueueSomething());

            await Task.Delay(10000);

            _ = channel.DeleteMessageAsync(queueSomethingMessage);
            
            foreach (var userData in Bot.UserData.Values)
            {
                if (userData.GuildId == guildId)
                {
                    var userToRemove = Bot.UserData.FirstOrDefault(x =>
                        x.Value.GuildId == guildId).Key;
                    Bot.UserData.Remove(userToRemove);
                }
            }
            
            Bot.GuildData.Remove(guildId);
        }
    }
}