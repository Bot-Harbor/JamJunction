﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Menus;

public class RemoveMenu : IMenu
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public RemoveMenu(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "remove")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = menuInteractionArgs.Guild.Id;

            var memberId = menuInteractionArgs.User.Id;
            var member = await menuInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = menuInteractionArgs.Interaction;

            await channel.DeferAsync();

            try
            {
                UserVoiceChannel = member.VoiceState.Channel;

                if (UserVoiceChannel == null)
                {
                    var errorMessage = await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError(menuInteractionArgs)));
                    await Task.Delay(10000);
                    _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                    return;
                }
            }
            catch (Exception)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError(menuInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await menuInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoPlayerError(menuInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError(menuInteractionArgs)));
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
                        errorEmbed.NoConnectionError(menuInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            foreach (var value in menuInteractionArgs.Values)
            {
                await player.Queue.RemoveAtAsync(Convert.ToInt32(value));
                _ = channel.DeleteFollowupMessageAsync(menuInteractionArgs.Message.Id);

                var guildData = Bot.GuildData[guildId];
                _ = channel.Channel.DeleteMessageAsync(guildData.Message);

                var guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                    new DiscordInteractionResponseBuilder(
                        audioPlayerEmbed.TrackInformation(player.CurrentTrack, player))));

                guildData.Message = guildMessage;

                var message = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(audioPlayerEmbed.Remove(menuInteractionArgs, player)));

                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(message.Id);
                break;
            }
        }
    }
}