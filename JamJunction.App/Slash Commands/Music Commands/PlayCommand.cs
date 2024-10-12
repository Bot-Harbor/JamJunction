using System.Collections.Immutable;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Integrations.Lavasearch;
using Lavalink4NET.Integrations.Lavasearch.Extensions;
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
                    errorEmbed.ValidVoiceChannelError(context)));

            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player = await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, connectToVoiceChannel: true);

        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError(context)));

            return;
        }

        var botId = context.Client.CurrentUser.Id;
        context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (userVoiceChannel.Id != botVoiceState!.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));

            return;
        }

        // Maybe move switch to this? 
        if (streamingPlatform == Platform.Spotify)
        {
            var searchResult = await _audioService.Tracks.SearchAsync(
                query: "[...]",
                loadOptions: new TrackLoadOptions(SearchMode: TrackSearchMode.Spotify),
                categories: ImmutableArray.Create(SearchCategory.Track));

            await player!.PlayAsync(searchResult!.Tracks.First()!);
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
                    .AddEmbed(errorEmbed.AudioTrackError(context)));

            return;
        }

        if (track.IsLiveStream)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(errorEmbed.LiveSteamError(context)));
            
            return;
        }
        
        if (!Bot.GuildData.ContainsKey(guildId))
        {
            Bot.GuildData.Add(guildId, new GuildData());
        }
        
        var guildData = Bot.GuildData[guildId];
        var textChannelId = context.Channel.Id;
        guildData.TextChannelId = textChannelId;

        await player.PlayAsync(track!);

        if (player.Queue.IsEmpty)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(audioPlayerEmbed.SongInformation(track, player))));

            return;
        }
        
        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddEmbed(audioPlayerEmbed.SongAddedToQueue(track)));
    }
}