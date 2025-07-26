using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Events.Buttons.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;

namespace JamJunction.App.Events.Buttons.Player;

public class RepeatButton : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public RepeatButton(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "repeat")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync();

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError(btnInteractionArgs)));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await btnInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoPlayerError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
            var player =
                await lavalinkPlayer.GetPlayerAsync(guildId, UserVoiceChannel, false);

            if (player == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoConnectionError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoAudioTrackError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var guildData = Bot.GuildData[guildId];
            var repeatMode = guildData.RepeatMode;

            guildData.RepeatMode = !guildData.RepeatMode;

            DiscordMessage guildMessage;

            if (repeatMode == false)
            {
                player!.RepeatMode = TrackRepeatMode.None;

                await channel.Channel.DeleteMessageAsync(guildData.Message);

                guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        audioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                guildData.Message = guildMessage;

                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        audioPlayerEmbed.DisableRepeat(btnInteractionArgs)));

                await Task.Delay(10000);

                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            player!.RepeatMode = TrackRepeatMode.Track;

            _ = channel.Channel.DeleteMessageAsync(guildData.Message);

            guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                new DiscordInteractionResponseBuilder(
                    audioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

            guildData.Message = guildMessage;

            var message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    audioPlayerEmbed.EnableRepeat(btnInteractionArgs)));

            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(message.Id);
        }
    }
}