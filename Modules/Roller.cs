using Discord;
using Discord.Commands;
using NLog;
using System;
using System.Threading.Tasks;

namespace dm.AOL.Bot.Modules
{
    public class Roller
    {
        private readonly Config config;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public Roller(Config config)
        {
            this.config = config;
        }

        public async Task Roll(CommandContext ctx, uint dice, uint sides)
        {
            try
            {
                if (ctx.Channel is IDMChannel)
                {
                    await Discord.ReplyAsync(ctx,
                        message: "Please make this request in one of the official channels.")
                        .ConfigureAwait(false);
                    return;
                }

                if (!config.ChannelIds.Contains(ctx.Channel.Id))
                    return;

                using (var a = ctx.Channel.EnterTypingState())
                {
                    log.Info($"{ctx.User} rolling dice");

                    string s = string.Empty;
                    var rnd = new Random();
                    for (int i = 0; i < dice; i++)
                    {
                        s += $" {rnd.Next(1, (int)sides)}";
                    }

                    await Discord.ReplyAsync(ctx,
                        message: $"{ctx.User.Mention} rolled {dice} {sides}-sided dice:{s}")
                        .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
    }
}