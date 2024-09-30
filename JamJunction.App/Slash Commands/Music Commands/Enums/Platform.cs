using DSharpPlus.SlashCommands;
using Lavalink4NET.Rest.Entities.Tracks;

namespace JamJunction.App.Slash_Commands.Music_Commands.Enums;

public enum Platform
{
    [ChoiceName("YouTube")] YouTube,
    [ChoiceName("YouTube Music")] YouTubeMusic,
    [ChoiceName("SoundCloud")] SoundCloud,
}