using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embeds;

public class ErrorEmbed
{
    public DiscordEmbedBuilder ValidVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder ValidVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SameVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SameVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83d\udd0a • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoConnectionError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udf0b • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoConnectionError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udf0b • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoPlayerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoPlayerError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoAudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udfb5 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoAudioTrackError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83c\udfb5 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder AudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Failed to find audio data.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder LiveSteamError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd34 • You can not play a livestream.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder VolumeNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The number for the volume must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoVolumeOver100Error(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  You cannot set the volume above 100.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder MaxVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The volume is already at its maximum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder MinVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd09  •  The volume is already at its minimum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed AlreadyPausedError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23f8  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed AlreadyPausedError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23f8  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SeekNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd52  •  The number for the time must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder SeekLargerThanDurationError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83d\udd52  •  The time you are seeking for is larger than the duration of the track.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToSkipToError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23ed • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToSkipToError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23ed • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToShuffleError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u21cc • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder NoTracksToShuffleError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u21cc • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder QueueIsFullError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u2630 • The queue is full. The max capacity is 25 tracks.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder RemoveError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\uddd1\ufe0f • There are no tracks in the queue to remove.",
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