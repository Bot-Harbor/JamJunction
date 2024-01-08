using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Interfaces;
using JamJunction.App.Slash_Commands.Music_Commands;

namespace JamJunction.App.Events.Buttons;

public class StopButton : IButton
{
    public static bool StopCommandInvoked { get; set; }

    public async Task Execute(DiscordClient sender, ComponentInteractionCreateEventArgs e)
    {
        var audioEmbed = new AudioPlayerEmbed();
        var errorEmbed = new ErrorEmbed();

        var message = e.Interaction;

        try
        {
            if (e.Interaction.Data.CustomId == "stop")
            {
                var member = await e.Guild.GetMemberAsync(e.User.Id);
                var userVc = member?.VoiceState?.Channel;
                var lava = sender.GetLavalink();
                var node = lava.ConnectedNodes.Values.First();
                
                if (member != null && (e.Channel.PermissionsFor(member) & Permissions.ManageChannels) != 0)
                {
                    if (!lava.ConnectedNodes!.Any())
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoConnectionErrorEmbedBuilder()));
                    }
                    
                    if (userVc == null || userVc.Type != ChannelType.Voice)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.ValidVoiceChannelBtnErrorEmbedBuilder(e)));
                    }

                    var connection = node.GetGuildConnection(e.Guild);

                    if (connection == null)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.LavaLinkErrorEmbedBuilder()));
                    }

                    if (connection != null && connection.CurrentState.CurrentTrack == null)
                    {
                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(
                                errorEmbed.NoAudioTrackErrorEmbedBuilder()));
                    }

                    if (connection != null)
                    {
                        await connection.StopAsync();

                        await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().AddEmbed(audioEmbed.StopEmbedBuilder(e))
                                .AddEmbed(audioEmbed.QueueSomethingEmbedBuilder()));

                        StopCommandInvoked = true;

                        PlayCommand.FirstTrack = true;

                        if (connection.IsConnected)
                        {
                            await Task.Delay((TimeSpan.FromMinutes(1)));
                            await message.EditOriginalResponseAsync(
                                new DiscordWebhookBuilder().AddEmbed(audioEmbed.StopEmbedBuilder(e)));
                            await connection.DisconnectAsync();
                        }
                        if (!connection.IsConnected)
                        {
                            await message.EditOriginalResponseAsync(
                                new DiscordWebhookBuilder().AddEmbed(audioEmbed.StopEmbedBuilder(e)));
                        }
                    }
                }
                else
                {
                    await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().AddEmbed(errorEmbed.NoStopPermissionEmbedBuilder()));
                }
            }
        }
        catch (Exception exception)
        {
            await message.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AsEphemeral().AddEmbed(errorEmbed.CommandFailedEmbedBuilder()));
        }
    }
}