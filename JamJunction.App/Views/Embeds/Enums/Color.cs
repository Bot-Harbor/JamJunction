using DSharpPlus.SlashCommands;

namespace JamJunction.App.Views.Embeds.Enums;

/// <summary>
/// Represents the available color choices that can be selected when
/// creating caption embeds. Each value maps to a corresponding
/// <see cref="DiscordColor"/> used when building embeds.
/// </summary>
/// <remarks>
/// The <see cref="ChoiceNameAttribute"/> values are used to display
/// user-friendly names in Discord slash command options.
/// </remarks>
public enum Color
{
    /// <summary>
    /// Represents the Aquamarine embed color.
    /// </summary>
    [ChoiceName("Aquamarine")] Aquamarine,

    /// <summary>
    /// Represents the Azure embed color.
    /// </summary>
    [ChoiceName("Azure")] Azure,

    /// <summary>
    /// Represents the Black embed color.
    /// </summary>
    [ChoiceName("Black")] Black,

    /// <summary>
    /// Represents the Blue embed color.
    /// </summary>
    [ChoiceName("Blue")] Blue,

    /// <summary>
    /// Represents the Discord "Blurple" themed embed color.
    /// </summary>
    [ChoiceName("Bplurple")] Bplurple,

    /// <summary>
    /// Represents the Brown embed color.
    /// </summary>
    [ChoiceName("Brown")] Brown,

    /// <summary>
    /// Represents the Chartreuse embed color.
    /// </summary>
    [ChoiceName("Chartreuse")] Chartreuse,

    /// <summary>
    /// Represents the Cyan embed color.
    /// </summary>
    [ChoiceName("Cyan")] Cyan,

    /// <summary>
    /// Represents the Dark Blue embed color.
    /// </summary>
    [ChoiceName("Dark Blue")] DarkBlue,

    /// <summary>
    /// Represents the Dark Gray embed color.
    /// </summary>
    [ChoiceName("Dark Gray")] DarkGray,

    /// <summary>
    /// Represents the Dark Green embed color.
    /// </summary>
    [ChoiceName("Dark Green")] DarkGreen,

    /// <summary>
    /// Represents the Dark Red embed color.
    /// </summary>
    [ChoiceName("Dark Red")] DarkRed,

    /// <summary>
    /// Represents the Gold embed color.
    /// </summary>
    [ChoiceName("Gold")] Gold,

    /// <summary>
    /// Represents the Goldenrod embed color.
    /// </summary>
    [ChoiceName("Golden Rod")] Goldenrod,

    /// <summary>
    /// Represents the Gray embed color.
    /// </summary>
    [ChoiceName("Gray")] Gray,

    /// <summary>
    /// Represents the Green embed color.
    /// </summary>
    [ChoiceName("Green")] Green,

    /// <summary>
    /// Represents the Light Gray embed color.
    /// </summary>
    [ChoiceName("Light Gray")] LightGray,

    /// <summary>
    /// Represents the Orange embed color.
    /// </summary>
    [ChoiceName("Orange")] Orange,

    /// <summary>
    /// Represents the Pink embed color.
    /// </summary>
    [ChoiceName("Pink")] Pink,

    /// <summary>
    /// Represents the Purple embed color.
    /// </summary>
    [ChoiceName("Purple")] Purple,

    /// <summary>
    /// Represents the Red embed color.
    /// </summary>
    [ChoiceName("Red")] Red,

    /// <summary>
    /// Represents the Sap Green embed color.
    /// </summary>
    [ChoiceName("Sap Green")] SapGreen,

    /// <summary>
    /// Represents the Teal embed color.
    /// </summary>
    [ChoiceName("Teal")] Teal,

    /// <summary>
    /// Represents the White embed color.
    /// </summary>
    [ChoiceName("White")] White,

    /// <summary>
    /// Represents the Yellow embed color.
    /// </summary>
    [ChoiceName("Yellow")] Yellow
}