﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    public static InteractionContext Context { get; set; }
    public static Queue<LavalinkTrack> Queue { get; set; } = new Queue<LavalinkTrack>();
    public static LavalinkTrack CurrentSongData { get; set; } = new LavalinkTrack();
    public static int DefaultVolume { get; set; } = 50;
    public static bool FirstTrackOnConnection { get; set; } = true;
    public static bool FirstSongInTrack { get; set; } = true;

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

        Context = context;

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

                    if (FirstSongInTrack)
                    {
                        CurrentSongData = Queue.Peek();

                        var nextTrackInQueue = Queue.Dequeue();

                        FirstSongInTrack = false;

                        await connection.PlayAsync(track);

                        await context.CreateResponseAsync(
                            new DiscordInteractionResponseBuilder(
                                audioEmbed.SongEmbedBuilder(nextTrackInQueue, context)));
                    }
                    else
                    {
                        await context.CreateResponseAsync(audioEmbed.QueueEmbedBuilder(track));
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
}