using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using JamJunction.App.Interfaces;

namespace JamJunction.App.Events.Buttons;

public class ButtonHandler
{
    /// <summary>
    /// Execute Button Event
    /// </summary>
    /// <param name="button"></param>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Task Execute(IButton button, DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        return button.Execute(sender, e);
    }
}