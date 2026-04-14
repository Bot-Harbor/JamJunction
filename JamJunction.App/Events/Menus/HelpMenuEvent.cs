using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Views.Embeds;

namespace JamJunction.App.Events.Menus;

public class HelpMenuEvent : IMenu
{
    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "help-menu")
        {
            var helpEmbed = new HelpEmbed();

            foreach (var value in menuInteractionArgs.Values)
            {
                DiscordEmbedBuilder embed = value switch
                {
                    "main"            => helpEmbed.BuildMain(sender),
                    "all-features"    => helpEmbed.BuildAllFeatures(),
                    "music-commands"  => helpEmbed.BuildMusicCommands(),
                    "other-commands"  => helpEmbed.BuildOtherCommands(),
                    "player-controls" => helpEmbed.BuildPlayerControls(),
                    _                 => helpEmbed.BuildMain(sender)
                };

                await menuInteractionArgs.Interaction.CreateResponseAsync(
                    InteractionResponseType.UpdateMessage,
                    helpEmbed.BuildSection(embed));
            }
        }
    }
}