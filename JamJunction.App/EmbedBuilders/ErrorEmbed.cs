using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace JamJunction.App.EmbedBuilders;

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

    public DiscordEmbedBuilder VoiceChannelErrorEmbedBuilder(InteractionContext context)
    {
        var voiceChannelErrorEmbed = new DiscordEmbedBuilder
        {
            Description = $"🔊 • You must be in a voice channel **{context.Member.Username}**!",
            Color = DiscordColor.Red
        };

        return voiceChannelErrorEmbed;
    }

    public DiscordEmbedBuilder LavaLinkErrorEmbedBuilder()
    {
        var lavaLinkErrorEmbed = new DiscordEmbedBuilder
        {
            Description = "🌋 🔗• Lavalink is not connected!",
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
}