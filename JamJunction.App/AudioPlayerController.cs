using DSharpPlus.Lavalink;
using JamJunction.App.Events.Buttons;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App;

public class AudioPlayerController
{
    public ulong ChannelId { get; set; }
    public Queue<LavalinkTrack> Queue { get; set; } = new Queue<LavalinkTrack>();
    public LavalinkTrack CurrentSongData { get; set; } = new LavalinkTrack();
    public int Volume { get; set; } = 50;
    public bool FirstSongInTrack { get; set; } = true;
    public bool PauseInvoked { get; set; }
    public bool MuteInvoked { get; set; }
}