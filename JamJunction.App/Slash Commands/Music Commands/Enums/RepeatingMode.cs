using DSharpPlus.SlashCommands;

namespace JamJunction.App.Slash_Commands.Music_Commands.Enums;

public enum RepeatingMode
{
    [ChoiceName("None")] None,
    [ChoiceName("Repeat Track")] RepeatTrack,
    [ChoiceName("Repeat Queue")] RepeatQueue
}