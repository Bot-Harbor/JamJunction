﻿using DSharpPlus.SlashCommands;
using PingEmbed = JamJunction.App.Embed_Builders.PingEmbed;

namespace JamJunction.App.Slash_Commands.Other_Commands;

public class PingCommand : ApplicationCommandModule
{
    [SlashCommand("ping", "Will pong back to the server.")]
    public async Task PingAsync(InteractionContext context)
    {
        var pingEmbed = new PingEmbed();
        await context.CreateResponseAsync(pingEmbed.Ping(context), true);
    }
}