using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embeds;

public class ErrorEmbed
{
    public DiscordEmbedBuilder BuildValidVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildValidVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildSameVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildSameVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83d\udd0a • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoConnectionError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udf0b • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoConnectionError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udf0b • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoPlayerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoPlayerError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoAudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udfb5 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoAudioTrackError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83c\udfb5 • There are no tracks currently playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildAudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Failed to find audio data.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildLiveSteamError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd34 • You can not play a livestream.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildVolumeNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The number for the volume must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoVolumeOver100Error(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  You cannot set the volume above 100.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildMaxVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The volume is already at its maximum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildMinVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd09  •  The volume is already at its minimum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed BuildAlreadyPausedError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23f8  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbed BuildAlreadyPausedError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23f8  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildSeekNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd52  •  The number for the time must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildSeekLargerThanDurationError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83d\udd52  •  The time you are seeking for is larger than the duration of the track.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoTracksToSkipToError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23ed • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoTracksToSkipToError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23ed • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoTracksToShuffleError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u21cc • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildNoTracksToShuffleError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u21cc • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildQueueIsFullError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u2630 • The queue is full. The max capacity is 25 tracks.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildRemoveError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\uddd1\ufe0f • There are no tracks in the queue to remove.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildTrackFailedToLoadError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Track failed to load. Reattempting to connect in 5 seconds.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    public DiscordEmbedBuilder BuildCouldNotLoadTrackOnAttemptError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Attempt to load track again failed. The audio player has been reset.",
            Color = DiscordColor.Red
        };
        return embed;
    }
}