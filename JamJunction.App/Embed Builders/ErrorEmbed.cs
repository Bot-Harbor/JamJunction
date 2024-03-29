﻿using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.Embed_Builders;

public class ErrorEmbed
{
    public DiscordEmbedBuilder CommandFailedEmbedBuilder()
    {
        var commandErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "⚠️ • Command failed to execute!",
            Color = DiscordColor.Red
        };

        return commandErrorEmbed;
    }

    public DiscordEmbedBuilder ValidVoiceChannelErrorEmbedBuilder(InteractionContext context)
    {
        var voiceChannelErrorEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a valid voice channel ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return voiceChannelErrorEmbed;
    }

    public DiscordEmbedBuilder ValidVoiceChannelBtnErrorEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var voiceChannelErrorEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a valid voice channel ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return voiceChannelErrorEmbed;
    }

    public DiscordEmbedBuilder NoConnectionErrorEmbedBuilder()
    {
        var noConnectionErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "🌋 • Lavalink connection is not established!",
            Color = DiscordColor.Red
        };

        return noConnectionErrorEmbed;
    }

    public DiscordEmbedBuilder LavaLinkErrorEmbedBuilder()
    {
        var lavaLinkErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "🔊 • There is no player in the server!",
            Color = DiscordColor.Red
        };

        return lavaLinkErrorEmbed;
    }

    public DiscordEmbedBuilder AudioTrackErrorEmbedBuilder()
    {
        var audioTrackErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udfb5 • Failed to find song!",
            Color = DiscordColor.Red
        };

        return audioTrackErrorEmbed;
    }

    public DiscordEmbedBuilder NoAudioTrackErrorEmbedBuilder()
    {
        var audioTrackErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "\ud83c\udfb5 • There are no tracks currently playing!",
            Color = DiscordColor.Red
        };

        return audioTrackErrorEmbed;
    }

    public DiscordEmbedBuilder NoPlayPermissionEmbedBuilder()
    {
        var noPlayPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to play a song!",
            Color = DiscordColor.Red
        };

        return noPlayPermissionEmbed;
    }

    public DiscordEmbedBuilder NoPausePermissionEmbedBuilder()
    {
        var noPausePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to pause a song!",
            Color = DiscordColor.Red
        };

        return noPausePermissionEmbed;
    }

    public DiscordEmbedBuilder NoResumePermissionEmbedBuilder()
    {
        var noResumePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to resume a song!",
            Color = DiscordColor.Red
        };

        return noResumePermissionEmbed;
    }

    public DiscordEmbedBuilder NoStopPermissionEmbedBuilder()
    {
        var noStopPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to stop a song!",
            Color = DiscordColor.Red
        };

        return noStopPermissionEmbed;
    }

    public DiscordEmbedBuilder NoVolumePermissionEmbedBuilder()
    {
        var noVolumePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to change the volume!",
            Color = DiscordColor.Red
        };

        return noVolumePermissionEmbed;
    }

    public DiscordEmbedBuilder MaxVolumeEmbedBuilder(InteractionContext context)
    {
        var maxVolumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume above 100 ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return maxVolumeEmbed;
    }

    public DiscordEmbedBuilder MaxVolumeEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var maxVolumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The volume is already at its maximum ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return maxVolumeEmbed;
    }

    public DiscordEmbedBuilder MinVolumeEmbedBuilder(InteractionContext context)
    {
        var minVolumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume below 0 ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return minVolumeEmbed;
    }

    public DiscordEmbedBuilder MinVolumeEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var minVolumeEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  The volume is already at its minimum ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return minVolumeEmbed;
    }

    public DiscordEmbedBuilder NoVolumeWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noVolumeWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noVolumeWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoVolumeWhilePausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noVolumeWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊  •  You cannot set the volume while the player is paused ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noVolumeWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoSeekPermissionEmbedBuilder()
    {
        var noSeekPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to change the position of a song!",
            Color = DiscordColor.Red
        };

        return noSeekPermissionEmbed;
    }

    public DiscordEmbedBuilder NoSeekWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noSeekWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description =
                $"⌛  •  You cannot change the song position while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noSeekWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoMuteWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noMuteWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot mute the song while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noMuteWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoMuteWhilePausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noMuteWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot mute the song while the player is paused ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noMuteWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoUnMuteWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noUnMuteWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot unmute the song while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noUnMuteWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoUnMuteWhilePausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noUnMuteWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot unmute the song while the player is paused ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noUnMuteWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoRestartPermissionEmbedBuilder()
    {
        var noRestartPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to restart a song!",
            Color = DiscordColor.Red
        };

        return noRestartPermissionEmbed;
    }

    public DiscordEmbedBuilder NoRestartWithPausedEmbedBuilder(InteractionContext context)
    {
        var noRestartWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot restart the song while player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noRestartWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoRestartWithPausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noRestartWhilePausedEmbed = new DiscordEmbedBuilder
        {
            Description = $"⌛  •  You cannot restart the song while player is paused ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noRestartWhilePausedEmbed;
    }

    public DiscordEmbedBuilder NoLeavePermissionEmbedBuilder()
    {
        var noLeavePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to disconnect the player!",
            Color = DiscordColor.Red
        };

        return noLeavePermissionEmbed;
    }

    public DiscordEmbedBuilder NoShufflePermissionEmbedBuilder()
    {
        var noShufflePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to shuffle the queue!",
            Color = DiscordColor.Red
        };

        return noShufflePermissionEmbed;
    }


    public DiscordEmbedBuilder NoSkipPermissionEmbedBuilder()
    {
        var noSkipPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • You do not have permission to skip a song!",
            Color = DiscordColor.Red
        };

        return noSkipPermissionEmbed;
    }

    public DiscordEmbedBuilder NoSongsToSkipEmbedBuilder(InteractionContext context)
    {
        var noSongsToSkipEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip to ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noSongsToSkipEmbed;
    }

    public DiscordEmbedBuilder NoSongsToSkipEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noSongsToSkipEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip to ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noSongsToSkipEmbed;
    }

    public DiscordEmbedBuilder QueueIsEmptyEmbedBuilder(InteractionContext context)
    {
        var queueIsEmptyEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎶 • There are no songs in the queue to shuffle ``{context.User.Username}``!",
            Color = DiscordColor.Red
        };

        return queueIsEmptyEmbed;
    }

    public DiscordEmbedBuilder QueueIsEmptyEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var queueIsEmptyEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎶 • There are no songs in the queue to shuffle ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return queueIsEmptyEmbed;
    }

    public DiscordEmbedBuilder TrackFailedToLoadEmbedBuilder()
    {
        var trackFailedToLoadEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Track failed to load! Reattempting to connect in 5 seconds.",
            Color = DiscordColor.Red
        };

        return trackFailedToLoadEmbed;
    }

    public DiscordEmbedBuilder CouldNotLoadTrackOnAttemptEmbedBuilder()
    {
        var couldNotLoadOnAttemptTrackEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Attempt to load track again failed! The audio player has been reset.",
            Color = DiscordColor.Red
        };

        return couldNotLoadOnAttemptTrackEmbed;
    }

    public DiscordEmbedBuilder TrackStuckEmbedBuilder()
    {
        var trackStuckEmbed = new DiscordEmbedBuilder
        {
            Description = "\u274c • Track was stuck! The audio player has restarted the song.",
            Color = DiscordColor.Red
        };

        return trackStuckEmbed;
    }
}