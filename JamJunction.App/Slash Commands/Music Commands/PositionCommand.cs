using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class PositionCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public PositionCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }
    
    [SlashCommand("position", "Gets the current song position.")]
    public async Task PositionAsync(InteractionContext context)
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
                    errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context)));

            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoPlayerErrorEmbedBuilder()));

            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelErrorEmbedBuilder(context)));

            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(context, guildId, userVoiceChannel, connectToVoiceChannel: false);

        if (player == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionErrorEmbedBuilder()));

            return;
        }

        if (player!.CurrentTrack == null)
        {
            await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoAudioTrackErrorEmbedBuilder()));

            return;
        }
        
        var position = player.Position!.Value.Position;
        
        await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder().AddEmbed(
                audioPlayerEmbed.SongPositionBuilder(position)));
    }
}