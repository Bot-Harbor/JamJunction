using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class SkipCommand : ApplicationCommandModule
{
    [SlashCommand("skip", "Skips to the next song in the queue.")]
    public async Task SkipCommandAsync(InteractionContext context)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();
        
        try
        {
            var userVc = context.Member?.VoiceState?.Channel;
            var lava = context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (context.Member != null && (context.Member.Permissions & Permissions.ManageChannels) != 0)
            {
                if (!lava.ConnectedNodes!.Any())
                {
                    await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());
                }

                if (userVc == null || userVc.Type != ChannelType.Voice)
                {
                    await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));
                }

                var connection = node.GetGuildConnection(context.Guild);

                if (connection! == null)
                {
                    await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
                }

                if (connection != null && connection.CurrentState.CurrentTrack == null)
                {
                    await context.CreateResponseAsync(errorEmbed.NoAudioTrackErrorEmbedBuilder());
                }

                if (connection != null)
                {
                    var guildId = context.Guild.Id;
                    var audioPlayerController = Bot.GuildAudioPlayers[guildId];
                    
                    if (audioPlayerController.Queue.Count != 0)
                    {
                        var queue = audioPlayerController.Queue;

                        audioPlayerController.CurrentSongData = queue.Peek();

                        var nextTrackInQueue = audioPlayerController.Queue.Dequeue();

                        await connection.PlayAsync(nextTrackInQueue);
                        
                        await context.CreateResponseAsync(
                            new DiscordInteractionResponseBuilder(
                                audioEmbed.SongEmbedBuilder(context)));
                    }
                    else
                    {
                        await context.CreateResponseAsync(errorEmbed.NoSongsToSkipEmbedBuilder(context));
                    }
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoSkipPermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}