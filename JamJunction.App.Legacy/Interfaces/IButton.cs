using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Legacy.Interfaces;

public interface IButton
{
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}