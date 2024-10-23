using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class SeekCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public SeekCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("seek", "Sets the position of the song.")]
    public async Task SeekCommandAsync(InteractionContext context,
        [Option("time", "Change the current position of the song in seconds.")]
        double time)
    {
         await context.DeferAsync();

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError(context)));

            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoPlayerError(context)));

            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));

            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, connectToVoiceChannel: false);
        
        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError(context)));

            return;
        }
       
        if (player!.CurrentTrack == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoAudioTrackError(context)));

            return;
        }

        var isInt = time == (int) time;

        if (!isInt)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SeekNotAnIntegerError(context)));
            
            return;
        }
        
        var duration = Math.Round(player.CurrentTrack.Duration.TotalSeconds);
        
        if (time > duration)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SeekLargerThanDurationError(context)));
            
            return;
        }
        
        await player.SeekAsync(TimeSpan.FromSeconds(time));
        
        await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.Seek(context, time)));
    }
}