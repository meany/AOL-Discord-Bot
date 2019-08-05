using System.Collections.Generic;

namespace dm.AOL.Bot
{
    public class Config
    {
        public string BotToken { get; set; }
        public List<ulong> ChannelIds { get; set; }
        public int RequestCooldown { get; set; }
    }
}
