using DSharpPlus.Entities;

namespace JamJunction.App.Views.Modals;

public class PageNumberModal
{
    public DiscordInteractionResponseBuilder Build()
    {
        var model = new DiscordInteractionResponseBuilder()
            .WithTitle("Jump To Page")
            .WithCustomId("jump-to-page")
            .AddComponents(
                new TextInputComponent("Page Number", "page-number"));

        return model;
    }
}