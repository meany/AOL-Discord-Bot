using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using dm.AOL.Bot;
using NLog;

namespace dm.AOL.Bot
{
    public class Args
    {
        private readonly DiscordSocketClient discordClient;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public Args(DiscordSocketClient discordClient)
        {
            this.discordClient = discordClient;
        }
    }
}