using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Interfaces;

public interface IButton
{
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}