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
                        errorEmbed.BuildValidVoiceChannelError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }
        }
        catch (Exception)
        {
            var errorMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildValidVoiceChannelError()));
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
                    errorEmbed.BuildNoPlayerError()));
            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
            return;
        }

        UserVoiceChannel = member.VoiceState.Channel;

        if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.BuildSameVoiceChannelError()));
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
                    errorEmbed.BuildNoConnectionError()));
            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
            return;
        }

        var values = modalEventArgs.Values;
        var pageNumber = values["page-number"];

        var guildData = Bot.GuildData[guildId];
        _ = channel.Channel.DeleteMessageAsync(guildData.ViewQueueMessage);

        DiscordMessage message = null;

        switch (pageNumber)
        {
            case "1":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player)));
                break;
            case "2":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "2")));
                break;
            case "3":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "3")));
                break;
            case "4":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "4")));
                break;
            case "5":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "5")));
                break;
            case "6":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "6")));
                break;
            case "7":
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player,
                        backBtnIsDisabled: false, pageNumber: "7")));
                break;
            default:
                message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.PageNumberDoesNotExistError()));
                break;
        }

        guildData.ViewQueueMessage = message;
    }
}