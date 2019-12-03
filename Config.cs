using System.Collections.Generic;

namespace dm.AOL.Bot
{
    public class Config
    {
        public string BotToken { get; set; }
        public ulong GuildId { get; set; }
        public List<ulong> ChannelIds { get; set; }
        public ulong AdminChannelId { get; set; }
        public ulong GagRoleId { get; set; }
        public List<ulong> AdminRoleIds { get; set; }
        public string EmoteGagged { get; set; }
        public ulong PoleshiftUserId { get; set; }
        public ulong PoleshiftRoleId { get; set; }
    }
}
