using DSharpPlus.EventArgs;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class ResetDefaultVolume
{
    public static void ResetVolume(object sender, VoiceStateUpdateEventArgs e)
    {
        if (e.User == Bot.Client.CurrentUser)
        {
            VolumeCommand.VolumeCommandInvoked = false;
            PlayCommand.FirstTrackOnConnection = true;
            PlayCommand.DefaultVolume = 50;
        }
    }
}