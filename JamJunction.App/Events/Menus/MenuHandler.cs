using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Menus.Interfaces;

namespace JamJunction.App.Events.Menus;

public class MenuHandler
{
    public Task Execute(IMenu menu, DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        return menu.Execute(sender, btnInteractionArgs);
    }
}