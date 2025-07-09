using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embed_Builders;

public class ErrorEmbed
{
    public DiscordEmbedBuilder ValidVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder ValidVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SameVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SameVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🔊 • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoConnectionError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🌋 • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoConnectionError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🌋 • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoPlayerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"📻 • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoPlayerError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"📻 • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoAudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoAudioTrackError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🎵 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder AudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔴 • Failed to find music data.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder LiveSteamError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔴 • You can not play a livestream.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder VolumeNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The number for the volume must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoVolumeOver100Error(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume above 100.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder MaxVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The volume is already at its maximum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder MinVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔉  •  The volume is already at its minimum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed AlreadyPausedError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed AlreadyPausedError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SeekNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🕒  •  The number for the time must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SeekLargerThanDurationError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                $"🕒  •  The time you are seeking for is larger than the duration of the track.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToSkipToError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏭ • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToSkipToError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏭ • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToShuffleError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⇌ • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToShuffleError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⇌ • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder QueueIsFullError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"☰ • The queue is full. The max capacity is 25 tracks.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder RemoveError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🗑️ • There are no tracks in the queue to remove.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder TrackFailedToLoadError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Track failed to load. Reattempting to connect in 5 seconds.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder CouldNotLoadTrackOnAttemptError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Attempt to load track again failed. The audio player has been reset.",
            Color = DiscordColor.Red
        };
        return embed;
    }
}