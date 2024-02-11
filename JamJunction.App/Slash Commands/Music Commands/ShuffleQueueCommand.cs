using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class ShuffleQueueCommand : ApplicationCommandModule
{
    [SlashCommand("shuffle", "Shuffles the queue.")]
    public async Task ShuffleQueueCommandAsync(InteractionContext context)
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

                if (connection != null)
                {
                    var guildId = context.Guild.Id;
                    var audioPlayerController = Bot.GuildAudioPlayers[guildId];
                    
                    if (audioPlayerController.Queue.Count != 0)
                    {
                        ShuffleQueue(audioPlayerController.Queue);

                        await context.CreateResponseAsync(audioEmbed.ShuffleQueueBuilder(context));
                    }
                    else
                    {
                        await context.CreateResponseAsync(errorEmbed.QueueIsEmptyEmbedBuilder(context));
                    }   
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoShufflePermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }

    protected void ShuffleQueue<T>(Queue<T> queue)
    {
        var list = new List<T>(queue);
        var random = new Random();

        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        queue.Clear();
        foreach (var item in list)
        {
            queue.Enqueue(item);
        }
    }
}