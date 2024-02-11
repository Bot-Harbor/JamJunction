﻿using DSharpPlus;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using JamJunction.App.Embed_Builders;
using JamJunction.App.Events.Buttons;

namespace JamJunction.App.Slash_Commands.Music_Commands;

public class ResumeCommand : ApplicationCommandModule
{
    [SlashCommand("resume", "Resumes the current song.")]
    public async Task ResumeCommandAsync(InteractionContext context)
    {
        var errorEmbed = new ErrorEmbed();
        var audioEmbed = new AudioPlayerEmbed();
        
        try
        {
            var userVc = context.Member?.VoiceState?.Channel;
            var lava = context.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();

            if (context.Member != null && (context.Member.Permissions & Permissions.ManageChannels) != 0)
            {
                if (!lava.ConnectedNodes!.Any())
                {
                    await context.CreateResponseAsync(errorEmbed.NoConnectionErrorEmbedBuilder());
                }

                if (userVc == null || userVc.Type != ChannelType.Voice)
                {
                    await context.CreateResponseAsync(errorEmbed.ValidVoiceChannelErrorEmbedBuilder(context));
                }

                var connection = node.GetGuildConnection(context.Guild);

                if (connection! == null)
                {
                    await context.CreateResponseAsync(errorEmbed.LavaLinkErrorEmbedBuilder());
                }

                if (connection != null && connection.CurrentState.CurrentTrack == null)
                {
                    await context.CreateResponseAsync(errorEmbed.NoAudioTrackErrorEmbedBuilder());
                }

                if (connection != null)
                {
                    var guildId = context.Guild.Id;
                    var audioPlayerController = Bot.GuildAudioPlayers[guildId];
                    
                    await connection.ResumeAsync();
                    await context.CreateResponseAsync(audioEmbed.ResumeEmbedBuilder(context));
                    
                    audioPlayerController.PauseInvoked = false;
                }
            }
            else
            {
                await context.CreateResponseAsync(errorEmbed.NoResumePermissionEmbedBuilder());
            }
        }
        catch (Exception e)
        {
            await context.CreateResponseAsync(errorEmbed.CommandFailedEmbedBuilder(), ephemeral: true);
        }
    }
}