using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dm.AOL.Bot
{
    public class Program
    {
        private CommandService commands;
        private DiscordSocketClient discordClient;
        private IServiceProvider services;
        private IConfigurationRoot configuration;
        private Config config;
        private SocketRole gagRole;
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("Config.Local.json", optional: true, reloadOnChange: true);

                configuration = builder.Build();

                discordClient = new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 100
                });
                discordClient.Log += Log;

                services = new ServiceCollection()
                    .Configure<Config>(configuration)
                    .BuildServiceProvider();
                config = services.GetService<IOptions<Config>>().Value;

                if (args.Length > 0)
                {
                    await RunArgs(args).ConfigureAwait(false);
                }
                else
                {
                    await RunBot().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task RunArgs(string[] args)
        {
            try
            {
                //await Start().ConfigureAwait(false);
                //var handle = new Args(discordClient);
                //switch (args[0])
                //{
                //    case "arg1":
                //        if (int.TryParse(args[1], out int depositId))
                //        {
                //            await handle.Arg1(depositId).ConfigureAwait(false);
                //        }
                //        else
                //        {
                //            log.Warn($"Could not parse Arg1 '{args[1]}'");
                //        }
                //        break;
                //}
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task RunBot()
        {
            try
            {
                commands = new CommandService();

                await Install().ConfigureAwait(false);
                await Start().ConfigureAwait(false);

                string[] statuses = new string[]
                {
                    "Initializing modem...",
                    "Dialing 800-827-6364...",
                    "Connected at 56600 bps...",
                    "Requesting network attention...",
                    "Talking to network...",
                    "Connecting to America Online...",
                    "Checking password..."
                };
                int i = 0;

                while (true)
                {
                    await discordClient.SetGameAsync(statuses[i]).ConfigureAwait(false);
                    await Task.Delay(5000);
                    i = (i == 6) ? 0 : i + 1;
                }

                //await Task.Delay(-1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private Task Log(LogMessage msg)
        {
            log.Info(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task Install()
        {
            try
            {
                var events = new Events(commands, discordClient, services, config);
                discordClient.Connected += events.HandleConnected;
                discordClient.MessageReceived += events.HandleCommand;
                discordClient.UserJoined += events.HandleJoin;
                discordClient.UserLeft += events.HandleLeft;
                discordClient.UserBanned += events.HandleBanned;
                //client.ReactionAdded += events.HandleReaction;
                await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async Task Start()
        {
            try
            {
                await discordClient.LoginAsync(TokenType.Bot, config.BotToken).ConfigureAwait(false);
                await discordClient.StartAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
