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
        [Summary("Rolls dice. Exact syntax is //roll-dice#-sides#.")]
        [Remarks("For example, if you wanted to roll dice like in the Monopoly game you would say: `//roll-dice2-sides6`\n" +
            "Dice range: 2 to 15\n" +
            "Sides range: 2 to 999")]
        public async Task Roll(int dice, int sides)
        {
            // just here for the help
        }
    }
}