using DSharpPlus.Lavalink;

namespace JamJunction.App;

public class AudioPlayerController
{
    public ulong ChannelId { get; set; }
    public Queue<LavalinkTrack> Queue { get; set; } = new();
    public LavalinkTrack CurrentSongData { get; set; } = new();
    public int Volume { get; set; } = 50;
    public bool FirstSongInTrack { get; set; } = true;
    public bool PauseInvoked { get; set; }
    public bool MuteInvoked { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
}