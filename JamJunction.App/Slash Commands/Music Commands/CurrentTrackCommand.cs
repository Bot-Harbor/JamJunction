using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class CurrentTrackCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public CurrentTrackCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("current-track", "Shows details about the current track playing.")]
    public async Task CurrentTrackCommandAsync(InteractionContext context)
    {
        await context.DeferAsync();

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        var channel = context.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError(context)));
            await Task.Delay(10000);
            await context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoPlayerError(context)));
            await Task.Delay(10000);
            await channel.DeleteMessageAsync(errorMessage);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError(context)));
            await Task.Delay(10000);
            await channel.DeleteMessageAsync(errorMessage);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
           var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError(context)));
            await Task.Delay(10000);
            await channel.DeleteMessageAsync(errorMessage);
            return;
        }

        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoAudioTrackError(context)));
            await Task.Delay(10000);
            await channel.DeleteMessageAsync(errorMessage);
            return;
        }

        var track = player.CurrentTrack;

        var guildData = Bot.GuildData[guildId];
        
        channel = context.Channel;
        await channel.DeleteMessageAsync(guildData.Message);
        
        var message = await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(audioPlayerEmbed.TrackInformation(track, player))));
        guildData.Message = message;
    }
}