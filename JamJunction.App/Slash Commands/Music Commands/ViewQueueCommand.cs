using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using JamJunction.App.Models;
using Lavalink4NET;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class ViewQueueCommand : ApplicationCommandModule
{
    private readonly IAudioService _audioService;

    public ViewQueueCommand(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [SlashCommand("view-queue", "Displays what is currently in the queue.")]
    public async Task ViewQueueCommandAsync(InteractionContext context)
    {
        await context.DeferAsync(true);

        var audioPlayerEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var guildId = context.Guild.Id;
        var userVoiceChannel = context.Member?.VoiceState?.Channel;

        if (userVoiceChannel == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.ValidVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var botId = context.Client.CurrentUser.Id;
        var botVoiceChannel = context.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

        if (botVoiceChannel == false)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoPlayerError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        if (userVoiceChannel.Id != botVoiceState.Channel!.Id)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.SameVoiceChannelError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
        var player =
            await lavalinkPlayer.GetPlayerAsync(guildId, userVoiceChannel, false);

        if (player == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.NoConnectionError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }
        
        if (player!.CurrentTrack == null)
        {
            var errorMessage = await context.FollowUpAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    errorEmbed.PlayerInactiveError()));
            await Task.Delay(10000);
            _ = context.DeleteFollowupAsync(errorMessage.Id);
            return;
        }

        var userId = context.Interaction.User.Id;
        if (!Bot.UserData.ContainsKey(userId)) Bot.UserData.Add(userId, new UserData());
        
        var userData = Bot.UserData[userId];
        userData.GuildId = guildId;
        
        if (userData.ViewQueueMessage != null)
        {
            var previousViewQueueMessage = userData.ViewQueueMessage.Id;
            _ = context.DeleteFollowupAsync(previousViewQueueMessage);
        }
        
        userData.ViewQueueMessage = await context.FollowUpAsync(
            new DiscordFollowupMessageBuilder(audioPlayerEmbed.ViewQueue(context, player)));
    }
}