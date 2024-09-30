using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Buttons.Interfaces;

public interface IButton
{
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}