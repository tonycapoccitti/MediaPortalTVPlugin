using System;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class User
    {
        public int CardId { get; set; }
        public int ChannelId { get; set; }
        public DateTime HeartBeat { get; set; }
        public bool IsAdmin { get; set; }
        public string Name { get; set; }
        public int SubChannel { get; set; }
        public int TvStoppedReason { get; set; }
    }
}