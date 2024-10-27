using DSharpPlus.SlashCommands;

namespace JamJunction.App.Lavalink.Enums;

public enum Platform
{
    [ChoiceName("Spotify")] Spotify,
    [ChoiceName("YouTube")] YouTube,
    [ChoiceName("SoundCloud")] SoundCloud,
}