using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Events.Buttons.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Buttons.Queue_Controls;

public class BackButton : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public BackButton(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "back")
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
            var bot = await btnInteractionArgs.Guild.GetMemberAsync(botId);
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

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.BuildNoAudioTrackError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var userId = btnInteractionArgs.Interaction.User.Id;
            var userData = Bot.UserData[userId];
            
            var previousViewQueueMessage = userData.ViewQueueMessage;
            _ = channel.Channel.DeleteMessageAsync(previousViewQueueMessage);
            
            var currentPageNumber = userData.CurrentPageNumber;
            if (currentPageNumber == "2")
            {
                userData.CurrentPageNumber = "1";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "1")));
            }
            else if (currentPageNumber == "3")
            {
                userData.CurrentPageNumber = "2";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "2")));
            }
            else if (currentPageNumber == "4")
            {
                userData.CurrentPageNumber = "3";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "3")));
            }
            else if (currentPageNumber == "5")
            {
                userData.CurrentPageNumber = "4";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "4")));
            }
            else if (currentPageNumber == "6")
            {
                userData.CurrentPageNumber = "5";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "5")));
            }
            else if (currentPageNumber == "7")
            {
                userData.CurrentPageNumber = "6";
                userData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "6")));
            }
            else
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(errorEmbed.PageNumberDoesNotExistError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
            }
        }
    }
}