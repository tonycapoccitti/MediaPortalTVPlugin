using System;
using MediaBrowser.Controller.Channels;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class StreamingDetails
    {
        public String Id { get; set; }
        public String StreamIdentifier { get; set; }
        public ChannelMediaInfo StreamInfo { get; set; }
    }
}
