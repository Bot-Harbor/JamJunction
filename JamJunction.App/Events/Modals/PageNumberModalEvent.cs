using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Events.Modals.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Modals;

public class PageNumberModalEvent : IModal
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public PageNumberModalEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ModalSubmitEventArgs modalEventArgs)
    {
        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = modalEventArgs.Interaction.Guild.Id;

        var memberId = modalEventArgs.Interaction.User.Id;
        var member = await modalEventArgs.Interaction.Guild.GetMemberAsync(memberId);

        var channel = modalEventArgs.Interaction;

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
        var bot = await modalEventArgs.Interaction.Guild.GetMemberAsync(botId);
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
                    errorEmbed.PlayerInactiveError()));
            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
            return;
        }
        
        var userId = modalEventArgs.Interaction.User.Id;

        var values = modalEventArgs.Values;
        var pageNumber = values["page-number"];

        var userData = Bot.UserData[userId];

        var loadingMessage = await channel.CreateFollowupMessageAsync(
            new DiscordFollowupMessageBuilder(new DiscordMessageBuilder().WithContent("Loading...")));

        await Task.Delay(500);

        await channel.DeleteFollowupMessageAsync(loadingMessage.Id);

        var totalTracks = player.Queue.Count;
        switch (pageNumber)
        {
            case "1":
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player)));
                break;
            case "2" when totalTracks > 15:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "2")));
                break;
            case "3" when totalTracks > 30:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "3")));
                break;
            case "4" when totalTracks > 45:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "4")));
                break;
            case "5" when totalTracks > 60:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "5")));
                break;
            case "6" when totalTracks > 75:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "6")));
                break;
            case "7" when totalTracks > 90:
                userData.CurrentPageNumber = pageNumber;
                await channel.EditFollowupMessageAsync(userData.ViewQueueMessage.Id, new DiscordWebhookBuilder(
                    audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "7")));
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