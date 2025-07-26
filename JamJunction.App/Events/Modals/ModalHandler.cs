using DSharpPlus;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Modals.Interfaces;

namespace JamJunction.App.Events.Modals;

public class ModalHandler
{
    public Task Execute(IModal modal, DiscordClient sender, ModalSubmitEventArgs modalEventArgs)
    {
        return modal.Execute(sender, modalEventArgs);
    }
}