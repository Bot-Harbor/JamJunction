using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordEmbedBuilder PlayEmbedBuilder(LavalinkTrack track)
    {
        var audioTrackEmbed = new DiscordEmbedBuilder()
        {
            Description = $"💿  •  **Now playing**: {track.Title}\n" +
                          $"🎙️  •  **Artist**: {track.Author}\n" +
                          $"🔗  •  **Link:** {track.Uri.AbsoluteUri}",
            Color = DiscordColor.Teal,
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() // Add track photo
            {
                Width = 135,
                Height = 71,
                Url = "https://haulixdaily.com/wp-content/uploads/2018/" +
                      "08/tumblr_inline_pe4i0bR0o21s24py6_540.png"
            }
        };

        return audioTrackEmbed;
    }

    public DiscordEmbedBuilder PauseEmbedBuilder(InteractionContext context)
    {
        var pauseEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟡  • ``{context.Member.Username}`` paused the track!",
            Color = DiscordColor.Yellow
        };

        return pauseEmbed;
    }

    public DiscordEmbedBuilder ResumeEmbedBuilder(InteractionContext context)
    {
        var resumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🟢  • ``{context.Member.Username}`` resumed the track!",
            Color = DiscordColor.Green
        };

        return resumeEmbed;
    }

    public DiscordEmbedBuilder StopEmbedBuilder(InteractionContext context)
    {
        var stopEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔴   • ``{context.Member.Username}`` stopped the player!",
            Color = DiscordColor.Red
        };

        return stopEmbed;
    }

    public DiscordEmbedBuilder VolumeEmbedBuilder(int volume, InteractionContext context)
    {
        var volumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  ``{context.Member.Username}`` changed the volume to ``{volume}``!",
            Color = DiscordColor.Teal
        };

        return volumeEmbed;
    }

    public DiscordEmbedBuilder MaxVolumeEmbedBuilder(InteractionContext context)
    {
        var maxVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  You cannot set the volume above 200 ``{context.Member.Username}!``",
            Color = DiscordColor.Yellow
        };

        return maxVolumeEmbed;
    }

    public DiscordEmbedBuilder MinVolumeEmbedBuilder(InteractionContext context)
    {
        var minVolumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🔊  •  You cannot set the volume below 0 ``{context.Member.Username}!``",
            Color = DiscordColor.Teal
        };

        return minVolumeEmbed;
    }

    public DiscordEmbedBuilder SeekEmbedBuilder(InteractionContext context, double time)
    {
        var seekEmbed = new DiscordEmbedBuilder()
        {
            Description =
                $"⌛   • ``{context.Member.Username}`` changed the song position to ``{time}`` seconds!",
            Color = DiscordColor.Teal
        };

        return seekEmbed;
    }
}