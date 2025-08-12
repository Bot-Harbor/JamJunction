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

        var userId = modalEventArgs.Interaction.User.Id;
        
        var values = modalEventArgs.Values;
        var pageNumber = values["page-number"];
        
        var userData = Bot.UserData[userId];
        
        var previousViewQueueMessage = userData.ViewQueueMessage;
        _ = channel.Channel.DeleteMessageAsync(previousViewQueueMessage);

        DiscordMessage message = null;

        var totalTracks = player.Queue.Count;
        
        if (pageNumber == "1")
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player)));
        }
        else if (pageNumber == "2" && totalTracks > 15)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "2")));
        }
        else if (pageNumber == "3" && totalTracks > 30)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "3")));
        }
        else if (pageNumber == "4" && totalTracks > 45)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "4")));
        }
        else if (pageNumber == "5" && totalTracks > 60)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "5")));
        }
        else if (pageNumber == "6" && totalTracks > 75)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "6")));
        }
        else if (pageNumber == "7" && totalTracks > 90)
        {
            userData.CurrentPageNumber = pageNumber;
            message = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(modalEventArgs, player, pageNumber: "7")));
        }
        else
        {
            var errorMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.PageNumberDoesNotExistError()));
            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
        }
        
        userData.ViewQueueMessage = message;
    }
}