using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events;

public class VoiceStateUpdated
{
    public static Task ClientOnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args)
    {
        if (args.User?.Id == sender.CurrentUser?.Id)
        {
            if (args.Before?.Channel != null && args.After?.Channel == null)
            {
                var guildId = args.Guild.Id;
                Bot.GuildAudioPlayers.Remove(guildId, out var audioPlayerController);
            }
        }

        return Task.CompletedTask;
    }
}