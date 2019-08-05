using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dm.AOL.Bot.Modules
{
    public class RollModule : ModuleBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        [Command("//roll")]
        [Summary("Rolls dice")]
        public async Task Roll(int dice, int sides)
        {
            // just here for the help
        }
    }
}