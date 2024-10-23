using DSharpPlus.SlashCommands;

namespace JamJunction.App.Lavalink.Enums;

public enum Platform
{
    [ChoiceName("YouTube")] YouTube,
    [ChoiceName("Spotify")] Spotify,
    [ChoiceName("SoundCloud")] SoundCloud,
}