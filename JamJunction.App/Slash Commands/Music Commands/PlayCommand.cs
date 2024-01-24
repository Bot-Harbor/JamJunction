using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    public static Queue<LavalinkTrack> Queue { get; set; } = new Queue<LavalinkTrack>();
    public static int DefaultVolume { get; set; } = 50;
    public static bool FirstTrackOnConnection { get; set; } = true;

    [SlashCommand("play", "Queue a song.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Song", "Enter the name of the song you want to queue.")]
        string query
    )
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

                await node.ConnectAsync(userVc);

                var connection = node.GetGuildConnection(context.Guild);

                if (connection! == null)
                {
                    await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
                }

                var loadResult = await node.Rest.GetTracksAsync(query);

                if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    await context.CreateResponseAsync(errorEmbed.AudioTrackErrorEmbedBuilder());
                }

                var track = loadResult.Tracks.First();

                if (connection != null)
                {
                    Queue.Enqueue(track);

                    if (FirstTrackOnConnection)
                    {
                        await connection.SetVolumeAsync(DefaultVolume);
                        FirstTrackOnConnection = false;
                    }

                    if (Queue.Count <= 0)
                    {
                        await context.CreateResponseAsync(
                            new DiscordInteractionResponseBuilder(audioEmbed.CurrentSongEmbedBuilder(track, context)));
                        Console.WriteLine(Queue.Count);
                    }
                    else
                    {
                        await context.CreateResponseAsync(audioEmbed.QueueEmbedBuilder(track));
                        Console.WriteLine(Queue.Count);
                    }

                    if (connection.CurrentState.CurrentTrack == null)
                    {
                        await PlayNextTrack(connection, context, track);
                    }
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoPlayPermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }

    private static async Task PlayNextTrack(LavalinkGuildConnection connection, InteractionContext context,
        LavalinkTrack track)
    {
        var audioEmbed = new AudioPlayerEmbed();

        if (Queue.Count > 0)
        {
            var nextTrack = Queue.Dequeue();
            await connection.PlayAsync(nextTrack);

            // If seek, pause, or restart is used, update task length accordingly
            await Task.Delay(nextTrack.Length);

            // If Queue is empty, display ambient mode
            if (Queue.Count != 0)
            {
                await PlayNextTrack(connection, context, track);
                await context.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder(audioEmbed.CurrentSongEmbedBuilder(track, context)));
                Console.WriteLine("Current Song");
            }
            else
            {
                await context.CreateResponseAsync(audioEmbed.QueueSomethingEmbedBuilder());
                Console.WriteLine("Queue Something");
            }
        }
    }
}