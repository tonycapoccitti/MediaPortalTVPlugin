namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class Channel
    {
        public int Id { get; set; }
        public bool IsRadio { get; set; }
        public bool IsTv { get; set; }
        public string Title { get; set; }
    }
}