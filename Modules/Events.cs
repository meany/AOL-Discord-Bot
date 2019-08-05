using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dm.AOL.Bot;
using dm.AOL.Bot.Modules;

namespace dm.AOL.Bot
{
    public class Events
    {
        private readonly CommandService commands;
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider services;
        private readonly Config config;

        public Events(CommandService commands, DiscordSocketClient client, IServiceProvider services, Config config)
        {
            this.commands = commands;
            this.client = client;
            this.services = services;
            this.config = config;
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message))
                return;

            int argPos = 0;
            var context = new CommandContext(client, message);

            if (message.HasStringPrefix("?", ref argPos) || message.HasStringPrefix("{S", ref argPos))
            {
                var result = await commands.ExecuteAsync(context, 0, services).ConfigureAwait(false);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand && result.Error != CommandError.ParseFailed)
                    await context.Channel.SendMessageAsync(result.ErrorReason).ConfigureAwait(false);
                return;
            }

            if (message.HasStringPrefix("//roll-dice", ref argPos))
            {
                var pattern = @"^\/\/roll-dice([1-9]|[1][0-5])-sides([2-9]|[1-9][0-9]|[1-9][0-9][0-9])$";
                var r = new Regex(pattern);
                var m = r.Match(message.Content);

                if (m.Success && m.Groups.Count == 3)
                {
                    int dice = int.Parse(m.Groups[1].Value);
                    int sides = int.Parse(m.Groups[2].Value);

                    await new Roller(config).Roll(context, dice, sides);
                }
            }

        }

        public async Task HandleConnected()
        {
            foreach (var g in client.Guilds)
            {
                await client.CurrentUser.ModifyAsync(x =>
                {
                    x.Username = "OnlineHost";
                }).ConfigureAwait(false);
            }

            //var chan = (ITextChannel)client.GetChannel(config.ChannelIds[0]);
            //if (chan != null)
            //{
            //    await chan.SendMessageAsync($"Started at {DateTime.Now.ToDate()}").ConfigureAwait(false);
            //}
        }

        internal async Task HandleJoin(SocketGuildUser arg)
        {
            var dmChannel = await arg.GetOrCreateDMChannelAsync().ConfigureAwait(false);
            await dmChannel.SendMessageAsync("\\*\\*\\* You are in \"Town Square - Lobby 1\". \\*\\*\\*").ConfigureAwait(false);

            foreach (var channelId in config.ChannelIds) {
                var channel = (ITextChannel)client.GetChannel(channelId);
                await channel.SendMessageAsync($"{arg.Mention} has entered the room.").ConfigureAwait(false);
            }
        }

        internal async Task HandleLeft(SocketGuildUser arg)
        {
            foreach (var channelId in config.ChannelIds)
            {
                var channel = (ITextChannel)client.GetChannel(channelId);
                await channel.SendMessageAsync($"{arg.Mention} has left the room.").ConfigureAwait(false);
            }
        }

        //public async Task HandleReaction(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction reaction)
        //{
        //    return;
        //}
    }
}
