using DSharpPlus.SlashCommands;

namespace JamJunction.App.Lavalink.Enums;

public enum Platform
{
    [ChoiceName("Spotify")] Spotify,
    [ChoiceName("YouTube")] YouTube,
    [ChoiceName("Deezer")] Deezer,
    [ChoiceName("SoundCloud")] SoundCloud,
    [ChoiceName("YouTubeMusic")] YouTubeMusic
}