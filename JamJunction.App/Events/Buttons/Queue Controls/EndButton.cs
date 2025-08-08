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

            var guildData = Bot.GuildData[guildId];
            var previousViewQueueMessage = guildData.ViewQueueMessage;

            _ = channel.Channel.DeleteMessageAsync(previousViewQueueMessage);

            var userId = btnInteractionArgs.Interaction.User.Id;
            var userData = Bot.UserData[userId];

            var totalTracks = player.Queue.Count;
            if (totalTracks < 31)
            {
                userData.CurrentPageNumber = "2";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "2")));
            }
            else if (totalTracks < 46)
            {
                userData.CurrentPageNumber = "3";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "3")));
            }
            else if (totalTracks < 61)
            {
                userData.CurrentPageNumber = "4";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "4")));
            }
            else if (totalTracks < 76)
            {
                userData.CurrentPageNumber = "5";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "5")));
            }
            else if (totalTracks < 91)
            {
                userData.CurrentPageNumber = "6";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "6")));
            }
            else if (totalTracks < 101)
            {
                userData.CurrentPageNumber = "7";
                guildData.ViewQueueMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(btnInteractionArgs, player,
                        pageNumber: "7")));
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