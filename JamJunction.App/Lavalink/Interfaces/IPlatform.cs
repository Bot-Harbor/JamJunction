using DSharpPlus.SlashCommands;
using JamJunction.App.Lavalink.Enums;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Lavalink.Interfaces;

public interface IPlatform
{
    Task PlayTrack(QueuedLavalinkPlayer player, InteractionContext context, string query, bool queueNext = false);
}