using DSharpPlus.Entities;

namespace JamJunction.App.Views.Embeds;

/// <summary>
/// Provides embed messages used to display error responses
/// related to audio playback, queue management, and user input.
/// </summary>
/// <remarks>
/// These embeds are returned when an invalid action occurs,
/// such as attempting to use commands outside a voice channel,
/// interacting with an inactive player, or providing invalid values.
/// </remarks>
public class ErrorEmbed
{
    /// <summary>
    /// Builds an embed message indicating that the user must be connected
    /// to a valid voice channel to use the command.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the voice channel validation error.
    /// </returns>
    public DiscordEmbedBuilder ValidVoiceChannelError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in a valid voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the user must be in the same
    /// voice channel as the bot to execute the command.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the voice channel mismatch error.
    /// </returns>
    public DiscordEmbedBuilder SameVoiceChannelError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a • You must be in the same voice channel as the bot.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the Lavalink connection
    /// has not been established.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the Lavalink connection error.
    /// </returns>
    public DiscordEmbedBuilder NoConnectionError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udf0b • Lavalink connection is not established.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that no audio player exists
    /// in the current voice channel.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the missing player error.
    /// </returns>
    public DiscordEmbedBuilder NoPlayerError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • There is no player in the voice channel.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the audio player is not
    /// currently active and no track is playing.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the inactive player error.
    /// </returns>
    public DiscordEmbedBuilder PlayerInactiveError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcfb • The player is not active currently. Please queue something.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that audio data could not be found
    /// for the requested track.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the audio lookup error.
    /// </returns>
    public DiscordEmbedBuilder AudioTrackError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Failed to find audio data.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that livestream content cannot be played.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the livestream playback error.
    /// </returns>
    public DiscordEmbedBuilder LiveSteamError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd34 • You can not play a livestream.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume value must be a whole number.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the invalid volume input error.
    /// </returns>
    public DiscordEmbedBuilder VolumeNotAnIntegerError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The number for the volume must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume cannot be set above 100.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the maximum volume limit error.
    /// </returns>
    public DiscordEmbedBuilder NoVolumeOver100Error()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  You cannot set the volume above 100.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume is already at its maximum level.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the maximum volume state error.
    /// </returns>
    public DiscordEmbedBuilder MaxVolumeError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd0a  •  The volume is already at its maximum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the volume is already at its minimum level.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the minimum volume state error.
    /// </returns>
    public DiscordEmbedBuilder MinVolumeError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd09  •  The volume is already at its minimum.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the audio player is already paused.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbed"/> representing the already-paused error.
    /// </returns>
    public DiscordEmbed AlreadyPausedError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23f8  •  The player is already paused.",
            Color = DiscordColor.Red
        };
        return embed;
    }
    
    /// <summary>
    /// Builds an embed message indicating that the audio player is already playing.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbed"/> representing the already-playing error.
    /// </returns>
    public DiscordEmbed AlreadyPlayingError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "▶  •  The player is already playing.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the seek time must be a whole number.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the invalid seek input error.
    /// </returns>
    public DiscordEmbedBuilder SeekNotAnIntegerError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udd52  •  The number for the time must be a whole number.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the requested seek time
    /// exceeds the total duration of the track.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the seek duration error.
    /// </returns>
    public DiscordEmbedBuilder SeekLargerThanDurationError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description =
                "\ud83d\udd52  •  The time you are seeking for is larger than the duration of the track.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that there are no tracks
    /// available in the queue to skip to.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the skip-to error.
    /// </returns>
    public DiscordEmbedBuilder NoTracksToSkipToError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u23ed • There are no tracks to skip to.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that there are no tracks
    /// available in the queue to shuffle.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the shuffle error.
    /// </returns>
    public DiscordEmbedBuilder NoTracksToShuffleError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u21cc • There are no tracks in the queue to shuffle.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the queue has reached its
    /// maximum capacity.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the queue full error.
    /// </returns>
    public DiscordEmbedBuilder QueueIsFullError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u2630 • The queue is full. The max capacity is 100 tracks.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that there are no tracks
    /// available in the queue to remove.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the remove error.
    /// </returns>
    public DiscordEmbedBuilder RemoveError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\uddd1\ufe0f • There are no tracks in the queue to remove.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that a track failed to load
    /// and that the bot will attempt to reconnect.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the track load failure error.
    /// </returns>
    public DiscordEmbedBuilder TrackFailedToLoadError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Track failed to load. Reattempting to connect in 5 seconds.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the second attempt to load
    /// a track failed and the audio player has been reset.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the failed retry error.
    /// </returns>
    public DiscordEmbedBuilder CouldNotLoadTrackOnAttemptError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "❌ • Attempt to load track again failed. The audio player has been reset.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the requested queue page
    /// does not exist.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the invalid page number error.
    /// </returns>
    public DiscordEmbedBuilder PageNumberDoesNotExistError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcc4 • Page number does not exist.",
            Color = DiscordColor.Red
        };
        return embed;
    }

    /// <summary>
    /// Builds an embed message indicating that the requested track
    /// does not exist in the queue.
    /// </summary>
    /// <returns>
    /// A <see cref="DiscordEmbedBuilder"/> representing the invalid track error.
    /// </returns>
    public DiscordEmbedBuilder TrackDoesNotExistError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\ud83d\udcc4 • Track does not exist in the queue.",
            Color = DiscordColor.Red
        };
        return embed;
    }
}