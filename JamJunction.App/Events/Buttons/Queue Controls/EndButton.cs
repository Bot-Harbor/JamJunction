using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Events.Buttons.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Buttons.Queue_Controls;

public class EndButton : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public EndButton(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "end")
        {
            var errorEmbed = new ErrorEmbed();
            var audioPlayerEmbed = new AudioPlayerEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync(true);

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError()));
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
                        errorEmbed.NoPlayerError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError()));
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
                        errorEmbed.NoConnectionError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoAudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var userId = btnInteractionArgs.Interaction.User.Id;
            var userData = Bot.UserData[userId];
            
            var loadingMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent("Loading...")));
            
            await Task.Delay(500);
            
            await channel.DeleteFollowupMessageAsync(loadingMessage.Id);

            var totalTracks = player.Queue.Count;
            switch (totalTracks)
            {
                case < 31:
                    userData.CurrentPageNumber = "2";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "2")));
                    break;
                case < 46:
                    userData.CurrentPageNumber = "3";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "3")));
                    break;
                case < 61:
                    userData.CurrentPageNumber = "4";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "4")));
                    break;
                case < 76:
                    userData.CurrentPageNumber = "5";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "5")));
                    break;
                case < 91:
                    userData.CurrentPageNumber = "6";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "6")));
                    break;
                case < 101:
                    userData.CurrentPageNumber = "7";
                    await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id,
                        new DiscordWebhookBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                            pageNumber: "7")));
                    break;
                default:
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.PageNumberDoesNotExistError()));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    break;
                }
            }
        }
    }
}