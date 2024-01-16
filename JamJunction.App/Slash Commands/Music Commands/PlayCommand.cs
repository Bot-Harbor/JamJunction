using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
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
                    await connection.PlayAsync(track);

                    if (FirstTrackOnConnection)
                    {
                        await connection.SetVolumeAsync(DefaultVolume);
                        FirstTrackOnConnection = false;
                    }

                    await context.CreateResponseAsync(
                        new DiscordInteractionResponseBuilder(audioEmbed.CurrentSongEmbedBuilder(track, context)));

                    await Task.Delay(connection!.CurrentState.CurrentTrack.Length);

                    await context.DeleteResponseAsync();

                    if (connection.CurrentState.CurrentTrack == null)
                    {
                        if (connection.IsConnected)
                        {
                            if (!StopCommand.StopCommandInvoked && !StopButton.StopCommandInvoked)
                            {
                                var queueSomethingMessage =
                                    await context.Channel.SendMessageAsync(audioEmbed.QueueSomethingEmbedBuilder());

                                await Task.Delay(TimeSpan.FromMinutes(1));
                                await queueSomethingMessage.DeleteAsync("End of ambient mode.");

                                await connection.DisconnectAsync();

                                StopCommand.StopCommandInvoked = false;
                                StopButton.StopCommandInvoked = false;
                                PauseCommand.PauseCommandInvoked = false;
                                PauseButton.PauseCommandInvoked = false;
                                MuteCommand.MuteCommandInvoked = false;
                                MuteButton.MuteButtonInvoked = false;
                                VolumeCommand.VolumeCommandInvoked = false;
                                FirstTrackOnConnection = true;
                            }
                        }
                    }
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoPlayPermissionEmbedBuilder());
            }

            StopCommand.StopCommandInvoked = false;
            StopButton.StopCommandInvoked = false;
            PauseCommand.PauseCommandInvoked = false;
            PauseButton.PauseCommandInvoked = false;
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}