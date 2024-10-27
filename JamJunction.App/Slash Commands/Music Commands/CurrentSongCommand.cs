using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class CurrentSongCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public CurrentSongCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("current-song", "Shows details about the current song playing.")]
    public async Task CurrentSongCommandAsync(InteractionContext context)
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

        var track = player.CurrentTrack;

        await context
            .FollowUpAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(audioPlayerEmbed.TrackInformation(track, player))));
    }
}