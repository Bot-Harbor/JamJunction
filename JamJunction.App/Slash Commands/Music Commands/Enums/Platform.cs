using DSharpPlus.SlashCommands;

namespace JamJunction.App.Slash_Commands.Music_Commands.Enums;

public enum Platform
{
    [ChoiceName("YouTube")] YouTube,
    [ChoiceName("Spotify")] Spotify,
    [ChoiceName("SoundCloud")] SoundCloud,
}