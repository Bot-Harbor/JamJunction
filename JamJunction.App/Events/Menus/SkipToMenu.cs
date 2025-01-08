using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;

namespace JamJunction.App.Events.Menus;

public class SkipToMenu : IMenu
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public SkipToMenu(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "queue-menu")
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
                    await channel.CreateFollowupMessageAsync(
                        new DiscordFollowupMessageBuilder().AddEmbed(
                            errorEmbed.ValidVoiceChannelError(menuInteractionArgs)));
                    return;
                }
            }
            catch (Exception)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.ValidVoiceChannelError(menuInteractionArgs)));
                return;
            }

            var botId = _discordClient.CurrentUser.Id;
            var bot = await menuInteractionArgs.Guild.GetMemberAsync(botId);
            var botVoiceChannel = bot.Guild.VoiceStates.TryGetValue(botId, out var botVoiceState);

            if (botVoiceChannel == false)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoPlayerError(menuInteractionArgs)));
                return;
            }

            UserVoiceChannel = member.VoiceState.Channel;

            if (UserVoiceChannel!.Id != botVoiceState.Channel!.Id)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.SameVoiceChannelError(menuInteractionArgs)));
                return;
            }

            var lavalinkPlayer = new LavalinkPlayerHandler(_audioService);
            var player =
                await lavalinkPlayer.GetPlayerAsync(guildId, UserVoiceChannel, false);

            if (player == null)
            {
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoConnectionError(menuInteractionArgs)));
                return;
            }
            
            foreach (var value in menuInteractionArgs.Values)
            {
                await player.SkipAsync(Convert.ToInt32(value));
                await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(audioPlayerEmbed.SkipTo(menuInteractionArgs, player)));
                break;
            }
        }
    }
}