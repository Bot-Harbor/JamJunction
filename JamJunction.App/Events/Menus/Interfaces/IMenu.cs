using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Menus.Interfaces;

public interface IMenu
{
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}