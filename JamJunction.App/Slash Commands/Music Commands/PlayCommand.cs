using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Slash_Commands.Music_Commands.Enums;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using LavalinkTrack = Lavalink4NET.Tracks.LavalinkTrack;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PlayCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;
    private LavalinkTrack Track { get; set; }
    private AudioPlayerEmbed AudioPlayerEmbed { get; set; } = new();
    private ErrorEmbed ErrorEmbed { get; set; } = new();

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
        
        var player = await GetPlayerAsync(context, connectToVoiceChannel: true);

        switch (streamingPlatform)
        {
            case Platform.YouTube:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.YouTube);
                break;
            case Platform.YouTubeMusic:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.YouTubeMusic);
                break;
            case Platform.SoundCloud:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.SoundCloud);
                break;
            default:
                Track = await _audioService.Tracks
                    .LoadTrackAsync(query, TrackSearchMode.SoundCloud);
                break;
        }

        if (Track is null)
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(ErrorEmbed.AudioTrackErrorEmbedBuilder()));
        }

        var guildId = context.Guild.Id;
        var textChannelId = context.Channel.Id;
        
        // Check to see if it already exists
        if (!Bot.GuildData.ContainsKey(guildId))
        {
            Bot.GuildData.Add(guildId, new GuildData());
        }
        
        var guildController = Bot.GuildData[guildId];
        guildController.TextChannelId = textChannelId;
        
        await player!.PlayAsync(Track!);
        
        if (guildController.FirstSongInQueue)
        {
            guildController.FirstSongInQueue = false;
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(AudioPlayerEmbed.SongEmbedBuilder(Track, player))));
        }
        else
        {
            await context
                .FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .AddEmbed(AudioPlayerEmbed.QueueEmbedBuilder(Track)));
        }
    }
    
    private async ValueTask<QueuedLavalinkPlayer> GetPlayerAsync(InteractionContext interactionContext,
        bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var playerOptions = new QueuedLavalinkPlayerOptions {HistoryCapacity = 10000};

        var result = await _audioService.Players
            .RetrieveAsync(interactionContext.Guild.Id, interactionContext.Member?.VoiceState.Channel!.Id,
                playerFactory: PlayerFactory.Queued, Options.Create(playerOptions), retrieveOptions);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status;

            if (errorMessage == PlayerRetrieveStatus.UserNotInVoiceChannel)
            {
                // Not sending
                Console.WriteLine("User Not In Voice Channel");
                await interactionContext.FollowUpAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        ErrorEmbed.ValidVoiceChannelErrorEmbedBuilder(interactionContext)));
            }
            else if (errorMessage == PlayerRetrieveStatus.BotNotConnected)
            {
                // Not sending
                Console.WriteLine("Bot Not Connected");
                await interactionContext.FollowUpAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        ErrorEmbed.LavaLinkErrorEmbedBuilder()));
            }
            else
            {
                // Not sending
                Console.WriteLine("Unknown Error");
                await interactionContext.FollowUpAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(ErrorEmbed.UnknownErrorEmbedBuilder()));
            }
        }

        return result.Player;
    }
}