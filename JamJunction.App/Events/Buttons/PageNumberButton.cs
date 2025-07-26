using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Events.Buttons.Interfaces;
using JamJunction.App.Lavalink;
using JamJunction.App.Modals;
using Lavalink4NET;

namespace JamJunction.App.Events.Buttons;

public class PageNumberButton : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public PageNumberButton(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }
    
    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "page-number")
        {
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.BuildValidVoiceChannelError(btnInteractionArgs)));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.BuildValidVoiceChannelError(btnInteractionArgs)));
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
                        errorEmbed.BuildNoPlayerError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.BuildSameVoiceChannelError(btnInteractionArgs)));
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
                        errorEmbed.BuildNoConnectionError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.BuildNoAudioTrackError(btnInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }
            
            // Add condition if page number does not match with any current ones in the modal interaction

            var pageNumberModal = new PageNumberModal();
            await channel.CreateResponseAsync(InteractionResponseType.Modal, pageNumberModal.Build());
        }
    }
}