using Discord;
using Discord.Audio;
using Discord.Commands;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dm.AOL.Bot.Modules
{
    public class GagModule : ModuleBase
    {
        private readonly Config config;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public GagModule(IOptions<Config> config)
        {
            this.config = config.Value;
        }

        [Command("=qpermagag", RunMode = RunMode.Async)]
        [Summary("Gag a user for specified minutes. Only available for HOSTs.")]
        [Remarks("You must mention the @user, and the valid range for gagging is 1 to 1440 minutes.")]
        public async Task Gag(IUser user, int minutes)
        {
            try
            {
                if (minutes < 1 || minutes > 1440)
                    return;

                var adminRoles = Context.Guild.Roles.Where(x => config.AdminRoleIds.Contains(x.Id));
                var gagger = await Context.Guild.GetUserAsync(Context.User.Id).ConfigureAwait(false);

                // check if gagger is an admin
                if (!adminRoles.Any(x => gagger.RoleIds.Contains(x.Id)))
                    return;

                var gagRole = Context.Guild.Roles.Single(x => x.Id == config.GagRoleId);
                var guildUser = await Context.Guild.GetUserAsync(user.Id).ConfigureAwait(false);

                // lets not gag ourselves
                if (guildUser.Id == Context.User.Id)
                    return;

                // check if user is already gagged
                if (guildUser.RoleIds.Contains(gagRole.Id))
                {
                    await Discord.ReplyAsync(Context,
                        message: $"{guildUser.Mention} is already gagged!")
                        .ConfigureAwait(false);
                    return;
                }

                // gag em
                log.Info($"{Context.User} is gagging {guildUser} for {minutes} minutes");
                await guildUser.AddRoleAsync(gagRole).ConfigureAwait(false);
                var emote = await new Emotes(Context).Get(config.EmoteGagged);
                await Context.Message.AddReactionAsync(emote).ConfigureAwait(false);

                // disconnect em in VC
                var vc = guildUser.VoiceChannel;
                if (vc != null)
                {
                    string filePCM = $"Assets/GOODBYE.pcm";
                    using (var audioClient = await vc.ConnectAsync())
                        await SendAsync(audioClient, filePCM).ConfigureAwait(false);

                    await guildUser.ModifyAsync(x => x.Mute = true).ConfigureAwait(false);
                }

                // wait & remove
                await Task.Delay(minutes * 60000);
                await guildUser.RemoveRoleAsync(gagRole).ConfigureAwait(false);
                await guildUser.ModifyAsync(x => x.Mute = false).ConfigureAwait(false);
                log.Info($"{guildUser} has been ungagged");
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