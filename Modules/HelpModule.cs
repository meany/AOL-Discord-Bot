using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using static dm.AOL.Bot.Modules.SoundModule;

namespace dm.AOL.Bot.Modules
{
    public class HelpModule : ModuleBase
    {
        private readonly Config config;
        private readonly CommandService commands;

        public HelpModule(IOptions<Config> config, CommandService commands)
        {
            this.config = config.Value;
            this.commands = commands;
        }

        [Command("?help")]
        [Summary("List of commands and information for the bot")]
        [Remarks("Simply using 'help' will show all commands and command aliases.\n" +
            "Aliases are available to make sending commands faster.\n" +
            "To review command usage and remarks, use the 'help' command along with the full 'name' of the command (not an alias).\n")]
        public async Task Help(string command = "")
        {
            var output = new EmbedBuilder()
                .WithColor(Color.AOL);
            if (command == string.Empty)
            {
                foreach (var cmd in commands.Commands)
                {
                    AddHelp(cmd, ref output);
                    output.WithAuthor(author =>
                    {
                        author.WithName($"AOL Bot v{Common.Util.GetVersion()}");
                    }).WithFooter(footer =>
                    {
                        footer.WithText($"Use '?help <command>' to get help with a specifc command")
                            .WithIconUrl(Asset.INFO);
                    });
                }
            }
            else
            {
                var cmd = commands.Commands.FirstOrDefault(m => m.Name.ToLower() == command.ToLower());
                if (cmd == null)
                {
                    return;
                }

                string cmdName = cmd.Aliases.FirstOrDefault();
                cmdName = (cmdName == "{s") ? "{S" : cmdName;

                output.AddField($"Command: **{cmdName}**",
                    $"{GetParams(cmd)}\n" +
                    $"**Summary**: {cmd.Summary}\n" +
                    $"**Remarks**: {cmd.Remarks}" +
                    $"{GetAliases(cmd)}");
            }

            await Discord.ReplyDMAsync(Context, output, deleteUserMessage: true).ConfigureAwait(false);
        }

        private void AddHelp(CommandInfo cmd, ref EmbedBuilder output)
        {
            string cmdName = cmd.Aliases.FirstOrDefault();
            cmdName = (cmdName == "{s") ? "{S" : cmdName;

            output.AddField($"— {cmdName} {GetParams(cmd, false)}",
                $"{cmd.Summary}" +
                $"{GetAliases(cmd)}");
        }

        private string GetAliases(CommandInfo cmd)
        {
            string s = string.Empty;
            var aliases = cmd.Aliases.Where(x => x != cmd.Name.ToLower());
            if (aliases.Any())
            {
                string aliasJoin = string.Join("|", aliases.Select(x => $"`{x}`"));
                s = $"\n**Aliases:** {aliasJoin}";
            }
            return s;
        }

        private string GetParams(CommandInfo cmd, bool withIntro = true)
        {
            string s = string.Empty;
            if (cmd.Parameters.Any())
            {
                s += (withIntro) ? "\n**Parameters**: " : string.Empty;
                foreach (var param in cmd.Parameters)
                {
                    if (param.IsOptional)
                    {
                        s += $"[{param.Name} = '{param.DefaultValue}'] ";
                    }
                    else if (param.IsMultiple)
                    {
                        s += $"*{param.Name}... ";
                    }
                    else if (param.IsRemainder)
                    {
                        s += $"...{param.Name} ";
                    }
                    else if (param.Type == typeof(SoundName))
                    {
                        s += "`";
                        s += string.Join("`|`", Enum.GetNames(typeof(SoundName)));
                        s += "`";
                    }
                    else if (param.Type == typeof(int))
                    {
                        s += $"-{param.Name}#";
                    }
                    else
                    {
                        s += $"?{param.Name} ";
                    }
                }
            }
            return s;
        }
    }
}
