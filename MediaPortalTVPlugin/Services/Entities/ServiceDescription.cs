namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class ServiceDescription
    {
        public int ApiVersion { get; set; }
        public bool HasConnectionToTVServer { get; set; }
        public string ServiceVersion { get; set; }
    }
}