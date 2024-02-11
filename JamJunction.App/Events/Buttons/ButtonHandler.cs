using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Interfaces;

namespace JamJunction.App.Events.Buttons;

public class ButtonHandler
{
    public static Task Execute(IButton button, DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        return button.Execute(sender, e);
    }
}