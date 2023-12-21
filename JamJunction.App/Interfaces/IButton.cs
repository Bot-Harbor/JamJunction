using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Interfaces;

public interface IButton
{
    Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e);
}