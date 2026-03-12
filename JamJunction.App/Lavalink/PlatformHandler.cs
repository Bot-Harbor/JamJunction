using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Enums;
using JamJunction.App.Lavalink.Interfaces;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Lavalink;

public class PlatformHandler
{
    public void Excute(IPlatform platform, QueuedLavalinkPlayer player, InteractionContext context, string query,
        bool queueNext = false)
    {
        platform.PlayTrack(player, context, query, queueNext);
    }
}