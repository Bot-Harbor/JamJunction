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
            Description = $"🔊 • You must be in a valid voice channel ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder ValidVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a valid voice channel ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder SameVoiceChannelError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in the same voice channel as the bot ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder SameVoiceChannelError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in the same voice channel as the bot ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoConnectionError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🌋 • Lavalink connection is not established ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoConnectionError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🌋 • Lavalink connection is not established ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder NoPlayerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • There is no player in the voice channel ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoPlayerError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • There is no player in the voice channel ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoAudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"\ud83c\udfb5 • There are no tracks currently playing ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoAudioTrackError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"\ud83c\udfb5 • There are no tracks currently playing ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder AudioTrackError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"\ud83c\udfb5 • Failed to find song ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder LiveSteamError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"\ud83d\udd34 • You can not play a livestream ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder VolumeNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The number for the volume must be a whole number ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoVolumeOver100Error(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume above 100 ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder MaxVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The volume is already at its maximum ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder MinVolumeError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The volume is already at its minimum ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbed AlreadyPausedError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  •  The player is already paused ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbed AlreadyPausedError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"⏸  •  The player is already paused ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder SeekNotAnIntegerError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The number for the time must be a whole number ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder SeekLargerThanDurationError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The time you are seeking for is larger than the duration of the song ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoSongsToSkipToError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip to ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoSongsToSkipToError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip to ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoSongsToShuffleError(InteractionContext context)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔀 • There are no songs in the queue to shuffle ``{context.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }
    
    public DiscordEmbedBuilder NoSongsToShuffleError(ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"🔀 • There are no songs in the queue to shuffle ``{btnInteractionArgs.User.Username}``.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder TrackFailedToLoadError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Track failed to load. Reattempting to connect in 5 seconds.",
            Color = DiscordColor.Red
        };

        return embed;
    }

    public DiscordEmbedBuilder CouldNotLoadTrackOnAttemptError()
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Attempt to load track again failed. The audio player has been reset.",
            Color = DiscordColor.Red
        };

        return embed;
    }
}