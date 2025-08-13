using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embeds;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using IButton = JamJunction.App.Events.Buttons.Interfaces.IButton;

namespace JamJunction.App.Events.Buttons.Player;

public class VolumeDownButtonEvent : IButton
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public VolumeDownButtonEvent(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs btnInteractionArgs)
    {
        if (btnInteractionArgs.Interaction.Data.CustomId == "volume-down")
        {
            var audioPlayerEmbed = new AudioPlayerEmbed();
            var errorEmbed = new ErrorEmbed();

            var guildId = btnInteractionArgs.Guild.Id;

            var memberId = btnInteractionArgs.User.Id;
            var member = await btnInteractionArgs.Guild.GetMemberAsync(memberId);

            var channel = btnInteractionArgs.Interaction;

            await channel.DeferAsync();

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

            var currentVolume = player.Volume;
            Console.WriteLine($"Current Volume: {currentVolume}");

            if (currentVolume == 0)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.BuildMinVolumeError()));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var decreasedVolume = Math.Max(Math.Round(currentVolume - 0.10, 2), 0);
            await player!.SetVolumeAsync((float)decreasedVolume);

            var guildData = Bot.GuildData[guildId];

            await channel.EditFollowupMessageAsync(guildData.PlayerMessage.Id,
                new DiscordWebhookBuilder(audioPlayerEmbed.TrackInformation(player.CurrentTrack, player)));

            var volumeDecreasedMessage = await channel.CreateFollowupMessageAsync(
                new DiscordFollowupMessageBuilder().AddEmbed(
                    audioPlayerEmbed.VolumeDecreased(btnInteractionArgs)));

            await Task.Delay(10000);
            _ = channel.DeleteFollowupMessageAsync(volumeDecreasedMessage.Id);
        }
    }
}