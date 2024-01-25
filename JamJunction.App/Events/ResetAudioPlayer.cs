using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events;

public class ResetAudioPlayer
{
    public static void UserDisconnectsPlayer(object sender, VoiceStateUpdateEventArgs e)
    {
        if (e.User == Bot.Client.CurrentUser)
        {
            VolumeCommand.VolumeCommandInvoked = false;
            PlayCommand.FirstTrackOnConnection = true;
            PlayCommand.DefaultVolume = 50;
            PauseCommand.PauseCommandInvoked = false;
            PauseButton.PauseCommandInvoked = false;
            MuteCommand.MuteCommandInvoked = false;
            MuteButton.MuteButtonInvoked = false;
            PlayCommand.FirstSongInTrack = true;
            PlayCommand.Queue.Clear();
        }
    }
    
    public static void NodeDisconnected(LavalinkNodeConnection sender, NodeDisconnectedEventArgs e)
    {
        if (e.LavalinkNode.IsConnected == false)
        {
            VolumeCommand.VolumeCommandInvoked = false;
            PlayCommand.FirstTrackOnConnection = true;
            PlayCommand.DefaultVolume = 50;
            PauseCommand.PauseCommandInvoked = false;
            PauseButton.PauseCommandInvoked = false;
            MuteCommand.MuteCommandInvoked = false;
            MuteButton.MuteButtonInvoked = false;
            PlayCommand.FirstSongInTrack = true;
            PlayCommand.Queue.Clear();
        }
    }
}