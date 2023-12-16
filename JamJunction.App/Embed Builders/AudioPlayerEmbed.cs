using DSharpPlus.Entities;

namespace JamJunction.App.Embed_Builders;

public class AudioPlayerEmbed
{
    public DiscordEmbedBuilder PauseEmbedBuilder()
    {
        var pauseEmbed = new DiscordEmbedBuilder()
        {
            Description = "🎵 • Track paused!",
            Color = DiscordColor.Yellow
        };

        return pauseEmbed;
    }
    
    public DiscordEmbedBuilder VolumeEmbedBuilder(int volume)
    {
        var volumeEmbed = new DiscordEmbedBuilder()
        {
            Description = $"🎵 • Volume changed to {volume}!",
            Color = DiscordColor.Blue
        };

        return volumeEmbed;
    }
}