using System;
using MediaBrowser.Model.Plugins;

namespace MediaBrowser.Plugins.MediaPortal.Configuration
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration" /> class.
        /// </summary>
        public PluginConfiguration()
        {
            ApiPortNumber = 4322;
            LiveStreamingProfileName = "Direct";
        }

        public string ApiIpAddress { get; set; }
        public Int32 ApiPortNumber { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Int32 PreRecordPaddingInSecs { get; set; }
        public Int32 PostRecordPaddingInSecs { get; set; }

        public String LiveStreamingProfileName { get; set; }
    }
}
