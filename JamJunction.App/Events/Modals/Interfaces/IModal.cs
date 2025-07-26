using DSharpPlus;
using DSharpPlus.EventArgs;

namespace JamJunction.App.Events.Modals.Interfaces;

public interface IModal
{
    Task Execute(DiscordClient sender, ModalSubmitEventArgs modalEventArgs);
}