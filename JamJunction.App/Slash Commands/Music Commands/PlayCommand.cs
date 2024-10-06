using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Rest.Entities.Tracks;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public PlayCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("play", "Queue a song.")]
    public async Task PlayAsync
    (
        InteractionContext context,
        [Option("Song", "Enter the name or url of the song you want to queue.")]
        string query,
        [Option("Platform", "Pick a streaming platform.")]
        Platform streamingPlatform = default
    )
    {
        await context.DeferAsync();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        if (userVoiceChannel == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context)));
            
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player = await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, connectToVoiceChannel: true);

        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionErrorEmbedBuilder()));
            
            return;
        }
        
        var botId = context.Client.CurrentUser.Id;
        context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);
        
        if (userVoiceChannel.Id != botVoiceState!.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelErrorEmbedBuilder(context)));
            
            return;
        }

        var track = streamingPlatform switch
        {
            Platform.YouTube => await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTube),
            Platform.YouTubeMusic => await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTubeMusic),
            Platform.SoundCloud => await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.SoundCloud),
            _ => await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.SoundCloud)
        };

        if (track == null)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(errorEmbed.AudioTrackErrorEmbedBuilder()));
            
            return;
        }
        
        if (!Bot.GuildData.ContainsKey(guildId))
        {
            Bot.GuildData.Add(guildId, new GuildData());
        }

        var guildData = Bot.GuildData[guildId];
        var textChannelId = context.Channel.Id;
        guildData.TextChannelId = textChannelId;

        await player!.PlayAsync(track!);
        
        if (player.Queue.IsEmpty)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(audioPlayerEmbed.SongEmbedBuilder(track, player))));
        }
        else
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(audioPlayerEmbed.QueueEmbedBuilder(track)));
        }
    }
}