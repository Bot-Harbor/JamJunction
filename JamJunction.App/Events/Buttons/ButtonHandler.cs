using DSharpPlus;
using DSharpPlus.EventArgs;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons;

public class ButtonHandler
{
    public Task Execute(IButton button, DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        return button.Execute(sender, btnInteractionArgs);
    }
}