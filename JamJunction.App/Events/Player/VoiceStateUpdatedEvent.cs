using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Player;

public class VoiceStateUpdatedEvent
{
    private readonly IAudioService _audioService;

    public VoiceStateUpdatedEvent(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public async Task VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args)
    {
        var guild = args.Guild;
        var voiceChannel = args.Before?.Channel;

        if (voiceChannel == null)
            return;
        
        var userUpdated = args.User.Id;
        if (Bot.UserData.ContainsKey(userUpdated))
        {
            Bot.UserData.Remove(userUpdated);
        }
        
        var users = voiceChannel.Users;
        if (users.Count == 1 && users.First().Id == sender.CurrentUser.Id)
        {
            var guildId = guild.Id;
            var voiceChannelId = voiceChannel.Id;

            var lavaPlayerHandler = new LavalinkPlayerHandler(_audioService);
            var player = await lavaPlayerHandler.GetPlayerAsync(guildId, voiceChannelId);

            await player.DisconnectAsync();
        }
    }
}