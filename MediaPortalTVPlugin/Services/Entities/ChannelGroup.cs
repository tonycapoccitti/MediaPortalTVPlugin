namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class ChannelGroup
    {
        public string GroupName { get; set; }
        public int Id { get; set; }
        public bool IsChanged { get; set; }
        public int SortOrder { get; set; }
        public bool IsRadio { get; set; }
        public bool IsTv { get; set; }
    }
}