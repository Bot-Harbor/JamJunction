using DSharpPlus.EventArgs;

namespace JamJunction.App.Events;

public class VoiceStateUpdated
{
    public static Task UserDisconnectsPlayer(object sender, VoiceStateUpdateEventArgs e)
    {
        if (e.User == Bot.Client.CurrentUser)
        {
            var guildId = e.Guild.Id;
            var audioPlayerController = Bot.GuildAudioPlayers[guildId];
            
            audioPlayerController.Volume = 50;
            audioPlayerController.PauseInvoked = false;
            audioPlayerController.MuteInvoked = false;
            audioPlayerController.FirstSongInTrack = true;
            audioPlayerController.Queue.Clear();
            
            // Does stop lavalink. May be useless
            Console.WriteLine("Voice state has been removed");
        }

        return Task.CompletedTask;
    }
}