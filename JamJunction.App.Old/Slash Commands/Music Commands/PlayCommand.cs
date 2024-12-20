﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
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

            if (!lava.ConnectedNodes!.Any())
                await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());

            if (userVc == null || userVc.Type != ChannelType.Voice)
                await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));

            await node.ConnectAsync(userVc);

            var connection = node.GetGuildConnection(context.Guild);

            if (connection! == null) await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());

            var loadResult = await node.Rest.GetTracksAsync(query);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
                await context.CreateResponseAsync(errorEmbed.AudioTrackErrorEmbedBuilder());

            var track = loadResult.Tracks.First();

            if (connection != null)
            {
                var guildId = context.Guild.Id;
                var audioPlayerController = Bot.GuildAudioPlayers[guildId];

                audioPlayerController.Queue.Enqueue(track);

                if (audioPlayerController.FirstSongInTrack)
                {
                    audioPlayerController.CurrentSongData = audioPlayerController.Queue.Peek();
                    audioPlayerController.Queue.Dequeue();
                    audioPlayerController.ChannelId = context.Channel.Id;
                    audioPlayerController.FirstSongInTrack = false;

                    audioPlayerController.CancellationTokenSource.Cancel();
                    audioPlayerController.CancellationTokenSource.Dispose();

                    await connection.PlayAsync(track);

                    await connection.SetVolumeAsync(audioPlayerController.Volume);

                    await context.CreateResponseAsync(
                        new DiscordInteractionResponseBuilder(
                            audioEmbed.SongEmbedBuilder(context)));
                }
                else
                {
                    await context.CreateResponseAsync(audioEmbed.QueueEmbedBuilder(track));
                }
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), true);
        }
    }
}