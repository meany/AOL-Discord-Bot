using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace dm.AOL.Bot.Modules
{
    public class SoundModule : ModuleBase
    {
        private readonly Config config;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public SoundModule(IOptions<Config> config)
        {
            this.config = config.Value;
        }

        public enum SoundName
        {
            drop,
            filedone,
            goodbye,
            gotmail,
            im,
            welcome
        }

        [Command("{S", RunMode = RunMode.Async)]
        [Summary("Plays a sound")]
        public async Task Sound(SoundName name)
        {
            try
            {
                if (Context.Channel is IDMChannel)
                {
                    await Discord.ReplyAsync(Context,
                        message: "Please make this request in one of the official channels.");

                    return;
                }

                if (!config.ChannelIds.Contains(Context.Channel.Id))
                    return;

                using (var a = Context.Channel.EnterTypingState())
                {
                    string soundName = name.ToString().ToUpper();
                    log.Info($"{Context.User} playing {soundName}");

                    // upload sound
                    string fileName = $"{soundName}.wav";
                    string fileLocation = $"Assets/{soundName}.wav";
                    string filePCM = $"Assets/{soundName}.pcm";

                    using (var file = File.Open(fileLocation, FileMode.Open))
                    {
                        await Discord.SendAsync(Context, Context.Channel.Id, file: file, fileName: fileName).ConfigureAwait(false);
                    }

                    // find all voice channels with users and play them there
                    foreach (var vc in await Context.Guild.GetVoiceChannelsAsync())
                    {
                        var users = (vc as SocketVoiceChannel).Users;
                        if (users.Count > 0)
                        {
                            using (var audioClient = await vc.ConnectAsync())
                                await SendAsync(audioClient, filePCM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            using (var output = File.Open(path, FileMode.Open))
            using (var stream = client.CreatePCMStream(AudioApplication.Music, 48000, 100))
            {
                try
                {
                    await client.SetSpeakingAsync(true);
                    await output.CopyToAsync(stream);
                }
                finally
                {
                    await stream.FlushAsync();
                    await client.SetSpeakingAsync(false);
                }
            }
        }
    }
}