using DSharpPlus.Entities;
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
            Description = "🌋 🔗• Lavalink connection is not established!",
            Color = DiscordColor.Red
        };

        return noConnectionErrorEmbed;
    }

    public DiscordEmbedBuilder LavaLinkErrorEmbedBuilder()
    {
        var lavaLinkErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "🌋 🔗• Lavalink failed to connect!",
            Color = DiscordColor.Red
        };

        return lavaLinkErrorEmbed;
    }

    public DiscordEmbedBuilder AudioTrackErrorEmbedBuilder()
    {
        var audioTrackErrorEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • Failed to find song!",
            Color = DiscordColor.Red
        };

        return audioTrackErrorEmbed;
    }

    public DiscordEmbedBuilder NoAudioTrackErrorEmbedBuilder()
    {
        var audioTrackErrorEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no tracks currently playing!",
            Color = DiscordColor.Red
        };

        return audioTrackErrorEmbed;
    }

    public DiscordEmbedBuilder NoPlayPermissionEmbedBuilder()
    {
        var noPlayPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to play a song!",
            Color = DiscordColor.Red
        };

        return noPlayPermissionEmbed;
    }

    public DiscordEmbedBuilder NoPausePermissionEmbedBuilder()
    {
        var noPausePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to pause a song!",
            Color = DiscordColor.Red
        };

        return noPausePermissionEmbed;
    }

    public DiscordEmbedBuilder NoResumePermissionEmbedBuilder()
    {
        var noResumePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to resume a song!",
            Color = DiscordColor.Red
        };

        return noResumePermissionEmbed;
    }

    public DiscordEmbedBuilder NoStopPermissionEmbedBuilder()
    {
        var noStopPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to stop a song!",
            Color = DiscordColor.Red
        };

        return noStopPermissionEmbed;
    }

    public DiscordEmbedBuilder NoVolumePermissionEmbedBuilder()
    {
        var noVolumePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to change the volume!",
            Color = DiscordColor.Red
        };

        return noVolumePermissionEmbed;
    }

    public DiscordEmbedBuilder MaxVolumeEmbedBuilder(InteractionContext context)
    {
        var maxVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  You cannot set the volume above 100 ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return maxVolumeEmbed;
    }
    
    public DiscordEmbedBuilder MaxVolumeEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var maxVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  The volume is already at its maximum ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return maxVolumeEmbed;
    }

    public DiscordEmbedBuilder MinVolumeEmbedBuilder(InteractionContext context)
    {
        var minVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  You cannot set the volume below 0 ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return minVolumeEmbed;
    }
    
    public DiscordEmbedBuilder MinVolumeEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var minVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  The volume is already at its minimum ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return minVolumeEmbed;
    }

    public DiscordEmbedBuilder NoVolumeWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noVolumeWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  You cannot set the volume while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noVolumeWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoSeekPermissionEmbedBuilder()
    {
        var noSeekPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to change the position of a song!",
            Color = DiscordColor.Red
        };

        return noSeekPermissionEmbed;
    }
    
    public DiscordEmbedBuilder NoSeekWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noSeekWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"⌛  •  You cannot change the song position while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noSeekWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoMuteWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noMuteWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"⌛  •  You cannot mute the song while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noMuteWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoMuteWhilePausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noMuteWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"⌛  •  You cannot mute the song while the player is paused ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noMuteWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoUnMuteWhilePausedEmbedBuilder(InteractionContext context)
    {
        var noUnMuteWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"⌛  •  You cannot unmute the song while the player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noUnMuteWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoUnMuteWhilePausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noUnMuteWhilePausedEmbed = new DiscordEmbedBuilder()
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
            Description = $"❌ • You do not have permission to restart a song!",
            Color = DiscordColor.Red
        };

        return noRestartPermissionEmbed;
    }
    
    public DiscordEmbedBuilder NoRestartWithPausedEmbedBuilder(InteractionContext context)
    {
        var noRestartWhilePausedEmbed = new DiscordEmbedBuilder()
        {
            Description = $"⌛  •  You cannot restart the song while player is paused ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noRestartWhilePausedEmbed;
    }
    
    public DiscordEmbedBuilder NoRestartWithPausedEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noRestartWhilePausedEmbed = new DiscordEmbedBuilder()
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
            Description = $"❌ • You do not have permission to disconnect the player!",
            Color = DiscordColor.Red
        };

        return noLeavePermissionEmbed;
    }
    
    public DiscordEmbedBuilder NoShufflePermissionEmbedBuilder()
    {
        var noShufflePermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to shuffle the queue!",
            Color = DiscordColor.Red
        };

        return noShufflePermissionEmbed;
    }

    
    public DiscordEmbedBuilder NoSkipPermissionEmbedBuilder()
    {
        var noSkipPermissionEmbed = new DiscordEmbedBuilder
        {
            Description = $"❌ • You do not have permission to skip a song!",
            Color = DiscordColor.Red
        };

        return noSkipPermissionEmbed;
    }
    
    public DiscordEmbedBuilder NoSongsToSkipEmbedBuilder(InteractionContext context)
    {
        var noSongsToSkipEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip ``{context.Member.Username}``!",
            Color = DiscordColor.Red
        };

        return noSongsToSkipEmbed;
    }
    
    public DiscordEmbedBuilder NoSongsToSkipEmbedBuilder(ComponentInteractionCreateEventArgs e)
    {
        var noSongsToSkipEmbed = new DiscordEmbedBuilder
        {
            Description = $"🎵 • There are no songs to skip ``{e.User.Username}``!",
            Color = DiscordColor.Red
        };

        return noSongsToSkipEmbed;
    }
}