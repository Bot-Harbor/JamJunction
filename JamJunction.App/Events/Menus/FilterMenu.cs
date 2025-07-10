using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Menus.Interfaces;
using JamJunction.App.Lavalink;
using Lavalink4NET;
using Lavalink4NET.Filters;

namespace JamJunction.App.Events.Menus;

public class FilterMenu : IMenu
{
    private readonly IAudioService _audioService;
    private readonly DiscordClient _discordClient;

    public FilterMenu(IAudioService audioService, DiscordClient discordClient)
    {
        _audioService = audioService;
        _discordClient = discordClient;
    }

    private DiscordChannel UserVoiceChannel { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs menuInteractionArgs)
    {
        if (menuInteractionArgs.Interaction.Data.CustomId == "filters-menu")
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

            if (player!.CurrentTrack == null)
            {
                var errorMessage = await channel.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder().AddEmbed(
                        errorEmbed.NoAudioTrackError(menuInteractionArgs)));
                await Task.Delay(10000);
                _ = channel.DeleteFollowupMessageAsync(errorMessage.Id);
                return;
            }

            var guildData = Bot.GuildData[guildId];
            _ = channel.Channel.DeleteMessageAsync(guildData.Message);

            DiscordMessage guildMessage;

            foreach (var value in menuInteractionArgs.Values)
                switch (value)
                {
                    case "reset":
                        player.Filters.Clear();
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    case "nightcore":
                    {
                        player.Filters.Clear();
                        var nightcore = new TimescaleFilterOptions
                        {
                            Speed = 1.25f,
                            Pitch = 1.2f,
                            Rate = 1.0f
                        };

                        player.Filters.Timescale = nightcore;
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    }
                    case "8d":
                    {
                        player.Filters.Clear();
                        var eightDFilter = new RotationFilterOptions
                        {
                            Frequency = 0.2f
                        };

                        player.Filters.Rotation = eightDFilter;
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    }
                    case "vapor-wave":
                    {
                        player.Filters.Clear();
                        var vaporwaveFilter = new TimescaleFilterOptions
                        {
                            Speed = 0.8f,
                            Pitch = 0.85f,
                            Rate = 1.0f
                        };

                        player.Filters.Timescale = vaporwaveFilter;
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    }
                    case "karaoke":
                    {
                        player.Filters.Clear();
                        var karaokeFilter = new KaraokeFilterOptions
                        {
                            Level = 0.2f,
                            MonoLevel = 0.1f,
                            FilterBand = 220.0f,
                            FilterWidth = 100.0f
                        };

                        player.Filters.Karaoke = karaokeFilter;
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    }
                    case "slow-motion":
                    {
                        player.Filters.Clear();
                        var slowMotionFilter = new TimescaleFilterOptions
                        {
                            Speed = 0.5f
                        };

                        player.Filters.Timescale = slowMotionFilter;
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                    }
                    default:
                        player.Filters.Clear();
                        await player!.Filters.CommitAsync();

                        guildMessage = await channel.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder(
                            new DiscordInteractionResponseBuilder(
                                audioPlayerEmbed.TrackInformation(player.CurrentTrack, player, true))));
                        guildData.Message = guildMessage;
                        break;
                }
        }
    }
}